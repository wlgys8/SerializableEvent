
namespace MS.Events{
    using System.Runtime.InteropServices;
    public static class ConvertUtil
    {
        [StructLayout(LayoutKind.Explicit)]
        struct UIntFloat
        {       
            [FieldOffset(0)]
            public float FloatValue;

            [FieldOffset(0)]
            public int IntValue;        
        }


        public static int BitConvertFloatToInt(float value){
            var intfloat = new UIntFloat();
            intfloat.FloatValue = value;
            return intfloat.IntValue;
        }
        public static float BitConvertIntToFloat(int value){
            var intfloat = new UIntFloat();
            intfloat.IntValue = value;
            return intfloat.FloatValue;
        }
        
        public static int ConvertToInt32(string str){
            int result = 0;
            var flag = 1;
            var index = 0;
            if(str[0] == '-'){
                flag = -1;
                index ++;
            }
            for (int i = index; i < str.Length; i++){
                var c = str[i];
                var num = c - '0';
                if(num <0 || num > 9){
                    throw new System.FormatException("ConvertToInt32 failed:" + str);
                }
                result = result * 10 + num;
            }
            return result * flag;
        }


        public static float ConvertToFloat(string str){
            float intValue = 0;
            var flag = 1;
            var index = 0;
            if(str[0] == '-'){
                flag = -1;
                index ++;
            }
            int pointIndex = str.Length;
            float decimalValue = 0;
            for (int i = index; i < str.Length; i++){
                var c = str[i];
                if(c == '.'){
                    pointIndex = i;
                    break;
                }
                var num = c - '0';
                if(num <0 || num > 9){
                    throw new System.FormatException("ConvertToFloat failed:" + str);
                }
                intValue = intValue * 10 + num;
            }

            for(var i = str.Length - 1;i > pointIndex;i--){
                var c = str[i];
                var num = c - '0';
                if(num <0 || num > 9){
                    throw new System.FormatException("ConvertToFloat failed:" + str);
                }
                decimalValue = (decimalValue + num) * 0.1f;
            }
            return (intValue + decimalValue) * flag;   
        }
    }
}
