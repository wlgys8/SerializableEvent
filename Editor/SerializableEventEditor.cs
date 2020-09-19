using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MS.Events.Editor{

    [CustomPropertyDrawer(typeof(SerializableEventBase),true)]
    internal class SerializableEventEditor:PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label){
            var persistentCalls = property.FindPropertyRelative("_persistentCalls");
            return EditorGUI.GetPropertyHeight(persistentCalls);
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label){
            var persistentCalls = property.FindPropertyRelative("_persistentCalls");
            var evt = EditorHelper.GetTargetObjectOfProperty(property);
            var type = evt.GetType();
            var arguementTypeNames = GetFormatedArguementTypeNames(type);
            EditorGUI.PropertyField(rect,persistentCalls,new GUIContent(label.text + " " + arguementTypeNames),true);
        }


        private static HashSet<System.Type> _eventDefinitionTypes = new HashSet<System.Type>(){
            typeof(SerializableEvent<>),
            typeof(SerializableEvent<,>),
            typeof(SerializableEvent<,,>),
            typeof(SerializableEvent<,,>),
        };


        private static string GetFormatedArguementTypeNames(System.Type eventType){
            // var typeName = eventType.Name;
            var genericEventType = GetDerivedGenericEventType(eventType);
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            // builder.Append(typeName);
            builder.Append("(");
            if(genericEventType != null){
                var index = 0;
                var arguements = genericEventType.GetGenericArguments();
                foreach(var p in arguements){
                    builder.Append(p.Name);
                    if(index < arguements.Length - 1){
                        builder.Append(",");
                    }
                    index ++;
                }
            }
            builder.Append(")");
            return builder.ToString();
        }

        private static System.Type GetDerivedGenericEventType(System.Type type){
            if(type == null){
                return null;
            }
            if(type.IsGenericType){
                if(_eventDefinitionTypes.Contains(type.GetGenericTypeDefinition())){
                    return type;
                }
            }
            return GetDerivedGenericEventType(type.BaseType);
        }
    }
}
