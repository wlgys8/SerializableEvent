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
- Support distinguish target component when there are mulpitle identical components in one GameObject.
    
![](https://raw.githubusercontent.com/wiki/wlgys8/SerializableEvent/.images/img2.jpeg)

# Dependencies

[FastReflection - .Net fast reflection for Unity](https://github.com/wlgys8/FastReflection)

# Install

add packages in \<Project>/Packages/manifest.json

```json

"com.ms.fastreflection":"https://github.com/wlgys8/FastReflection.git",
"com.ms.serializable.event":"https://github.com/wlgys8/SerializableEvent.git"

```

# Usage

[Visit WIKI](https://github.com/wlgys8/SerializableEvent/wiki)

# Benchmark

add one persistent void call to UnityEvent & SerializableEvent and call invoke 10000 times.



 -- |SerializableEvent|UnityEvent
---|---|---
Cost|1.394ms|1.662ms


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

