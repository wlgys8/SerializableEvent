using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MS.Events{

    [System.Serializable]
    public class SerializableEventBase:System.ICloneable{

        [SerializeField]
        private SerializableCallbackGroup _persistentCalls;

        protected void InvokePersistentCalls(object[] args){
            if(_persistentCalls != null){
                _persistentCalls.Invoke(args);
            }
        }

        public virtual object Clone()
        {
            var clone = this.MemberwiseClone() as SerializableEventBase;
            if(_persistentCalls != null){
                clone._persistentCalls = _persistentCalls.Clone() as SerializableCallbackGroup;
            }
            return clone;
        }


#if UNITY_EDITOR
        internal System.Type[] constrainedDynamicArguementTypes{
            set{
                _persistentCalls.editorConstrainedDynamicArguementTypes = value;
            }
        }
        #endif
    }



    [System.Serializable]
    public class SerializableEvent:SerializableEventBase
    #if UNITY_EDITOR
    ,ISerializationCallbackReceiver
    #endif
    {

        private List<UnityAction> _runtimeCalls = new List<UnityAction>();
        private static readonly object[] EMPTY_ARGS = new object[]{};

        #if UNITY_EDITOR
        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            this.constrainedDynamicArguementTypes = new System.Type[0];;
        }
        #endif

        public void Invoke(){
            this.InvokePersistentCalls(EMPTY_ARGS);
            for(var i = 0; i < _runtimeCalls.Count; i ++){
                _runtimeCalls[i].Invoke();
            }
        }

        /// <summary>
        /// add runtime callbacks
        /// </summary>
        /// <param name="listener"></param>
        public void AddListener(UnityAction listener){
            _runtimeCalls.Add(listener);
        }

        /// <summary>
        /// remove runtime callbacks
        /// </summary>
        /// <param name="listener"></param>
        public void RemoveListener(UnityAction listener){
            _runtimeCalls.Remove(listener);
        }

        public override object Clone()
        {
            var clone = base.Clone() as SerializableEvent;
            clone._runtimeCalls = new List<UnityAction>();
            foreach(var call in _runtimeCalls){
                clone._runtimeCalls.Add(call);
            }
            return clone;
        }

    }


    [System.Serializable]
    public class SerializableEvent<T0>:SerializableEventBase
    #if UNITY_EDITOR
    ,ISerializationCallbackReceiver
    #endif
    {

        private List<UnityAction<T0>> _runtimeCalls = new List<UnityAction<T0>>();
        private readonly object[] _args = new object[1];

        #if UNITY_EDITOR
        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            this.constrainedDynamicArguementTypes = new System.Type[]{typeof(T0)};
        }
        #endif
        
        public void Invoke(T0 arg){
            _args[0] = arg;
            this.InvokePersistentCalls(_args);
            for(var i = 0; i < _runtimeCalls.Count;i++){
                _runtimeCalls[i](arg);
            }
        }

        /// <summary>
        /// add runtime callbacks
        /// </summary>
        /// <param name="listener"></param>
        public void AddListener(UnityAction<T0> listener){
            _runtimeCalls.Add(listener);
        }

        /// <summary>
        /// remove runtime callbacks
        /// </summary>
        /// <param name="listener"></param>
        public void RemoveListener(UnityAction<T0> listener){
            _runtimeCalls.Remove(listener);
        }

        public override object Clone()
        {
            var clone = base.Clone() as SerializableEvent<T0>;
            clone._runtimeCalls = new List<UnityAction<T0>>();
            foreach(var call in _runtimeCalls){
                clone._runtimeCalls.Add(call);
            }
            return clone;
        }
    }


    [System.Serializable]
    public class SerializableEvent<T0,T1>:SerializableEventBase
    #if UNITY_EDITOR
    ,ISerializationCallbackReceiver
    #endif
    {

        private List<UnityAction<T0,T1>> _runtimeCalls = new List<UnityAction<T0,T1>>();
        private readonly object[] _args = new object[2];

        #if UNITY_EDITOR
        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            this.constrainedDynamicArguementTypes = new System.Type[]{typeof(T0),typeof(T1)};
        }
        #endif
        
        public void Invoke(T0 arg0,T1 arg1){
            _args[0] = arg0;
            _args[1] = arg1;
            this.InvokePersistentCalls(_args);
            for(var i = 0; i < _runtimeCalls.Count;i++){
                _runtimeCalls[i](arg0,arg1);
            }
        }

        /// <summary>
        /// add runtime callbacks
        /// </summary>
        /// <param name="listener"></param>
        public void AddListener(UnityAction<T0,T1> listener){
            _runtimeCalls.Add(listener);
        }

        /// <summary>
        /// remove runtime callbacks
        /// </summary>
        /// <param name="listener"></param>
        public void RemoveListener(UnityAction<T0,T1> listener){
            _runtimeCalls.Remove(listener);
        }
        public override object Clone()
        {
            var clone = base.Clone() as SerializableEvent<T0,T1>;
            clone._runtimeCalls = new List<UnityAction<T0,T1>>();
            foreach(var call in _runtimeCalls){
                clone._runtimeCalls.Add(call);
            }
            return clone;
        }
    }


    [System.Serializable]
    public class SerializableEvent<T0,T1,T2>:SerializableEventBase
    #if UNITY_EDITOR
    ,ISerializationCallbackReceiver
    #endif
    {

        private List<UnityAction<T0,T1,T2>> _runtimeCalls = new List<UnityAction<T0,T1,T2>>();
        private readonly object[] _args = new object[3];

        #if UNITY_EDITOR
        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            this.constrainedDynamicArguementTypes = new System.Type[]{typeof(T0),typeof(T1),typeof(T2)};
        }
        #endif
        
        public void Invoke(T0 arg0,T1 arg1,T2 arg2){
            _args[0] = arg0;
            _args[1] = arg1;
            _args[2] = arg2;
            this.InvokePersistentCalls(_args);
            for(var i = 0; i < _runtimeCalls.Count;i++){
                _runtimeCalls[i](arg0,arg1,arg2);
            }
        }

        /// <summary>
        /// add runtime callbacks
        /// </summary>
        /// <param name="listener"></param>
        public void AddListener(UnityAction<T0,T1,T2> listener){
            _runtimeCalls.Add(listener);
        }

        /// <summary>
        /// remove runtime callbacks
        /// </summary>
        /// <param name="listener"></param>
        public void RemoveListener(UnityAction<T0,T1,T2> listener){
            _runtimeCalls.Remove(listener);
        }

        public override object Clone()
        {
            var clone = base.Clone() as SerializableEvent<T0,T1,T2>;
            clone._runtimeCalls = new List<UnityAction<T0,T1,T2>>();
            foreach(var call in _runtimeCalls){
                clone._runtimeCalls.Add(call);
            }
            return clone;
        }
    }


    [System.Serializable]
    public class SerializableEvent<T0,T1,T2,T3>:SerializableEventBase
    #if UNITY_EDITOR
    ,ISerializationCallbackReceiver
    #endif
    {

        private List<UnityAction<T0,T1,T2,T3>> _runtimeCalls = new List<UnityAction<T0,T1,T2,T3>>();
        private readonly object[] _args = new object[4];

        #if UNITY_EDITOR
        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            this.constrainedDynamicArguementTypes = new System.Type[]{typeof(T0),typeof(T1),typeof(T2),typeof(T3)};
        }
        #endif
        
        public void Invoke(T0 arg0,T1 arg1,T2 arg2,T3 arg3){
            _args[0] = arg0;
            _args[1] = arg1;
            _args[2] = arg2;
            _args[3] = arg3;
            this.InvokePersistentCalls(_args);
            for(var i = 0; i < _runtimeCalls.Count;i++){
                _runtimeCalls[i](arg0,arg1,arg2,arg3);
            }
        }

        /// <summary>
        /// add runtime callbacks
        /// </summary>
        /// <param name="listener"></param>
        public void AddListener(UnityAction<T0,T1,T2,T3> listener){
            _runtimeCalls.Add(listener);
        }

        /// <summary>
        /// remove runtime callbacks
        /// </summary>
        /// <param name="listener"></param>
        public void RemoveListener(UnityAction<T0,T1,T2,T3> listener){
            _runtimeCalls.Remove(listener);
        }

        public override object Clone()
        {
            var clone = base.Clone() as SerializableEvent<T0,T1,T2,T3>;
            clone._runtimeCalls = new List<UnityAction<T0,T1,T2,T3>>();
            foreach(var call in _runtimeCalls){
                clone._runtimeCalls.Add(call);
            }
            return clone;
        }
    }
}
