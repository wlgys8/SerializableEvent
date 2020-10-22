using UnityEngine;

namespace MS.Events{
    using MS.Reflection;
    using System.Reflection;
    using System.Linq;
    using System.Collections.Generic;

    public enum CallState{
        Off,
        EditorAndRuntime	,
        RuntimeOnly
    }

    public enum ArguementMode{
        Static,
        Dynamic
    }

    [System.Serializable]
    public class SerializableCallback:System.ICloneable
    #if UNITY_EDITOR
    ,ISerializationCallbackReceiver
    #endif
    {

        [SerializeField]
        private CallState _callState = CallState.RuntimeOnly;

        [SerializeField]
        private Object _target;

        [SerializeField]
        private string _funcName;

        [SerializeField]
        private List<SerializableArguement> _arguements;

        [SerializeField]
        private ArguementMode _arguementMode = ArguementMode.Static;

        private ArguementsCache _arguementsCache;

        [System.NonSerialized]
        private BaseCachedCall _cachedCall;

        [System.NonSerialized]
        private bool _isCachedCallDirty = true;

        #if UNITY_EDITOR
        internal System.Type[] constrainedDynamicArguementTypes{
            get;set;
        }
        #endif

        public SerializableCallback():this(null,null){
        }

        public SerializableCallback(Object target,MethodInfo methodInfo){
            _callState = CallState.RuntimeOnly;
            this.Reset(target,methodInfo);
        }

        public void Reset(Object target,MethodInfo methodInfo){
            if(methodInfo != null){
                if(target == null){
                    throw new System.ArgumentNullException("target");
                }
                if(!methodInfo.DeclaringType.IsAssignableFrom(target.GetType())){
                    throw new System.ArgumentException($"typemismatch {target.GetType()} vs {methodInfo.DeclaringType.GetType()}");
                }    
            }
            _target = target;
            _funcName = methodInfo == null ? null :methodInfo.Name;
            if(methodInfo != null){
                var parameters = methodInfo.GetParameters();
                RebuildArguements(parameters);
            }else{
                RebuildArguements(null);
            }
            _isCachedCallDirty = true;
            _cachedCall = null;      
            _arguementsCache = null;  
        }

        private void RebuildArguements(ParameterInfo[] parameterInfos){
            if(_arguements != null){
                _arguements.Clear();
            }
            if(parameterInfos != null){
                if(_arguements == null){
                    _arguements = new List<SerializableArguement>();
                }
                foreach(var parameterInfo in parameterInfos){
                    var arg = new SerializableArguement(parameterInfo.ParameterType);
                    arg.arguementName = parameterInfo.Name;
                    _arguements.Add(arg);
                }
            }
        }

        private ArguementsCache arguementsCache{
            get{
                if(_arguementsCache == null){
                    _arguementsCache = new ArguementsCache(_arguements);
                }
                return _arguementsCache;
            }
        }

        public System.Type[] arguementTypes{
            get{
                return arguementsCache.types;
            }
        }

        public CallState callState{
            get{
                return _callState;
            }set{
                _callState = value;
            }
        }

        public int arguementsCount{
            get{
                return arguementsCache.count;
            }
        }

        /// <summary>
        /// Validate if methodInfo exists by reflection.
        /// </summary>
        /// <returns></returns>
        public bool Validate(){
            if(_target == null){
                return false;
            }
            var methodInfo = _target.GetType().GetMethod(_funcName,arguementsCache.types);
            if(methodInfo == null){
                Debug.LogWarning($"{_target.GetType()}.{_funcName}: {arguementsCache.types.Length}");
                return false;
            }
            return true;
        }

        public MethodInfo GetMethodInfo(){
            if(_target == null){
                return null;
            }
            return _target.GetType().GetMethod(_funcName,arguementsCache.types);
        }

        public Object target{
            get{
                return _target;
            }
        }

        public string funcName{
            get{
                return _funcName;
            }
        }

        public void Invoke(object[] args){
            if(_callState == CallState.Off){
                return;
            }
            #if UNITY_EDITOR
            if(_callState == CallState.EditorAndRuntime){
                DoActualInvoke(args);
            }else{
                if(Application.isPlaying){
                    DoActualInvoke(args);
                }
            }
            #else
            DoActualInvoke(args);
            #endif
            
        }

        private void DoActualInvoke(object[] args){
            if(_cachedCall == null){
                if(!_isCachedCallDirty){
                    return;
                }
                _isCachedCallDirty = false;
                try{
                    _cachedCall = CachedCallFactory.Create(_target,_funcName,arguementsCache.types,false);
                    if(_cachedCall == null){
                        Debug.LogWarning($"failed to invoke method:{_target.GetType()}.{_funcName}");
                        return;
                    }
                }catch(System.Exception e){
                    Debug.LogException(e);
                }
            }
            if(_arguementMode == ArguementMode.Dynamic){
                if(args.Length != this.arguementsCount){
                    throw new System.ArgumentException($"arguements count mismatch for <{_target.GetType()}.{_funcName}>");
                }
                _cachedCall.Invoke(args);
            }else{
                _cachedCall.Invoke(arguementsCache.values);
            }
        }

        public void SetCachedValuesDirty(){
            arguementsCache.SetValuesDirty();
        }
        public object Clone()
        {
            var clone = new SerializableCallback();
            clone._target = _target;
            clone._funcName = _funcName;
            clone._arguementMode = _arguementMode;
            clone._arguements = new List<SerializableArguement>();
            clone._callState = _callState;
            if(_arguements != null){
                foreach(var arg in _arguements){
                    clone._arguements.Add(arg.Clone() as SerializableArguement);
                }
            }
            return clone;
        }

#if UNITY_EDITOR

        private bool _editorArguementsNamesDirty = true;
        /// <summary>
        /// Only for Editor
        /// </summary>
        public bool UpdateEditorArguementNamesIfRequired(){
            if(!_editorArguementsNamesDirty){
                return false;
            }
            _editorArguementsNamesDirty = false;
            var methodInfo = GetMethodInfo();
            if(methodInfo != null){
                var ps = methodInfo.GetParameters();
                if(_arguements == null || ps.Length != _arguements.Count){
                    return false;
                }
                for(var i = 0; i < ps.Length;i ++){
                    _arguements[i].arguementName = ps[i].Name;
                }
                return true;
            }
            return false;
            
        }
        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            _editorArguementsNamesDirty = true;
            _arguementsCache = null;
            _cachedCall = null;
            _isCachedCallDirty = true;
        }
#endif
        static SerializableCallback(){
            RegisterCacheCallArguementTypes();
        }

