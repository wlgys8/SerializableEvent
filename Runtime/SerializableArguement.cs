
using UnityEngine;
namespace MS.Events{
    using MS.Reflection;
    using System.Collections.Generic;

    public enum ArguementSerializeType{
        String,
        Int,
        Float,
        Bool,
        Object,
        Vector2,
        Vector3,
        Vector4,
        Vector2Int,
        Vector3Int,
        Rect,
        RectInt,
        Color,
        Quaternion,
        Enum,
    }

    [System.Serializable]
    public class SerializableArguement:ISerializationCallbackReceiver{

        [SerializeField]
        private ArguementSerializeType _serializeType;

        /// <summary>
        /// When serializaType is string, it is used to store string value.
        /// When serializaType is Object and reference is NULL it is used to store Object's typeName.
        /// When serializaType is Enum, it is used to store Enum's typeName
        /// </summary>
        [SerializeField]
        private string _string;

        [SerializeField]
        private Object _unityObject;

        [SerializeField]
        private int _x,_y,_z,_w;

        private string _arguementName;

        private SerializableArguement(){
        }

        public SerializableArguement(string str){
            _serializeType = ArguementSerializeType.String;
            _string = str;
        }
        public SerializableArguement(int value){
            _serializeType = ArguementSerializeType.Int;
            this.intValue = value;
        }
        public SerializableArguement(float value){
            _serializeType = ArguementSerializeType.Float;
            this.floatValue = value;
        }   

        public SerializableArguement(bool value){
            _serializeType = ArguementSerializeType.Bool;
            this.boolValue = value;
        }

        public SerializableArguement(System.Type type){
            if(type.IsSubclassOf(typeof(UnityEngine.Object))){
                _serializeType = ArguementSerializeType.Object;
                _string = type.AssemblyQualifiedName;
            }else if(type.IsEnum){
                _serializeType = ArguementSerializeType.Enum;
                _string = type.AssemblyQualifiedName;
            }
            else{
                if(_typeToSerializeType.ContainsKey(type)){
                    _serializeType = _typeToSerializeType[type];
                    ArgTypeMeta argTypeMeta = GetTypeMeta(_serializeType);
                    object defaultValue = null;
                    if(type.IsValueType){
                        defaultValue = System.Activator.CreateInstance(type);
                    }
                    argTypeMeta.onAssignValue(this,defaultValue);
                }else{
                    throw new System.NotImplementedException("unsupport type:" + type);
                }
            }
        }

        public string arguementName{
            get{
                return _arguementName;
            }set{
                _arguementName = value;
            }
        }
        
        public ArguementSerializeType serializeType{
            get{
                return _serializeType;
            }
        }

        private void AssertSerializeType(ArguementSerializeType type){
            if(_serializeType != type){
                throw new System.InvalidOperationException($"This operation is only valid when ArguementSerializeType is {type}, current is {_serializeType}");
            }
        }

        public string stringValue{
            get{
                AssertSerializeType(ArguementSerializeType.String);
                return _string;
            }set{
                AssertSerializeType(ArguementSerializeType.String);
                _string = value;
            }
        }
 
        public int intValue{
            get{
                return _x;
            }set{
                AssertSerializeType(ArguementSerializeType.Int);
                _x = value;
            }
        }

        public float floatValue{
            get{
                return ConvertUtil.BitConvertIntToFloat(_x);
            }set{
                AssertSerializeType(ArguementSerializeType.Float);
                _x = ConvertUtil.BitConvertFloatToInt(value);
            }
        }

        public bool boolValue{
            get{
                return _x != 0;
            }set{
                AssertSerializeType(ArguementSerializeType.Bool);
                _x = value ? 1:0;
            }
        }

        public Vector2 vector2Value{
            get{
                return new Vector2(
                    ConvertUtil.BitConvertIntToFloat(_x),
                    ConvertUtil.BitConvertIntToFloat(_y));
            }set{
                AssertSerializeType(ArguementSerializeType.Vector2);
                _x = ConvertUtil.BitConvertFloatToInt(value.x);
                _y = ConvertUtil.BitConvertFloatToInt(value.y);
            }
        }
        public Vector3 vector3Value{
            get{
                return new Vector3(
                    ConvertUtil.BitConvertIntToFloat(_x),
                    ConvertUtil.BitConvertIntToFloat(_y),
                    ConvertUtil.BitConvertIntToFloat(_z));
            }set{
                AssertSerializeType(ArguementSerializeType.Vector3);
                _x = ConvertUtil.BitConvertFloatToInt(value.x);
                _y = ConvertUtil.BitConvertFloatToInt(value.y);
                _z = ConvertUtil.BitConvertFloatToInt(value.z);
            }
        }
        public Vector4 vector4Value{
            get{
                return new Vector4(
                    ConvertUtil.BitConvertIntToFloat(_x),
                    ConvertUtil.BitConvertIntToFloat(_y),
                    ConvertUtil.BitConvertIntToFloat(_z),
                    ConvertUtil.BitConvertIntToFloat(_w));
            }set{
                AssertSerializeType(ArguementSerializeType.Vector4);
                _x = ConvertUtil.BitConvertFloatToInt(value.x);
                _y = ConvertUtil.BitConvertFloatToInt(value.y);
                _z = ConvertUtil.BitConvertFloatToInt(value.z);
                _w = ConvertUtil.BitConvertFloatToInt(value.w);
            }
        }

