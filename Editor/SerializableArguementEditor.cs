using UnityEngine;
using UnityEditor;

namespace MS.Events.Editor{

    [CustomPropertyDrawer(typeof(SerializableArguement))]
    internal class SerializableArguementEditor : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label){
            var enumIndex = property.FindPropertyRelative("_serializeType").enumValueIndex;
            var serializeType = (ArguementSerializeType)System.Enum.GetValues(typeof(ArguementSerializeType)).GetValue(enumIndex);
            
            var arguement = EditorHelper.GetTargetObjectOfProperty(property) as SerializableArguement;
            var arguementName = GUIContent.none;
            if(arguement.arguementName != null){
                arguementName = new GUIContent(arguement.arguementName);
            }

            switch(serializeType){
                case ArguementSerializeType.Object:
                return EditorGUI.GetPropertyHeight(SerializedPropertyType.ObjectReference,arguementName);
                case ArguementSerializeType.Bool:
                return EditorGUI.GetPropertyHeight(SerializedPropertyType.Boolean,arguementName);
                case ArguementSerializeType.Int:
                return EditorGUI.GetPropertyHeight(SerializedPropertyType.Integer,arguementName);
                case ArguementSerializeType.Float:
                return EditorGUI.GetPropertyHeight(SerializedPropertyType.Float,arguementName);
                case ArguementSerializeType.String:
                return EditorGUI.GetPropertyHeight(SerializedPropertyType.String,arguementName);
                case ArguementSerializeType.Vector2:
                return EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector2,arguementName);
                case ArguementSerializeType.Vector3:
                return EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector3,arguementName);
                case ArguementSerializeType.Vector4:
                return EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector4,arguementName);
                case ArguementSerializeType.Color:
                return EditorGUI.GetPropertyHeight(SerializedPropertyType.Color,arguementName);
                case ArguementSerializeType.Quaternion:
                return EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector3,arguementName);
                case ArguementSerializeType.Vector2Int:
                return EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector2Int,arguementName);
                case ArguementSerializeType.Vector3Int:
                return EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector3Int,arguementName);
                case ArguementSerializeType.Rect:
                return EditorGUI.GetPropertyHeight(SerializedPropertyType.Rect,arguementName);
                case ArguementSerializeType.RectInt:
                return EditorGUI.GetPropertyHeight(SerializedPropertyType.RectInt,arguementName);
                case ArguementSerializeType.Enum:
                return EditorGUI.GetPropertyHeight(SerializedPropertyType.Enum,arguementName);

            }
            return 18;
        }




        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label){
            var fieldInfo = this.fieldInfo;
            var arguement = EditorHelper.GetTargetObjectOfProperty(property) as SerializableArguement;
            var arguementName = GUIContent.none;
            if(arguement.arguementName != null){
                arguementName = new GUIContent(arguement.arguementName);
            }
            var serializeType = arguement.serializeType;
            var originalLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = rect.width * 0.3f;
            switch(serializeType){
                case ArguementSerializeType.Object:
                arguement.unityObject = EditorGUI.ObjectField(rect,arguementName,arguement.unityObject,arguement.type,true);
                break;
                case ArguementSerializeType.Bool:
                arguement.boolValue = EditorGUI.Toggle(rect,arguementName,arguement.boolValue);
                break;
                case ArguementSerializeType.Int:
                arguement.intValue = EditorGUI.IntField(rect,arguementName,arguement.intValue);
                break;
                case ArguementSerializeType.Float:
                arguement.floatValue = EditorGUI.FloatField(rect,arguementName,arguement.floatValue);
                break;
                case ArguementSerializeType.String:
                arguement.stringValue = EditorGUI.TextField(rect,arguementName,arguement.stringValue);
                break;
                case ArguementSerializeType.Vector2:
                arguement.vector2Value = EditorGUI.Vector2Field(rect,arguementName,arguement.vector2Value);
                break;
                case ArguementSerializeType.Vector3:
                arguement.vector3Value = EditorGUI.Vector3Field(rect,arguementName,arguement.vector3Value);
                break;
                case ArguementSerializeType.Vector4:
                arguement.vector4Value = EditorGUI.Vector4Field(rect,arguementName,arguement.vector4Value);
                break;
                case ArguementSerializeType.Color:
                arguement.colorValue = EditorGUI.ColorField(rect,arguementName,arguement.colorValue);
                break;
                case ArguementSerializeType.Quaternion:
                var eulerAngles = arguement.quaternionValue.eulerAngles;
                eulerAngles = EditorGUI.Vector3Field(rect,arguementName,eulerAngles);
                arguement.quaternionValue = Quaternion.Euler(eulerAngles);
                break;
                case ArguementSerializeType.Vector2Int:
                arguement.vector2Value = EditorGUI.Vector2IntField(rect,arguementName,arguement.vector2IntValue);
                break;
                case ArguementSerializeType.Vector3Int:
                arguement.vector3Value = EditorGUI.Vector3IntField(rect,arguementName,arguement.vector3IntValue);
                break;
                case ArguementSerializeType.Rect:
                arguement.rectValue = EditorGUI.RectField(rect,arguementName,arguement.rectValue);
                break;
                case ArguementSerializeType.RectInt:
                arguement.rectIntValue = EditorGUI.RectIntField(rect,arguementName,arguement.rectIntValue);
                break;
                case ArguementSerializeType.Enum:
                arguement.enumValue = EditorGUI.EnumPopup(rect,arguementName,(System.Enum)arguement.enumValue);
                break;
                default:
                EditorGUI.HelpBox(rect,$"NotImplemented arguement type:" + serializeType,MessageType.Error);
                break;
            }
            EditorGUIUtility.labelWidth = originalLabelWidth;
        }




    }
}
