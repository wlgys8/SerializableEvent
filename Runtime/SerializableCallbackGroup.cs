using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MS.Events{



    [System.Serializable]
    public class SerializableCallbackGroup:System.ICloneable
    #if UNITY_EDITOR
    ,ISerializationCallbackReceiver
    #endif
    {

        [SerializeField]
        private List<SerializableCallback> _callbacks;

        public void AddCallback(Object target,System.Reflection.MethodInfo methodInfo){
            var callback = new SerializableCallback(target,methodInfo);
            #if UNITY_EDITOR
            callback.constrainedDynamicArguementTypes = editorConstrainedDynamicArguementTypes;
            #endif
            if(_callbacks == null){
                _callbacks = new List<SerializableCallback>();
            }
            _callbacks.Add(callback);
        }

        public void Invoke(object[] args){
            if(_callbacks == null || _callbacks.Count == 0){
                return;
            }
            for(var i = 0; i < _callbacks.Count;i++){
                _callbacks[i].Invoke(args);
            }
        }

        public int count{
            get{
                if(_callbacks == null){
                    return 0;
                }
                return _callbacks.Count;
            }
        }
        public object Clone()
        {
            var clone = new SerializableCallbackGroup();
            if(_callbacks != null){
                clone._callbacks = new List<SerializableCallback>();
                foreach(var c in _callbacks){
                    clone._callbacks.Add(c.Clone() as SerializableCallback);
                }
            }
            return clone;
        }


        #if UNITY_EDITOR
        private bool _editorConstrainedDynamicArguementTypesDirty = true;
        private System.Type[] _constrainedDynamicArguementTypes;
        internal System.Type[] editorConstrainedDynamicArguementTypes{
            get{
                return _constrainedDynamicArguementTypes;
            }set{
                _constrainedDynamicArguementTypes = value;
                _editorConstrainedDynamicArguementTypesDirty = true;
            }
        }

        public void UpdateEditorConstrainedDynamicArguementTypesIfRequired(){
            if(!_editorConstrainedDynamicArguementTypesDirty){
                return;
            }
            _editorConstrainedDynamicArguementTypesDirty = false;
            if(_callbacks != null){
                foreach(var callback in _callbacks){
                    callback.constrainedDynamicArguementTypes = editorConstrainedDynamicArguementTypes;
                }
            }
        }
        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            _editorConstrainedDynamicArguementTypesDirty = true;
        }

        #endif
    }
}
