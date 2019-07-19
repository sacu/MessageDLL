using UnityEngine;
using System.Collections.Generic;
using Sacu.Events;
using Sacu.Factory.Worker;

namespace Sacu.Factory
{
    public class SABaseFactory
    {

        /**
         * 数据工人集合标识。
         */
        public const string DATA_WORKER = "DATA_WORKER";//工人组(数据)
                                                        /**
                                                         * 图形工人集合标识。
                                                         */
        public const string GRAPH_WORKER = "GRAPH_WORKER";//工人组(图形)
        protected bool _init;
        protected bool _start;
        protected SAEventDispatcher eventDispatcher;//事件域

        protected Dictionary<string, SAWorkers> workersTreeList;//工人名册
        protected Dictionary<string, GameObject> repeatObjects;//已存在图形

        protected List<SABaseGraphWorker> graphWorkers;//视图worker控制列表
        protected List<SABaseDataWorker> dataWorkers;//数据worker控制列表
        private string _name;//名称

        protected System.Object args;
        /**
         * 构造函数。
         * @param Name 工厂名称。
         */
        public SABaseFactory(string name)
        {
            _init = false;
            _name = name;
            eventDispatcher = new SAEventDispatcher();
            graphWorkers = new List<SABaseGraphWorker>();
            dataWorkers = new List<SABaseDataWorker>();
            repeatObjects = new Dictionary<string, GameObject>();
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
         * 启动工厂入口。
         * @param args 启动工厂可能需要到的参数集。
         */
        virtual public void startFactory(System.Object args)
        {
            this.args = args;
            if (!_init)
            {
                onInitFactory();
            }
            if (!_start)
            {
                onStartFactory();
            }
        }

        /**
         * 初始化工厂。
         */
        virtual protected void onInitFactory()
        {//初始工厂因子
            _init = true;
            workersTreeList = new Dictionary<string, SAWorkers>();
        }
        /**
         * 执行工厂。
         */
        virtual protected void onStartFactory()
        {//执行工厂因子
            changeWorkerState(true);
        }
        /**
         * 释放工厂。
         */
        virtual public void disposeFactory()
        {//放假工厂因子
            changeWorkerState(false);
        }

        private void changeWorkerState(bool state)
        {
            _start = state;
            int length = graphWorkers.Count;
            int i = 0;
            SABaseGraphWorker graph;
            for (; i < length; ++i)
            {
                graph = graphWorkers[i];
                if (_start) registerGraphicsWorker(graph);
                else removeGraphicsWorker(graph.getName());
            }
            workersTreeList[GRAPH_WORKER].callAllOnRegister();

            SABaseDataWorker data;
            length = dataWorkers.Count;
            for (i = 0; i < length; ++i)
            {
                data = dataWorkers[i];
                if (_start) registerDataWorker(data);
                else removeDataWorker(data.getName());

            }
        }

        /**
         * 注册一个数据工人到当前工厂。
         * @param worker 要注册到当前工厂的数据工人实例。
         * @return 注册成功则返回true，否则返回false。
         */
        public bool registerDataWorker(SABaseDataWorker worker)
        {//注册一个数据工人到工人组
            SAWorkers workers;
            if (!workersTreeList.ContainsKey(DATA_WORKER))
            {//如果数据工人不存在 则新建
                workers = new SAWorkers(DATA_WORKER, this);
                workersTreeList.Add(DATA_WORKER, workers);
            }
            workers = workersTreeList[DATA_WORKER];
            workers.registerWorker(worker);
            return true;
        }
        /**
         * 移除一个数据工人从当前工厂。
         * @param worker 要从当前工厂移除的数据工人名称。
         * @return 移除成功则返回true，否则返回false。
         */
        public bool removeDataWorker(string workerName)
        {
            if (workersTreeList.ContainsKey(DATA_WORKER))
            {
                SAWorkers workers = workersTreeList[DATA_WORKER];//得到数据工人组
                SABaseDataWorker worker = (SABaseDataWorker)workers.removeWorker(workerName);//移除数据工人组中的一名工人
                if (worker != null)
                {
                    worker.onRemove();
                    worker.unInitialize(this);
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        /**
         * 注册一个图形工人到当前工厂。
         * @param worker 要注册到当前工厂的图形工人实例。
         * @return 注册成功则返回true，否则返回false。
         */
        public void registerGraphicsWorker(SABaseGraphWorker worker)
        {
            SAWorkers workers;
            if (!workersTreeList.ContainsKey(GRAPH_WORKER))
            {
                workers = new SAWorkers(GRAPH_WORKER, this);
                workersTreeList.Add(GRAPH_WORKER, workers);
            }
            workers = workersTreeList[GRAPH_WORKER];
            workers.registerWorker(worker);//添加图形工人到图形工人组
            //worker.initialize(this);
            worker.onSrart(args);

            Dictionary<string, string>.Enumerator en = worker.handleActions.GetEnumerator();
            while (en.MoveNext())
            {
                addEventDispatcher(en.Current.Key, worker, "handleAction");
                //addEventDispatcher(en.Current.Key, worker, en.Current.Value);
            }
        }
        /**
         * 移除一个图形工人从当前工厂。
         * @param worker 要从当前工厂移除的图形工人名称。
         * @return 移除成功则返回true，否则返回false。
         */
        public bool removeGraphicsWorker(string workerName)
        {
            if (workersTreeList.ContainsKey(GRAPH_WORKER))
            {
                SAWorkers workers = workersTreeList[GRAPH_WORKER];
                SABaseGraphWorker worker = workers.removeWorker(workerName) as SABaseGraphWorker;//得到画板工人组
                if (worker != null)
                {
                    Dictionary<string, string>.Enumerator en = worker.handleActions.GetEnumerator();
                    while (en.MoveNext())
                    {
                        removeEventDispatcher(en.Current.Key, worker, "handleAction");
                        //removeEventDispatcher(en.Current.Key, worker, en.Current.Value);
                    }
                    worker.onRemove();
                    worker.onDispose();
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        /**
         * 添加一个需要关心消息类型到当前工厂。
         * @param type 消息类型。
         * @param listener 消息触发后执行的方法。
         */
        public void addEventDispatcher(string type, System.Object notifier, string listenerName)
        {
            eventDispatcher.addEventListener(type, notifier, listenerName);
        }
        /**
         * 移除一个以添加的消息类型从当前工厂。
         * @param type 消息类型。
         * @param listener 要移除的消息类型所触发的方法。
         */
        public void removeEventDispatcher(string type, System.Object notifier, string listenerName)
        {
            eventDispatcher.removeEventListener(type, notifier, listenerName);
        }
        /**
         * 从当前工厂触发一个消息到它所包含的所有工人。
         * @param type 消息类型。
         * @param body 消息携带数据，默认为null。
         * @param target 本次消息触发的起始目标，默认为null。
         */
        public void dispatchEvent(string type, System.Object body = null, System.Object target = null)
        {
            if (target == null)
            {
                target = this;
            }
            eventDispatcher.dispatchEvent(new SAFactoryEvent(type, body, target));
        }
    }
}