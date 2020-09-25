using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace MS.Events.Editor{
    using System.Reflection;

    [CustomPropertyDrawer(typeof(SerializableCallback))]
    internal class SerializableCallbackEditor : PropertyDrawer
    {
        
        private const float SPACE_OF_ARGUEMENTS = 3;
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label){
            var arguements = property.FindPropertyRelative("_arguements");
            var arguementMode = property.FindPropertyRelative("_arguementMode");
            var argMode = GetArguementMode(arguementMode.enumValueIndex);

            var height = 18f; // first column
            height += 18; //second column
            if(argMode == ArguementMode.Static){
                for(var i = 0 ; i < arguements.arraySize;i++){
                    var arg = arguements.GetArrayElementAtIndex(i);
                    height += (EditorGUI.GetPropertyHeight(arg) + SPACE_OF_ARGUEMENTS);
                }
            }
            return height;
        }


        private void DrawMethod(Rect rect, SerializedProperty property){
            var callback = EditorHelper.GetTargetObjectOfProperty(property) as SerializableCallback;
            if(callback.UpdateEditorArguementNamesIfRequired()){
                // property.serializedObject.UpdateIfRequiredOrScript();
            }
            var target = property.FindPropertyRelative("_target");
            var funcName = property.FindPropertyRelative("_funcName").stringValue;
            string displayFuncName = funcName;
            if(!target.objectReferenceValue || string.IsNullOrEmpty(funcName)){
                displayFuncName = "No Function";
            }else if(!callback.Validate()){
                displayFuncName = $"Missing<{target.objectReferenceValue.GetType().Name}.{funcName}>";
            }else{
                var methodInfo = callback.GetMethodInfo();
                var validateResult = ValidateMethod(methodInfo);
                if(validateResult == MethodInvalidReason.None){
                    displayFuncName = GetFormatedMethodName(target.objectReferenceValue,funcName);
                }else{
                    displayFuncName = $"<{target.objectReferenceValue.GetType().Name}.{funcName}> invalid with reason: {validateResult}";
                }
            }
            EditorGUI.BeginDisabledGroup(target.objectReferenceValue == null);
            if(GUI.Button(rect, displayFuncName, EditorStyles.popup)){
                ShowCallbackSelector(target.objectReferenceValue,property);
            }
            EditorGUI.EndDisabledGroup();
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label){
            var callback = EditorHelper.GetTargetObjectOfProperty(property) as SerializableCallback;
            var target = property.FindPropertyRelative("_target");
            var funcName = property.FindPropertyRelative("_funcName").stringValue;
            var callState = property.FindPropertyRelative("_callState");
            var arguementMode = property.FindPropertyRelative("_arguementMode");

            var height = 18f;

            var y = rect.y;

            //draw callState
            
            var callStateRect = new Rect(rect.x,y,rect.width * 0.3f,height);
            EditorGUI.PropertyField (callStateRect, callState,GUIContent.none);


            //draw func
            var funcRect = new Rect(callStateRect.xMax,y,rect.width * 0.7f,height);
            this.DrawMethod(funcRect,property);
  

            //draw source

            y += height;
            var sourceRect = new Rect(rect.x,y,rect.width * 0.3f,height);
            var identicalComponentIndex = CalculateIdenticalComponentIndex(target.objectReferenceValue);
            if(identicalComponentIndex >= 0){
                sourceRect.width -= 20;
            }
            EditorGUI.PropertyField (sourceRect, target,GUIContent.none);
            if(identicalComponentIndex >=0){
                EditorGUI.LabelField(new Rect(sourceRect.xMax,sourceRect.y,20,sourceRect.height),$"[{identicalComponentIndex}]");
                sourceRect.width += 20;
            }

            var dynamicArguementSupport = IsDynamicArguementSupport(property);
            var dynamic = GetArguementMode(arguementMode.enumValueIndex) == ArguementMode.Dynamic;
            //draw arguementMode
            EditorGUI.BeginDisabledGroup(!dynamicArguementSupport);
            var argModeRect = new Rect(sourceRect.xMax,y,rect.width * 0.7f,height);
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(argModeRect,arguementMode,GUIContent.none);
            if(EditorGUI.EndChangeCheck()){
                dynamic = GetArguementMode(arguementMode.enumValueIndex) == ArguementMode.Dynamic;
                arguementMode.serializedObject.ApplyModifiedProperties();
            }
            EditorGUI.EndDisabledGroup();

            if(!dynamicArguementSupport && dynamic){
                //Fix the ArguementMode to static
                arguementMode.enumValueIndex = GetEnumValueIndex(ArguementMode.Static);
                dynamic = false;
            }
            
            if(!dynamic){
                y += height;
                //draw arguements
                if(target.objectReferenceValue != null){
                    DrawArguements(new Rect(rect.x,y,rect.width,rect.height - y),property);
                }
            }
        }

        private bool IsDynamicArguementSupport(SerializedProperty property){
            var callback = EditorHelper.GetTargetObjectOfProperty(property) as SerializableCallback; 
            if(callback.target == null){
                return false;
            }
            if(string.IsNullOrEmpty(callback.funcName)){
                return false;
            }
            var propertyInfo = callback.GetType().GetProperty("constrainedDynamicArguementTypes",BindingFlags.Instance|BindingFlags.NonPublic);
            var constrainedDynamicArguementTypes = (System.Type[])propertyInfo.GetValue(callback);
            var callbackArguementTypes = callback.arguementTypes;
            if(callbackArguementTypes.Length == 0){
                return false;
            }
            if(constrainedDynamicArguementTypes == null){
                return false;
            }
            if(callbackArguementTypes.Length != constrainedDynamicArguementTypes.Length){
                return false;
            }
            for(var i = 0; i < callbackArguementTypes.Length;i++){
                if(callbackArguementTypes[i] != constrainedDynamicArguementTypes[i]){
                    return false;
                }
            }
            return true;
        }

        private static ArguementMode GetArguementMode(int enumValueIndex){
            var values = System.Enum.GetValues(typeof(ArguementMode));
            if(enumValueIndex < 0 || enumValueIndex >= values.Length){
                return ArguementMode.Static;
            }
            return (ArguementMode)values.GetValue(enumValueIndex);
        }

        private static int GetEnumValueIndex(ArguementMode mode){
            var values = System.Enum.GetValues(typeof(ArguementMode));
            return System.Array.IndexOf(values,mode);
        }

        private void DrawArguements(Rect rect, SerializedProperty callProperty){
            var argsList = callProperty.FindPropertyRelative("_arguements");
            var callback = EditorHelper.GetTargetObjectOfProperty(callProperty) as SerializableCallback;

            EditorGUI.BeginChangeCheck();
            var y = rect.y;
            for(var i = 0 ; i < argsList.arraySize;i++){
                var arg = argsList.GetArrayElementAtIndex(i);
                var height = EditorGUI.GetPropertyHeight(arg);
                EditorGUI.PropertyField(new Rect(rect.x,y,rect.width,height),arg,true);
                y += (height + SPACE_OF_ARGUEMENTS);
            }
            if(EditorGUI.EndChangeCheck()){
                callback.SetCachedValuesDirty();
            }
        }

        private static void ShowCallbackSelector(Object target,SerializedProperty property){
            var validMethods = GetValidMethodList(target);
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("No Function"),false,OnFunctionSelected,new FunctionSelectContext(){
                target = target,
                methodInfo = null,
                property = property
            });
            menu.AddSeparator("");
            foreach(var group in validMethods){
                foreach(var m in group.methods){
                    var context = new FunctionSelectContext(){
                        target = group.target,
                        methodInfo = m,
                        property = property
                    };
                    string itemName = GetMethodItemName(group.target,m,group.identicalComponentIndex);
                    menu.AddItem(new GUIContent(itemName),false,OnFunctionSelected,context);
                }
            }
            menu.ShowAsContext();
        }


        private static System.Type[] GetTypesFromGameObject(GameObject go){
            var components = go.GetComponents<Component>();
            return components.Select((c)=>{
                return c.GetType();
            }).Distinct().ToArray();
        }

        private static void OnFunctionSelected(object userData){
            var context = userData as FunctionSelectContext;
            var methodInfo = context.methodInfo;
            var property = context.property;
            var callback = EditorHelper.GetTargetObjectOfProperty(property) as SerializableCallback;
            if(methodInfo == null){
                callback.Reset(context.target,null);
            }else{
                var parameters = methodInfo.GetParameters();
                string funcName = methodInfo.Name;
                callback.Reset(context.target,methodInfo);
                property.serializedObject.UpdateIfRequiredOrScript();
            }
        }


        private static List<ValidMethods> GetValidMethodList(Object target){
            var srcTarget = target;
            if(target is Component component){
                target = component.gameObject;
            }
            var result = new List<ValidMethods>();
            if(target is GameObject go){

                {
                    var methods = GetValidMethodsFromType(go.GetType());
                    var validMethods = new ValidMethods(){
                        targetType = go.GetType(),
                        methods = methods,
                        target = go,
                    };
                    result.Add(validMethods);
                }

                var components = go.GetComponents<Component>();
                var identicalComponentsDict = new Dictionary<System.Type,List<ValidMethods>>();
                foreach(var comp in components){
                    var methods = GetValidMethodsFromType(comp.GetType());
                    var targetType = comp.GetType();
                    var validMethods = new ValidMethods(){
                        targetType = targetType,
                        methods = methods,
                        target = comp,
                    };
                    if(!identicalComponentsDict.ContainsKey(targetType)){
                        identicalComponentsDict[targetType] = new List<ValidMethods>();
                    }
                    identicalComponentsDict[targetType].Add(validMethods);
                    result.Add(validMethods);
                }

                foreach(var kv in identicalComponentsDict){
                    if(kv.Value.Count > 1){
                        var index = 0;
                        foreach(var validMethods in kv.Value){
                            validMethods.identicalComponentIndex = index;
                            index++;
                        }
                    }
                }
                return result;
            }
            return new List<ValidMethods>();
        }

        private static MethodInfo[] GetValidMethodsFromType(System.Type type){
            var methods = type.GetMethods(BindingFlags.Public|BindingFlags.Instance);
            return methods.Where((m)=>{
                return ValidateMethod(m) == MethodInvalidReason.None;
            }).ToArray();
        }



        private static MethodInvalidReason ValidateMethod(MethodInfo m){
            if(m.IsGenericMethod){
                return MethodInvalidReason.GenericMethodNotSupport;
            }
            if(m.GetCustomAttribute<System.ObsoleteAttribute>() != null){
                return MethodInvalidReason.Obsolete;
            }
            if(m.Name.StartsWith("get_")){
                return MethodInvalidReason.PropertyGetNotSupport;
            }
            var parameters = m.GetParameters();
            if(parameters.Length > 4){
                return MethodInvalidReason.TooManyArguements;
            }
            System.Type[] types = new System.Type[parameters.Length];
            var idx = 0;
            foreach(var p in parameters){
                if(p.IsOut){
                    return MethodInvalidReason.RefArguementNotSupport;
                }
                if(!SerializableArguement.CheckSupport(p.ParameterType)){
                    return MethodInvalidReason.ArguementNotSerializable;
                }
                types[idx ++] = p.ParameterType;
            }
            if(!SerializableCallback.CheckMethodWithArguementSupportInAOT(types)){
                return MethodInvalidReason.AOTEnsureRequired;
            }
            return MethodInvalidReason.None;
        }


        private static string GetFormatedMethodName(Object target, string funcName){
            if(funcName.StartsWith("set_")){
                funcName = funcName.Substring(4);
            }
            return target.GetType().Name + "." + funcName;
        }


        private static string GetMethodItemName(Object target, MethodInfo methodInfo,int identicalComponentIndex = -1){
            var typeName = target.GetType().Name;
            if(identicalComponentIndex >= 0){
                typeName += ("[" + identicalComponentIndex + "]");
            }
            string displayFuncName = methodInfo.Name;
            var ps = methodInfo.GetParameters();
            if(methodInfo.IsSpecialName && displayFuncName.StartsWith("set_") && ps.Length == 1){
                displayFuncName = ps[0].ParameterType.Name + " " +  displayFuncName.Remove(0,4);
                string result = typeName + "/" + displayFuncName;
                return result;
            }else{
                string result = typeName + "/" + displayFuncName + "(";
                var index = 0;
                foreach(var p in ps){
                    if(index < ps.Length - 1){
                        result += (p.ParameterType.Name + ",");
                    }else{
                        result += p.ParameterType.Name;
                    }
                    index ++;
                }
                result += ")";
                return result;
            }
        }

        private static int CalculateIdenticalComponentIndex(Object target){
            if(target == null){
                return -1;
            }
            if(target is Component comp){
                var go = comp.gameObject;
                var components = go.GetComponents<Component>();
                var index = 0;
                var totalIdenticalTypeCount = 0;
                foreach(var c in components){
                    if(c.GetType() == target.GetType()){
                        if(c == target){
                            index = totalIdenticalTypeCount;
                        }
                        totalIdenticalTypeCount ++;
                    }
                }
                if(totalIdenticalTypeCount > 1){
                    return index;
                }else{
                    return -1;
                }
            }

            return -1;
        }


        public class ValidMethods{

            public Object target;
            public System.Type targetType;
            public MethodInfo[] methods;
            public int identicalComponentIndex = -1;
        }

        public class FunctionSelectContext{
            public Object target;
            public SerializedProperty property;
            public MethodInfo methodInfo;
        }


        public enum MethodInvalidReason{
            None,
            GenericMethodNotSupport,
            PropertyGetNotSupport,
            ReturnTypeNotSupport,
            TooManyArguements,

            RefArguementNotSupport,
            ArguementNotSerializable,
            AOTEnsureRequired,
            Obsolete
            
        }

    }
}