        public Color colorValue{
            get{
                return new Color(
                    ConvertUtil.BitConvertIntToFloat(_x),
                    ConvertUtil.BitConvertIntToFloat(_y),
                    ConvertUtil.BitConvertIntToFloat(_z),
                    ConvertUtil.BitConvertIntToFloat(_w));
            }set{
                AssertSerializeType(ArguementSerializeType.Color);
                _x = ConvertUtil.BitConvertFloatToInt(value.r);
                _y = ConvertUtil.BitConvertFloatToInt(value.g);
                _z = ConvertUtil.BitConvertFloatToInt(value.b);
                _w = ConvertUtil.BitConvertFloatToInt(value.a);
            }
        }

        public Quaternion quaternionValue{
            get{
                return new Quaternion(
                    ConvertUtil.BitConvertIntToFloat(_x),
                    ConvertUtil.BitConvertIntToFloat(_y),
                    ConvertUtil.BitConvertIntToFloat(_z),
                    ConvertUtil.BitConvertIntToFloat(_w));
            }set{
                AssertSerializeType(ArguementSerializeType.Quaternion);
                _x = ConvertUtil.BitConvertFloatToInt(value.x);
                _y = ConvertUtil.BitConvertFloatToInt(value.y);
                _z = ConvertUtil.BitConvertFloatToInt(value.z);
                _w = ConvertUtil.BitConvertFloatToInt(value.w);
            }
        }
        

        public Vector2Int vector2IntValue{
            get{
                return new Vector2Int(_x,_y);
            }set{
                AssertSerializeType(ArguementSerializeType.Vector2Int);
                _x = value.x;
                _y = value.y;
            }
        }

        public Vector3Int vector3IntValue{
            get{
                return new Vector3Int(_x,_y,_z);
            }set{
                AssertSerializeType(ArguementSerializeType.Vector3Int);
                _x = value.x;
                _y = value.y;
                _z = value.z;
            }
        }

        public Rect rectValue{
            get{
                return new Rect(
                    ConvertUtil.BitConvertIntToFloat(_x),
                    ConvertUtil.BitConvertIntToFloat(_y),
                    ConvertUtil.BitConvertIntToFloat(_z),
                    ConvertUtil.BitConvertIntToFloat(_w));
            }set{
                AssertSerializeType(ArguementSerializeType.Rect);
                _x = ConvertUtil.BitConvertFloatToInt(value.x);
                _y = ConvertUtil.BitConvertFloatToInt(value.y);
                _z = ConvertUtil.BitConvertFloatToInt(value.width);
                _w = ConvertUtil.BitConvertFloatToInt(value.height);
            }
        }

        public RectInt rectIntValue{
            get{
                return new RectInt(_x,_y,_z,_w);
            }set{
                AssertSerializeType(ArguementSerializeType.RectInt);
                _x = value.x;
                _y = value.y;
                _z = value.width;
                _w = value.height;
            }
        }


        public Object unityObject{
            get{
                if(_unityObject == null){
                    return null;
                }else{
                    return _unityObject;
                }
            }set{
                AssertSerializeType(ArguementSerializeType.Object);
                _unityObject = value;
            }
        }

        public object enumValue{
            get{
                AssertSerializeType(ArguementSerializeType.Enum);
                return System.Enum.ToObject(this.type,_x);
            }set{
                AssertSerializeType(ArguementSerializeType.Enum);
                _x = (int)value;
            }
        }

        public object objectValue{
            get{
                if(_serializeType == ArguementSerializeType.Object){
                    return _unityObject;
                }else if(_serializeType == ArguementSerializeType.Enum){
                    return enumValue;
                }else{
                    var meta = GetTypeMeta(_serializeType);
                    return meta.onGetValue(this);
                }
            }
        }

        public System.Type type{
            get{
                if(_serializeType == ArguementSerializeType.Object){
                    return TypeCaches.GetType(_string);
                }else if(_serializeType == ArguementSerializeType.Enum){
                    return TypeCaches.GetType(_string);
                }
                else{
                    var meta = GetTypeMeta(_serializeType);
                    return meta.type;
                }
            }
        }


        public void OnAfterDeserialize()
        {

        }

        public void OnBeforeSerialize()
        {
 
        }


        internal class ArgTypeMeta{
            public ArguementSerializeType serializeType;
            public System.Type type;

            public System.Action<SerializableArguement,object> onAssignValue;

            public System.Func<SerializableArguement,object> onGetValue;
        }


