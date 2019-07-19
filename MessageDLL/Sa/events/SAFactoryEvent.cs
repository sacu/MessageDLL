using System.Collections;
using System;

namespace Sacu.Events{
    public class SAFactoryEvent : SAEvent {
    
        private Object _body;
	    public Object _target;
        public SAFactoryEvent(string type, Object body = null, Object target = null)
            : base(type){
		    _body = body;
		    _target = target;

	    }

        public Object Body{
            get{
                return _body;
            }
        }
    }
}