        private static void RegisterCacheCallArguementTypes(){
            CachedCallArguementsTypeRegister.EnsureCall<int>();
            CachedCallArguementsTypeRegister.EnsureCall<bool>();
            CachedCallArguementsTypeRegister.EnsureCall<float>();
            CachedCallArguementsTypeRegister.EnsureCall<Rect>();
            CachedCallArguementsTypeRegister.EnsureCall<RectInt>();
            CachedCallArguementsTypeRegister.EnsureCall<Color>();
            CachedCallArguementsTypeRegister.EnsureCall<Vector2>();
            CachedCallArguementsTypeRegister.EnsureCall<Vector2Int>();
            CachedCallArguementsTypeRegister.EnsureCall<Vector3>();
            CachedCallArguementsTypeRegister.EnsureCall<Vector3Int>();
            CachedCallArguementsTypeRegister.EnsureCall<Vector4>();
            CachedCallArguementsTypeRegister.EnsureCall<Quaternion>();
        }

        public static bool CheckMethodWithArguementSupportInAOT(System.Type[] arguementTypes,System.Type returnType = null){
            if(returnType == null || returnType == typeof(void)){
                return CachedCallArguementsTypeRegister.IsCallSupport(arguementTypes);
            }else{
                return CachedCallArguementsTypeRegister.IsFuncSupport(returnType,arguementTypes);
            }
        }


    }

}
