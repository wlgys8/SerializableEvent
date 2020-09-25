using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace MS.Events.Editor{
    using UnityEditor;
    using System.Reflection;
    internal class EditorHelper
    {
        public static void SetTargetObjectOfProperty(SerializedProperty prop, object value)
        {
            var path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            var elements = path.Split('.');
            foreach (var element in elements.Take(elements.Length - 1))
            {
                if (element.Contains("["))
                {
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    obj = GetValue_Imp(obj, elementName, index);
                }
                else
                {
                    obj = GetValue_Imp(obj, element);
                }
            }

            if (Object.ReferenceEquals(obj, null)){
                Debug.LogWarning("obj is null");
                return;
            };

            try
            {
                var element = elements.Last();

                if (element.Contains("["))
                {
                    var tp = obj.GetType();
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    var field = tp.GetField(elementName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    var arr = field.GetValue(obj) as System.Collections.IList;
                    arr[index] = value;
                    // var elementName = element.Substring(0, element.IndexOf("["));
                    // var index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    // var arr = DynamicUtil.GetValue(element, elementName) as System.Collections.IList;
                    // if (arr != null) arr[index] = value;
                }
                else
                {
                    var tp = obj.GetType();
                    var field = GetFieldRecursively(tp,element);
                    if (field != null)
                    {
                       field.SetValue(obj, value);
                    }else{
                        Debug.LogWarning($"missing field {element} at type {tp}");
                    }
                    // DynamicUtil.SetValue(obj, element, value);
                }

            }
            catch
            {
                return;
            }
        }
        public static object GetTargetObjectOfProperty(SerializedProperty prop)
        {
            if (prop == null) return null;

            var path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            var elements = path.Split('.');
            foreach (var element in elements)
            {
                if (element.Contains("["))
                {
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    obj = GetValue_Imp(obj, elementName, index);
                }
                else
                {
                    obj = GetValue_Imp(obj, element);
                }
            }
            if(obj == null){
                Debug.LogWarning($"can not find actual obj from SerializedProperty:{prop.propertyPath}");
            }
            return obj;
        }

        private static FieldInfo GetFieldRecursively(System.Type type,string fieldName){
            var field = type.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if(field != null){
                return field;
            }
            if(type.BaseType != null){
                return GetFieldRecursively(type.BaseType,fieldName);
            }else{
                return null;
            }
        }

        private static object GetValue_Imp(object source, string name)
        {
            if (source == null)
                return null;
            var type = source.GetType();

            while (type != null)
            {
                var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (f != null)
                    return f.GetValue(source);

                var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (p != null)
                    return p.GetValue(source, null);

                type = type.BaseType;
            }
            return null;
        }

        private static object GetValue_Imp(object source, string name, int index)
        {
            var enumerable = GetValue_Imp(source, name) as System.Collections.IEnumerable;
            if (enumerable == null) return null;
            var enm = enumerable.GetEnumerator();
            //while (index-- >= 0)
            //    enm.MoveNext();
            //return enm.Current;

            for (int i = 0; i <= index; i++)
            {
                if (!enm.MoveNext()) return null;
            }
            return enm.Current;
        }
    }
}
