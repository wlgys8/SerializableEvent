using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MS.Events{



    [System.Serializable]
    public class SerializableCallbackGroup
    #if UNITY_EDITOR
    :ISerializationCallbackReceiver
    #endif
    {

        [SerializeField]
        private List<SerializableCallback> _callbacks;
    
        #if UNITY_EDITOR
        private bool _editorConstrainedDynamicArguementTypesDirty = true;
        private System.Type[] _constrainedDynamicArguementTypes;
        internal System.Type[] constrainedDynamicArguementTypes{
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
                    callback.constrainedDynamicArguementTypes = constrainedDynamicArguementTypes;
                }
            }
        }
        #endif

        public void AddCallback(Object target,System.Reflection.MethodInfo methodInfo){
            var callback = new SerializableCallback(target,methodInfo);
            #if UNITY_EDITOR
            callback.constrainedDynamicArguementTypes = constrainedDynamicArguementTypes;
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

        #if UNITY_EDITOR
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
