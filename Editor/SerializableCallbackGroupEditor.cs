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
            var callbacksSP = property.FindPropertyRelative("_callbacks");
            var reorderableList = new ReorderableList(serializedObject,callbacksSP);

            reorderableList.drawHeaderCallback = (rect)=>{
                var label = _states[property.propertyPath].label;
                EditorGUI.LabelField(rect,label);
                this.DrawSettingButton(rect,property);
            };

            reorderableList.elementHeightCallback = (index)=>{
                var item = callbacksSP.GetArrayElementAtIndex(index);
                return EditorGUI.GetPropertyHeight(item) + 5;
            };

            reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused)=>{
                var item = callbacksSP.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect,item);
            };
            reorderableList.onAddCallback = (list)=>{
                var callbacks = EditorHelper.GetTargetObjectOfProperty(property) as SerializableCallbackGroup;
                callbacks.AddCallback(null,null);
                serializedObject.UpdateIfRequiredOrScript();
            } ;

            reorderableList.onRemoveCallback = (list)=>{
                callbacksSP.DeleteArrayElementAtIndex(list.index);
                serializedObject.ApplyModifiedProperties();
            };
            var state = new State(){
                list = reorderableList
            };
            _states.Add(property.propertyPath,state);
            return state;
        }

        public struct CustomMenuItem{
            public string menuName;
            public GenericMenu.MenuFunction2 action;
            public object userData;
        }

        static Dictionary<string,CustomMenuItem> _customMenuItems = new Dictionary<string, CustomMenuItem>();
        private static  HashSet<string> _builtinMenuNames = new HashSet<string>{"Copy","Paste"};
        static internal void AddCustomGenericContextMenuItem(CustomMenuItem item){
            if(_builtinMenuNames.Contains(item.menuName)){
                Debug.LogWarning("Can not use builtin menuName:" + item.menuName);
                return;
            }
            _customMenuItems.Add(item.menuName,item);
        }

        private static void ClearCustomGenericContextMenuItem(){
            _customMenuItems.Clear();
        }

        private void DrawSettingButton(Rect headerRect,SerializedProperty property){
            var rect = new Rect(headerRect.xMax - 20,headerRect.y,15,headerRect.height);
            var icon = EditorGUIUtility.Load("_Popup") as Texture;
            if(GUI.Button(rect,new GUIContent(icon),GUIStyle.none)){
                ShowSettingContextMenu(property);
            }
            ClearCustomGenericContextMenuItem();
        }

        private void ShowSettingContextMenu(SerializedProperty property){
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Copy"),false,OnSettingMenuClick,new SettingContext(){
                commandName = "Copy",
                property = property
            });
            if(_callbacksGroupCopied != null){
                menu.AddItem(new GUIContent("Paste"),false,OnSettingMenuClick,new SettingContext(){
                    commandName = "Paste",
                    property = property
                });
            }else{
                menu.AddDisabledItem(new GUIContent("Paste"));
            }

            foreach(var kv in _customMenuItems){
                menu.AddItem(new GUIContent(kv.Key),false,kv.Value.action,kv.Value.userData);
            }
            menu.ShowAsContext();
        }

        private static SerializableCallbackGroup _callbacksGroupCopied;
        private void OnSettingMenuClick(object userData){
            var context = (SettingContext)userData;
            if(context.commandName == "Copy"){
                var callbacksGroup = EditorHelper.GetTargetObjectOfProperty(context.property) as SerializableCallbackGroup;
                _callbacksGroupCopied = callbacksGroup.Clone() as SerializableCallbackGroup;
            }else if(context.commandName == "Paste"){
                if(_callbacksGroupCopied != null){
                    EditorHelper.SetTargetObjectOfProperty(context.property,_callbacksGroupCopied.Clone());
                    context.property.serializedObject.UpdateIfRequiredOrScript();
                }
            }
        }

        private struct SettingContext{
            public string commandName;
            public SerializedProperty property;

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
