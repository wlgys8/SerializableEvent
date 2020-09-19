# SerializableEvent

Similar to `UnityEvent`, but with more features :

- Support callbacks with multiple arguements 
- Support more arguement types as follow:
    - String
    - Int
    - Float
    - Bool
    - Object
    - Vector2
    - Vector3
    - Vector4
    - Vector2Int
    - Vector3Int
    - Rect
    - RectInt
    - Color
    - Quaternion
    - Enum
    
![](https://raw.githubusercontent.com/wiki/wlgys8/SerializableEvent/.images/img2.jpeg)

# Usage

[Visit WIKI](https://github.com/wlgys8/SerializableEvent/wiki)

# Benchmark

TODO

# Extend Callback Arguements Support
In consideration of IL2Cpp/AOT, you can only choose callbacks with ONE arguement or multiple arguements without ValueType by default.

To extend the callback support, you must write follows in you `RUNTIME Code`.


```csharp

//Only run in Editor, however declaration is still need for runtime.
#if UNITY_EDITOR 
[UnityEditor.InitializeOnLoadMethod] 
#endif
public static void RegisterArguementsSupport(){
    //support callbacks with 2 arguements, that are bool & string
    CachedCallArguementsTypeRegister.EnsureCall<bool,string>();
}

```


