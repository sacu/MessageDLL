
namespace Sacu.Core.Tree
{
    public interface ISATreeElement
    {
        string getName();
        /**
        * 节点元素。
        */
        ISAElement getElement();//自身
        /**
        * 父节点。
        */
        ISATreeElement getRoot();//父亲
        void setRoot(ISATreeElement value);
        /**
        * 左子节点。
        */
        ISATreeElement getLeft();//左子
        void setLeft(ISATreeElement value);
        /**
        * 右子节点。
        */
        ISATreeElement getRight();//右子
        void setRight(ISATreeElement value);

    }
}