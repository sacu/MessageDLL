using UnityEngine;
using System.Collections;

namespace Sacu.Events
{
    public class SAEvent
    {
        protected string type;
        protected string luaType;
        public SAEvent(string value)
        {
            type = value;
            luaType = type + ".Lua";
        }

        public string Type
        {
            get
            {
                return type;
            }
        }
        public string LuaType
        {
            get
            {
                return luaType;
            }
        }
    }
}