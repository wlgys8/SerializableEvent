using System.Collections.Generic;
using UnityEngine;

namespace  MS.Events
{
    using System.Reflection;

    public class ArguementsCache{
        
        private List<SerializableArguement> _arguements;
        private object[] _values;
        private System.Type[] _types;

        public ArguementsCache(List<SerializableArguement> arguements){
            _arguements = arguements;
        }

        public int count{
            get{
                if(_arguements == null){
                    return 0;
                }
                return _arguements.Count;
            }
        }
        
        public System.Type[] types{
            get{
                if(_types != null){
                    return _types;
                }
                if(_arguements == null){
                    _types = new System.Type[]{};
                }else{
                    _types = new System.Type[_arguements.Count];
                    for(var i = 0; i < _arguements.Count;i++){
                        var arg = _arguements[i];
                        _types[i] = arg.type;
                    }
                }
                return _types;
            }
        }

        public object[] values{
            get{
                if(_values == null){
                    if(_arguements == null){
                        _values = new object[]{};
                    }else{
                        _values = new object[_arguements.Count];
                        for(var i = 0; i < _arguements.Count;i++){
                            var arg = _arguements[i];
                            _values[i] = arg.objectValue;
                        }
                    }
                }
                return _values;
            }
        }

        public void SetValuesDirty(){
            _values = null;
        }

    }

}

