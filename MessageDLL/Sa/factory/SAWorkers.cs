
using System.Collections.Generic;
using Sacu.Core;
using Sacu.Factory.Worker;
namespace Sacu.Factory
{
    public class SAWorkers : SAUnDisplayElement
    {

        private Dictionary<string, ISAWorker> workerTreeList;
        private SABaseFactory _factory;
        /**
         * 构造函数。
         * @param name 工人集合名称。
         * @param factory 所管控的工厂的实例。
         */
        public SAWorkers(string name, SABaseFactory factory)
            : base(name)
        {
            workerTreeList = new Dictionary<string, ISAWorker>();
            _factory = factory;
        }
        /**
         * 注册一个所管控工厂中的工人到该集合。
         * @param woker 要注册到该集合的工人实例。
         */
        public void registerWorker(ISAWorker woker)
        {
            if (!workerTreeList.ContainsKey(woker.getName()))
            {
                workerTreeList.Add(woker.getName(), woker);
                woker.initialize(_factory);
                woker.onRegister();
            }
        }
        /**
         * 获取该集合所光控工厂中的一个工人。
         * @param name 要获取的工人实例的名称。
         * @return 该工人存在则返回该工人实例，否则返回null。
         */
        public ISAWorker getWorker(string name)
        {
            return workerTreeList.ContainsKey(name) ? workerTreeList[name] : null;
        }
        /**
         * 移除一个所管控工厂中的工人从该集合.
         * @param name 要移除的工人实例的名称。
         * @return 该工人存在溢出成功则返回该工人实例，否则返回null。
         */
        public ISAWorker removeWorker(string name)
        {
            if (workerTreeList.ContainsKey(name))
            {
                ISAWorker worker = workerTreeList[name];
                workerTreeList.Remove(name);
                worker.onRemove();
                return worker;
            }
            return null;
        }

        public void callAllOnRegister()
        {
            Dictionary<string, ISAWorker>.Enumerator workerID = workerTreeList.GetEnumerator();
            while (workerID.MoveNext())
            {
                workerID.Current.Value.onRegisterComplete();
            }
        }
    }
}