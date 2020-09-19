using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MS.Events.Editor{
    using UnityEditor;
    using UnityEditorInternal;

    [CustomPropertyDrawer(typeof(SerializableCallbackGroup))]
    internal class SerializableCallbackGroupEditor : PropertyDrawer
    {

        internal class State{
            public ReorderableList list;
            public GUIContent label;
        }

        private Dictionary<string, State> _states = new Dictionary<string, State>();

        private State GetState(SerializedProperty property){
            if(_states.ContainsKey(property.propertyPath)){
                return _states[property.propertyPath];
            }
            var serializedObject = property.serializedObject;
            var callbacks = property.FindPropertyRelative("_callbacks");
            var reorderableList = new ReorderableList(serializedObject,callbacks);

            reorderableList.drawHeaderCallback = (rect)=>{
                var label = _states[property.propertyPath].label;
                EditorGUI.LabelField(rect,label);
            };

            reorderableList.elementHeightCallback = (index)=>{
                var item = callbacks.GetArrayElementAtIndex(index);
                return EditorGUI.GetPropertyHeight(item) + 5;
            };

            reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused)=>{
                var item = callbacks.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect,item);
            };
            reorderableList.onAddCallback = (list)=>{
                callbacks.InsertArrayElementAtIndex(callbacks.arraySize);
                serializedObject.ApplyModifiedProperties();
                var lastItem = callbacks.GetArrayElementAtIndex(callbacks.arraySize - 1);
                var callback = EditorHelper.GetTargetObjectOfProperty(lastItem) as SerializableCallback;
                callback.callState = CallState.RuntimeOnly;
                callback.Reset(null,null);
            } ;

            reorderableList.onRemoveCallback = (list)=>{
                callbacks.DeleteArrayElementAtIndex(list.index);
                serializedObject.ApplyModifiedProperties();
            };
            var state = new State(){
                list = reorderableList
            };
            _states.Add(property.propertyPath,state);
            return state;
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label){
            var state = GetState(property);
            return state.list.GetHeight();
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label){
            var callbackGroup = EditorHelper.GetTargetObjectOfProperty(property) as SerializableCallbackGroup;
            callbackGroup.UpdateEditorConstrainedDynamicArguementTypesIfRequired();
            var state = GetState(property);
            if(state.label == null){
                state.label = new GUIContent(label);
            }
            state.list.DoList(rect);
        }
    }
}
