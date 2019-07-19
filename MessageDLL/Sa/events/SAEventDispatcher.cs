using System.Collections.Generic;
using System.Collections;
using System;
using System.Reflection;

namespace Sacu.Events
{
    public class SAEventDispatcher
    {
        private Dictionary<string, Dictionary<string, ArrayList>> events;

        public SAEventDispatcher()
        {
            events = new Dictionary<string, Dictionary<string, ArrayList>>();
        }

        /**
         * 添加事件监听
         * 类型、对象、方法名
         */
        public void addEventListener(string type, Object notifier, string functionName)
        {
            Dictionary<string, ArrayList> funs;
            if (!events.ContainsKey(type))
            {//判断监听类型
                funs = new Dictionary<string, ArrayList>();
                events.Add(type, funs);
            }
            funs = events[type];
            ArrayList list;
            if (!funs.ContainsKey(functionName))
            {//判断是否有方法名
                list = new ArrayList();
                funs.Add(functionName, list);
            }
            list = funs[functionName];//获取该方法的所有类
            if (!list.Contains(notifier))
            {//如果该类没有添加该方法，则添加
                list.Add(notifier);
            }
        }

        /**
         * 移除事件监听
         */
        public void removeEventListener(string type, Object notifier, string functionName)
        {
            Dictionary<string, ArrayList> funs;
            if (events.ContainsKey(type))
            {//如果类型存在
                funs = events[type];//获取到方法名列表
                if (funs.ContainsKey(functionName))
                {
                    ArrayList list = funs[functionName];
                    list.Remove(notifier);
                }
            }
        }
        /**
         * 消息广播
         */
        public void dispatchEvent(SAEvent notifier, Object target = null)
        {
            _dispatchEvent(notifier, target, notifier.Type);
            _dispatchEvent(notifier, target, notifier.LuaType);
        }
        private void _dispatchEvent(SAEvent notifier, Object target = null, string type = "")
        {
            Dictionary<string, ArrayList> funs;
            if (events.ContainsKey(type))
            {
                funs = events[type];
                Dictionary<string, ArrayList>.Enumerator en = funs.GetEnumerator();
                while (en.MoveNext())
                {
                    ArrayList list = en.Current.Value;
                    string functionName = en.Current.Key;
                    int length = list.Count;
                    for (int i = 0; i < length; ++i)
                    {
                        if (target == null || list[i] == target)
                        {
                            Type t = list[i].GetType();
                            MethodInfo methodInfo = t.GetMethod(functionName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                            if (methodInfo != null)
                            {
                                ParameterInfo[] paramsInfo = methodInfo.GetParameters();//得到指定方法的参数列表
                                if (paramsInfo.Length == 1)
                                {//参数数量
                                    if (notifier.GetType().Equals(paramsInfo[0].ParameterType))
                                    {
                                        try
                                        {
                                            methodInfo.Invoke(list[i], new Object[] { notifier });
                                        } catch (Exception ex)
                                        {
                                            UnityEngine.Debug.LogError(ex.Message);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        /**
         * 消息监听查询
         */
        public bool hasEventListener(string type)
        {
            return events.ContainsKey(type);
        }
    }
}