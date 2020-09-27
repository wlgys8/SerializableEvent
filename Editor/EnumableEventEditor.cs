using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using UnityEditorInternal;

namespace MS.Events.Editor{
    [CustomPropertyDrawer(typeof(EnumableEventBase),true)]
    public class EnumableEventEditor : PropertyDrawer
    {
        private static ReorderableList.Defaults s_defaults;

        private static ReorderableList.Defaults defaults{
            get{
                if(s_defaults == null){
                    s_defaults = new ReorderableList.Defaults();
                }
                return s_defaults;
            }
        }

        private const int titleBarHeight = 15;
        private const int verticalSpace = 8;
        private const int indentSpace = 5;

        private const int addStatusButtonHeight = 20;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var values = property.FindPropertyRelative("_events");
            var height = 0f;
            height += titleBarHeight;
            height += verticalSpace;
            if(values != null){
                for(var i = 0; i < values.arraySize;i++){
                    var value = values.GetArrayElementAtIndex(i);
                    height += EditorGUI.GetPropertyHeight(value);
                }
            }
            height += addStatusButtonHeight;
            height += 10;
            return height;
        }
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            
            var y = rect.y;
            var x = rect.x;
            var width = rect.width;
            var drawBackground = true;
            var drawHeader = true;
            var attribute = this.fieldInfo.GetCustomAttribute<EnumableEventStyleAttribute>();
            if(attribute != null){
                drawBackground = attribute.drawBackground;
                drawHeader = attribute.drawHeader;
            }

            if(drawBackground && Event.current.type == EventType.Repaint){
                defaults.boxBackground.Draw(rect,false,false,false,false);
            }
            if(drawHeader){
                //header
                defaults.DrawHeaderBackground(new Rect(rect.x,y,width,titleBarHeight));
                x += indentSpace;
                EditorGUI.LabelField(new Rect(x,y,width - x,titleBarHeight),label);
                x -= indentSpace;
                y += titleBarHeight;
            }


            y += verticalSpace;

            if(drawBackground){
                x += indentSpace;
                width -= indentSpace*2;
            }

            var enumType = GetDerivedGenericEnumType(this.fieldInfo.FieldType);
            var enumValues = System.Enum.GetValues(enumType);
            var keys = property.FindPropertyRelative("_enums");
            var values = property.FindPropertyRelative("_events");
            if(keys != null && values != null){
                for(var i = 0; i < keys.arraySize;i++){
                    var enumValueIndex = keys.GetArrayElementAtIndex(i).enumValueIndex;
                    var enumValue = enumValues.GetValue(enumValueIndex);
                    var value = values.GetArrayElementAtIndex(i);
                    var height = EditorGUI.GetPropertyHeight(value);
                    SerializableCallbackGroupEditor.AddCustomGenericContextMenuItem(new SerializableCallbackGroupEditor.CustomMenuItem(){
                        menuName = "Delete",
                        action = OnDeleteAction,
                        userData = new GenericMenuDeleteContext(){
                            property = property,
                            enumValue = enumValue,
                        },
                    });
                    EditorGUI.PropertyField(new Rect(x,y,width,height),value,new GUIContent(enumValue.ToString()),true);
                    y += height;
                }
            }

            if(GUI.Button(new Rect(x + width/2 - 100,y,200,addStatusButtonHeight),"Add EventType")){
                ShowAddMenu(property);
            }
        }

        private void OnDeleteAction(object userData){
            GenericMenuDeleteContext ctx = (GenericMenuDeleteContext)userData;
            this.RemoveByEnum(ctx.enumValue,ctx.property);
        }

        private struct GenericMenuDeleteContext{
            public SerializedProperty property;
            public object enumValue;

        }

        private void RemoveByEnum(object enumValue,SerializedProperty property){
            var fieldType = this.fieldInfo.FieldType;
            var addMethod = fieldType.GetMethod("Remove",new System.Type[]{enumValue.GetType()});
            var instance = EditorHelper.GetTargetObjectOfProperty(property);
            addMethod.Invoke(instance,new object[]{enumValue});
            property.serializedObject.UpdateIfRequiredOrScript();
        }


        private HashSet<object> CreateExistsEnumIndexes(SerializedProperty property){
            var keys = property.FindPropertyRelative("_enums");
            var set = new HashSet<object>();
            if(keys != null){
                for(var i = 0; i < keys.arraySize;i++){
                    var index = keys.GetArrayElementAtIndex(i).enumValueIndex;
                    set.Add(index);
                }
            }
            return set;
        }

        private void ShowAddMenu(SerializedProperty property){
            var menu = new GenericMenu();
            var enumType = GetDerivedGenericEnumType(this.fieldInfo.FieldType);
            var enumValues = System.Enum.GetValues(enumType);
            var existsEnumIndexes = CreateExistsEnumIndexes(property);
            for(var i = 0; i < enumValues.Length; i ++){
                var enumValue = (System.Enum)enumValues.GetValue(i);
                if(existsEnumIndexes.Contains(i)){
                    menu.AddDisabledItem(new GUIContent(enumValue.ToString()),false);
                }else{
                    menu.AddItem(new GUIContent(enumValue.ToString()),false,OnAddItemSelected,new ItemAddContext(){
                        property = property,
                        enumValue = enumValue,
                        enumType = enumType,
                    });
                }
            }
            menu.ShowAsContext();
        }

        private void OnAddItemSelected(object userData){
            var context = (ItemAddContext)userData; 
            var property = context.property;
            var keys = property.FindPropertyRelative("_enums");
            var values = property.FindPropertyRelative("_events");
            var fieldType = this.fieldInfo.FieldType;
            var addMethod = fieldType.GetMethod("Add",new System.Type[]{context.enumType});
            var instance = EditorHelper.GetTargetObjectOfProperty(property);
            addMethod.Invoke(instance,new object[]{context.enumValue});
            property.serializedObject.UpdateIfRequiredOrScript();
        }

        private static System.Type GetDerivedGenericEnumType(System.Type type){
            if(type == null){
                return null;
            }
            if(type.IsGenericType){
                if(type.GetGenericTypeDefinition() == typeof(EnumableEvent<,>)){
                    return type.GetGenericArguments()[0];
                }
            }
            return GetDerivedGenericEnumType(type.BaseType);
        }

        private struct ItemAddContext{
            public SerializedProperty property;
            public object enumValue;

            public System.Type enumType;
        }
    }
}
