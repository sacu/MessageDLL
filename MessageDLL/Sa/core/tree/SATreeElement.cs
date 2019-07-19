
namespace Sacu.Core.Tree
{
    public class SATreeElement : ISATreeElement
    {
        private ISAElement _element;
        private ISATreeElement _root;
        private ISATreeElement _left;
        private ISATreeElement _right;
        public SATreeElement(ISAElement element)
        {
            _element = element;
        }
        /**
        * 节点名称。
        */
        public string getName()
        {
            return _element.getName();
        }
        /**
            * 节点元素。
            */
        public ISAElement getElement()
        {
            return _element;
        }
        /**
            * 父节点。
            */
        public ISATreeElement getRoot()
        {
            return _root;
        }
        public void setRoot(ISATreeElement value)
        {
            _root = value;
        }
        /**
            * 左子节点。
            */
        public ISATreeElement getLeft()
        {
            return _left;
        }
        public void setLeft(ISATreeElement value)
        {
            _left = value;
        }
        /**
            * 右子节点。
            */
        public ISATreeElement getRight()
        {
            return _right;
        }
        public void setRight(ISATreeElement value)
        {
            _right = value;
        }

    }
}