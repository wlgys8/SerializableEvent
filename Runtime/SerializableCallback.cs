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
    public class SerializableCallback
    #if UNITY_EDITOR
    :ISerializationCallbackReceiver
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

        private ArguementsCache _arguementsCache;

        [SerializeField]
        private ArguementMode _arguementMode = ArguementMode.Static;

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

        private static readonly System.Type[] EMPTY_TYPES = new System.Type[0];

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
            if(!Application.isPlaying){
                if(_callState == CallState.EditorAndRuntime){
                    DoActualInvoke(args);
                }
                return;
            }
            #endif
            DoActualInvoke(args);
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

        public static bool CheckMethodWithArguementSupportInAOT(System.Type[] arguementTypes){
            return CachedCallArguementsTypeRegister.IsCallSupport(arguementTypes);
        }

    }

}
