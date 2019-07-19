using System;
using System.Collections.Generic;
namespace Sacu.Factory.Worker
{
    public class SABaseDataWorker : ISAWorker
    {
        private Dictionary<string, SABaseFactory> factoryTreeList;
        private int _state;//0:未读，1:正在读取,2:读取成功，-1：读取失败
        private Object _data;
        private string _command;
        private string _name;
        /**
        * 构造函数
        * @param name 工人名称。
        * @param command HTTPInterface的调用命令。
        */
        public SABaseDataWorker(string name, string command)
        {
            _command = command;
            _name = name;
            factoryTreeList = new Dictionary<string, SABaseFactory>();
            _state = 0;
        }
        public string getName()
        {
            return _name;
        }

        public void setName(string value)
        {
            _name = value;
        }
        /**
        * 当该工人被任职到某个工厂时触发。
        */
        virtual public void onRegister()
        {
        }
        /**
        * 当该工人从某个工厂被解雇时触发。
        */
        virtual public void onRemove()
        {
        }
        virtual public void onRegisterComplete()
        {
        }
        /**
        * 将该工人加入到指定的工厂。
        * @param factory 该工人要加入到的工厂实例。
        */
        public void initialize(SABaseFactory factory)
        {
            if (!factoryTreeList.ContainsKey(factory.getName()))
            {
                factoryTreeList.Add(factory.getName(), factory);
            }
        }
        /**
	    * 将该工人从指定的工厂中移除。
	    * @param factory 指定该工人要移除的工厂实例。
	    */
        public void unInitialize(SABaseFactory factory)
        {
            factoryTreeList.Remove(factory.getName());
        }
        /**
	    * 从该工人触发一个消息到它所任职的所有工厂。
	    * @param type 消息类型。
	    * @param body 消息附带的值，默认为null。
	    */
        public void dispatchEvent(string type, Object body = null)
        {
            SABaseFactory _factory;
            Dictionary<string, SABaseFactory>.Enumerator en = factoryTreeList.GetEnumerator();
            while (en.MoveNext())
            {
                _factory = en.Current.Value;
                _factory.dispatchEvent(type, body);
            }
        }
        /**
        * 数据工人特有的数据存储数据。
        */
        public Object data
        {
            get
            {
                return _data;
            }
            set
            {
                _state = 2;
                _data = value;
            }
        }
        /**
        * HTTPInterface数据请求成功后的调用方法。
        * @param result HTTPInterface数据请求成功后的返回值。
        */
        public void onloadSucess(Object result)
        {
            if (_state == 2)
            {
                return;
            }
            _state = 2;
            _data = result;
            update();
        }
        /**
	    * HTTPInterface数据请求失败后的调用方法。
	    * @param result HTTPInterface数据请求失败后的返回值。
	    */
        public void onloadFault(Object result)
        {
            _state = -1;
        }
        /**
        * 触发一次更新广播对该工人所任职的所有工厂，消息附带值为数据工人自身存储数据。
        * @param str 广播以在HTTPInterface的调用命令后加str后缀的命令消息。
        */
        public void update(string str = "")
        {
            dispatchEvent(_command + str, _data);
        }

        /**
        * 获取当前工人对HTTPInterface的调用命令。
        */
        public string Command
        {
            get
            {
                return _command;
            }
        }
        /**
        * 获取当前工人的数据状态，0:未读，1:正在读取,2:读取成功，-1：读取失败。
        */
        public int State
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;

            }
        }
    }
}