        private static List<ArgTypeMeta> _typeMetas = new List<ArgTypeMeta>(){
            new ArgTypeMeta(){
                serializeType = ArguementSerializeType.Float,
                type = typeof(float),
                onAssignValue = (arg,value)=>{
                    arg.floatValue = (float)value;
                },
                onGetValue = (arg)=>{
                    return arg.floatValue;
                },
            },
            new ArgTypeMeta(){
                serializeType = ArguementSerializeType.Int,
                type = typeof(int),
                onAssignValue = (arg,value)=>{
                    arg.intValue = (int)value;
                },
                onGetValue = (arg)=>{
                    return arg.intValue;
                },
            },
            new ArgTypeMeta(){
                serializeType = ArguementSerializeType.String,
                type = typeof(string),
                onAssignValue = (arg,value)=>{
                    arg.stringValue = (string)value;
                },
                onGetValue = (arg)=>{
                    return arg.stringValue;
                },
            },
            new ArgTypeMeta(){
                serializeType = ArguementSerializeType.Bool,
                type = typeof(bool),
                onAssignValue = (arg,value)=>{
                    arg.boolValue = (bool)value;
                },
                onGetValue = (arg)=>{
                    return arg.boolValue;
                },
            },
            new ArgTypeMeta(){
                serializeType = ArguementSerializeType.Vector2,
                type = typeof(Vector2),
                onAssignValue = (arg,value)=>{
                    arg.vector2Value = (Vector2)value;
                },
                onGetValue = (arg)=>{
                    return arg.vector2Value;
                },
            },
            new ArgTypeMeta(){
                serializeType = ArguementSerializeType.Vector3,
                type = typeof(Vector3),
                onAssignValue = (arg,value)=>{
                    arg.vector3Value = (Vector3)value;
                },
                onGetValue = (arg)=>{
                    return arg.vector3Value;
                },
            },
            new ArgTypeMeta(){
                serializeType = ArguementSerializeType.Vector4,
                type = typeof(Vector4),
                onAssignValue = (arg,value)=>{
                    arg.vector3Value = (Vector4)value;
                },
                onGetValue = (arg)=>{
                    return arg.vector4Value;
                },
            },

            new ArgTypeMeta(){
                serializeType = ArguementSerializeType.Vector2Int,
                type = typeof(Vector2Int),
                onAssignValue = (arg,value)=>{
                    arg.vector2IntValue = (Vector2Int)value;
                },
                onGetValue = (arg)=>{
                    return arg.vector2IntValue;
                },
            },

            new ArgTypeMeta(){
                serializeType = ArguementSerializeType.Vector3Int,
                type = typeof(Vector3Int),
                onAssignValue = (arg,value)=>{
                    arg.vector3IntValue = (Vector3Int)value;
                },
                onGetValue = (arg)=>{
                    return arg.vector3IntValue;
                },
            },

            new ArgTypeMeta(){
                serializeType = ArguementSerializeType.Rect,
                type = typeof(Rect),
                onAssignValue = (arg,value)=>{
                    arg.rectValue = (Rect)value;
                },
                onGetValue = (arg)=>{
                    return arg.rectValue;
                },
            },

            new ArgTypeMeta(){
                serializeType = ArguementSerializeType.RectInt,
                type = typeof(RectInt),
                onAssignValue = (arg,value)=>{
                    arg.rectIntValue = (RectInt)value;
                },
                onGetValue = (arg)=>{
                    return arg.rectIntValue;
                },
            },

            new ArgTypeMeta(){
                serializeType = ArguementSerializeType.Color,
                type = typeof(Color),
                onAssignValue = (arg,value)=>{
                    arg.colorValue = (Color)value;
                },
                onGetValue = (arg)=>{
                    return arg.colorValue;
                },
            },


            new ArgTypeMeta(){
                serializeType = ArguementSerializeType.Quaternion,
                type = typeof(Quaternion),
                onAssignValue = (arg,value)=>{
                    arg.quaternionValue = (Quaternion)value;
                },
                onGetValue = (arg)=>{
                    return arg.quaternionValue;
                },
            },

        };

        private static Dictionary<System.Type,ArguementSerializeType> _typeToSerializeType;
        private static Dictionary<ArguementSerializeType,ArgTypeMeta> _serializeTypeToMeta;


        static SerializableArguement(){
            _typeToSerializeType = new Dictionary<System.Type, ArguementSerializeType>();
            _serializeTypeToMeta = new Dictionary<ArguementSerializeType, ArgTypeMeta>();
            foreach(var meta in _typeMetas){
                _typeToSerializeType.Add(meta.type,meta.serializeType);
                _serializeTypeToMeta.Add(meta.serializeType,meta);
            }
        }

        private static ArgTypeMeta GetTypeMeta(ArguementSerializeType serializeType){
            ArgTypeMeta result = null;
            _serializeTypeToMeta.TryGetValue(serializeType,out result);
            return result;
        }


        public static bool CheckSupport(System.Type type){
            if(type.IsSubclassOf(typeof(UnityEngine.Object))){
                return true;
            }
            if(type.IsEnum){
                return true;
            }
            return _typeToSerializeType.ContainsKey(type);
        }


    }
}
