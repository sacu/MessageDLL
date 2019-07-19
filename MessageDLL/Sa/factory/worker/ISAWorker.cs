using Sacu.Core;
namespace Sacu.Factory.Worker
{
    public interface ISAWorker : ISAElement
    {
        /**
        * 将该工人加入到指定的工厂。
        * @param factory 该工人要加入到的工厂实例。
        */
        void initialize(SABaseFactory factory);
        /**
        * 将该工人从指定的工厂中移除。
        * @param factory 指定该工人要移除的工厂实例。
        */
        void unInitialize(SABaseFactory factory);
        /**
        * 从该工人触发一个消息到它所任职的所有工厂。
        * @param type 消息类型。
        * @param body 消息附带的值，默认为null。
        */
        void dispatchEvent(string type, System.Object body = null);
        /**
        * 当该工人被任职到某个工厂时触发。
        */
        void onRegister();
        /**
        * 当该工人从某个工厂被解雇时触发。
        */
        void onRemove();

        void onRegisterComplete();
    }
}