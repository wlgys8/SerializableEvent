using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MS.Events;

namespace MS.Events{


    public abstract class EnumableEventBase {}

    public class EnumableEvent<TEnum,TEvent> :EnumableEventBase,ISerializationCallbackReceiver where TEnum:System.Enum where TEvent:SerializableEventBase,new()
    {

        [SerializeField]
        private List<TEnum> _enums;

        [SerializeField]
        private List<TEvent> _events;

        private Dictionary<TEnum,TEvent> _eventsDict = new Dictionary<TEnum, TEvent>();

        public void OnAfterDeserialize()
        {
            _eventsDict.Clear();
            if(_enums != null && _events != null){
                if(_enums.Count == _events.Count){
                    for(var i = 0; i < _enums.Count;i ++){
                        var key = _enums[i];
                        var evt = _events[i];
                        if(_eventsDict.ContainsKey(key)){
                            Debug.LogWarning("Duplicate key:" + key);
                        }else{
                            _eventsDict.Add(key,evt);
                        }
                    }
                }
            }
        }


        public TEvent GetEvent(TEnum enumValue){
            TEvent evt = default(TEvent);
            _eventsDict.TryGetValue(enumValue,out evt);
            return evt;
        }

        public void Add(TEnum enumValue){
            if(_eventsDict.ContainsKey(enumValue)){
                throw new System.InvalidOperationException($"{enumValue} already exists");
            }
            var evt = new TEvent();
            _eventsDict.Add(enumValue,evt);
            #if UNITY_EDITOR
            if(_enums == null){
                _enums = new List<TEnum>();
            }
            if(_events == null){
                _events = new List<TEvent>();
            }
            _enums.Add(enumValue);
            _events.Add(evt);
            #endif
        }

        public bool Remove(TEnum enumValue){
            TEvent evt = default(TEvent);
            if(_eventsDict.TryGetValue(enumValue,out evt)){
                _eventsDict.Remove(enumValue);
                #if UNITY_EDITOR
                if(_enums != null){
                    _enums.Remove(enumValue);
                }
                if(_events != null){
                    _events.Remove(evt);
                }
                #endif
                return true;
            }
            return false;
        }

        public void OnBeforeSerialize()
        {
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class EnumableEventStyleAttribute:System.Attribute{

        public EnumableEventStyleAttribute(){
            this.drawBackground = true;
            this.drawHeader = true;
        }
        public bool drawBackground{
            get;set;
        }

        public bool drawHeader{
            get;set;
        }
        
    }
}
