using System;
using System.Reflection;

using Sacu.Events;
namespace Sacu.Core.Tree
{
    public class SATreeList : SAEventDispatcher
    {
        public static string CHANGE = ".change";
        private ISATreeElement _root;
        private int _length;
        /**
             * 构造函数
             */
        public SATreeList()
        {
            _root = null;
            _length = 0;
        }
        private void addNode(ISATreeElement treeElementRoot, ISATreeElement treeElement)
        {
            switch (toDirection(treeElementRoot.getName(), treeElement.getName()))
            {
                case -1:
                    {//左子节点查询
                        if (treeElementRoot.getLeft() != null)
                        {
                            addNode(treeElementRoot.getLeft(), treeElement);
                        }
                        else
                        {
                            treeElementRoot.setLeft(treeElement);
                            treeElement.setRoot(treeElementRoot);
                        }
                        break;
                    }
                case 1:
                    {//右子节点查询
                        if (treeElementRoot.getRight() != null)
                        {
                            addNode(treeElementRoot.getRight(), treeElement);
                        }
                        else
                        {
                            treeElementRoot.setRight(treeElement);
                            treeElement.setRoot(treeElementRoot);
                        }
                        break;
                    }
                case 0:
                    {//相同
                        if (treeElementRoot.getLeft() != null)
                        {
                            treeElementRoot.getLeft().setRoot(treeElement);
                            treeElement.setLeft(treeElementRoot.getLeft());
                        }
                        if (treeElementRoot.getRight() != null)
                        {
                            treeElementRoot.getRight().setRoot(treeElement);
                            treeElement.setRight(treeElementRoot.getRight());
                        }
                        if (treeElementRoot.getRoot() != null && treeElementRoot.getRoot().getLeft() != null && treeElementRoot.getRoot().getLeft() == treeElementRoot)
                        {
                            treeElementRoot.getRoot().setLeft(treeElement);
                        }
                        else if (treeElementRoot.getRoot() != null && treeElementRoot.getRoot().getRight() != null && treeElementRoot.getRoot().getRight() == treeElementRoot)
                        {
                            treeElementRoot.getRoot().setRight(treeElement);
                        }
                        else
                        {
                            //						trace(treeElement.getName());
                            //						trace(treeElementRoot.getRoot().getLeft().getName());
                            //						trace(treeElementRoot.getRoot().getRight().getName());
                        }
                        treeElement.setRoot(treeElementRoot.getRoot());
                        //....游离被替换资源数据
                        break;
                    }
            }
        }
        //----------------添加节点结束
        //----------------移除对象名节点
        private ISATreeElement removeTreeElement(string elementName)
        {//删除一个工人对象
            if (_root == null)
            {
                return null;
            }
            ISATreeElement treeElement = getTreeElement(elementName);//要删除的节点
            if (treeElement != null)
            {
                removeNode(treeElement);
                _length--;
                dispatchEvent(new SATreeListEvent(SATreeList.CHANGE));
                return treeElement;
            }
            else
            {
                return null;
            }
        }
        private bool removeNode(ISATreeElement treeElement)
        {
            if (treeElement.getRoot() != null)
            {
                if (treeElement.getLeft() != null)
                {
                    if (treeElement.getRoot().getLeft() != null && treeElement.getRoot().getLeft() == treeElement)
                    {
                        treeElement.getRoot().setLeft(treeElement.getLeft());
                    }
                    else
                    {
                        treeElement.getRoot().setRight(treeElement.getLeft());
                    }
                    if (treeElement.getRight() != null)
                    {
                        addNode(_root, treeElement.getRight());
                    }
                    treeElement.getLeft().setRoot(treeElement.getRoot());
                }
                else if (treeElement.getRight() != null)
                {
                    if (treeElement.getRoot().getLeft() != null && treeElement.getRoot().getLeft() == treeElement)
                    {
                        treeElement.getRoot().setLeft(treeElement.getRight());
                    }
                    else
                    {
                        treeElement.getRoot().setRight(treeElement.getRight());
                    }
                    treeElement.getRight().setRoot(treeElement.getRoot());
                }
                else
                {
                    clearNode(treeElement);
                }
            }
            else
            {
                if (treeElement.getLeft() != null)
                {
                    _root = treeElement.getLeft();
                    treeElement.getLeft().setRoot(null);
                    if (treeElement.getRight() != null)
                    {
                        addNode(_root, treeElement.getRight());
                    }
                }
                else if (treeElement.getRight() != null)
                {
                    _root = treeElement.getRight();
                    treeElement.getRight().setRoot(null);
                }
                else
                {
                    _root = null;
                }
            }
            return false;
        }
        //---------------移除对象名节点结束
        //---------------查找对象名节点
        private ISATreeElement getTreeElement(string elementName)
        {//获取一个工人对象
            if (_root == null)
            {
                return null;
            }
            return getNode(_root, elementName);
        }
        private ISATreeElement getNode(ISATreeElement treeElementRoot, string elementName)
        {
            ISATreeElement treeElement = null;
            switch (toDirection(treeElementRoot.getName(), elementName))
            {
                case -1:
                    {
                        if (treeElementRoot.getLeft() != null)
                        {
                            treeElement = getNode(treeElementRoot.getLeft(), elementName);
                        }
                        else
                        {
                            //未在左子节点中找到该对象
                        }
                        break;
                    }
                case 1:
                    {
                        if (treeElementRoot.getRight() != null)
                        {
                            treeElement = getNode(treeElementRoot.getRight(), elementName);
                        }
                        else
                        {
                            //未在右子节点中找到该对象
                        }
                        break;
                    }
                case 0:
                    {
                        treeElement = treeElementRoot;
                        break;
                    }
            }
            return treeElement;
        }
        //---------------查找对象名节点结束
        //----------------左、右子节点树选择
        private int toDirection(string elementRootName, string elementName)
        {
            return string.Compare(elementName, elementRootName);//左节点-1，有节点1，相等0
        }
        /**
             * 添加一个节点到该二叉树内，并通过TreeListNotifier事件类型抛出类型为TreeListNotifier.CHANGE的事件。
             * @param element 实现ISAElement接口的实例对象。
             * @return 返回当前添加状态，true表示成功 ，false则失败。
             */
        public bool addElement(ISAElement element)
        {//添加一个工人对象
            SATreeElement treeElement = new SATreeElement(element);
            if (_root == null)
            {
                _root = treeElement;
                _length++;
                dispatchEvent(new SATreeListEvent(SATreeList.CHANGE));
                return true;
            }
            addNode(_root, treeElement);//添加节点
            _length++;
            dispatchEvent(new SATreeListEvent(SATreeList.CHANGE));
            return true;
        }
        //------------------删除节点
        /**
             * 移除一个节点从该二叉树内。
             * @param elementName 要移除的节点名称，并通过TreeListNotifier事件类型抛出类型为TreeListNotifier.CHANGE的事件。
             * @return 移除成功则返回当前移除对象实例 ，否则返回null。
             */
        public ISAElement removeElement(string elementName)
        {//添加一个工人对象
            ISATreeElement treeElement = removeTreeElement(elementName);
            return treeElement != null ? treeElement.getElement() : null;
        }
        //------------------获取节点
        /**
             * 获取一个节点从该二叉树内。
             * @param elementName 要获取的节点名称。
             * @return 获取成功则返回当前获取对象实例，否则返回null。
             */
        public ISAElement getElement(string elementName)
        {//添加一个工人对象
            ISATreeElement treeElement = getTreeElement(elementName);
            return treeElement != null ? treeElement.getElement() : null;
        }
        //----------------遍历节点
        /**
             * 遍历该二叉树内的所有节点。
             * @param structure 遍历过程中每个节点元素所要传入执行的方法，这里注意向该方法传入的是元素对象非节点对象。
             * @return 遍历成功则返回true，否则返回false。
             */
        public bool traverse(Object target, string structure)
        {
            if (_root == null)
            {
                return false;
            }
            Type t = target.GetType();

            MethodInfo methodInfo = t.GetMethod(structure, BindingFlags.NonPublic | BindingFlags.Instance);
            if (methodInfo != null)
            {
                ParameterInfo[] paramsInfo = methodInfo.GetParameters();//得到指定方法的参数列表
                if (paramsInfo.Length == 1)
                {//参数数量
                    inOrder(_root, target, methodInfo);
                }
            }

            return true;
        }


        private void inOrder(ISATreeElement treeElementRoot, Object target, MethodInfo methodInfo)
        {
            if (treeElementRoot != null)
            {
                inOrder(treeElementRoot.getLeft(), target, methodInfo);
                methodInfo.Invoke(target, new Object[] { treeElementRoot.getElement() });
                inOrder(treeElementRoot.getRight(), target, methodInfo);
            }
        }
        //----------------遍历节点结束
        //----------------清空节点
        /**
             * 清空当前二叉树所有节点，并通过TreeListNotifier事件类型抛出类型为TreeListNotifier.CHANGE的事件。
             * @return 清空成功则返回true，否则返回false或因操作不慎导致的异常。
             */
        public bool clear()
        {
            _length = 0;
            _root = null;
            dispatchEvent(new SATreeListEvent(SATreeList.CHANGE));
            return true; //traverse(clearNode);
        }
        private void clearNode(ISATreeElement treeElementRoot)
        {
            if (treeElementRoot.getRoot() == null)
            {
                _root = null;
            }
            else if (treeElementRoot.getRoot() != null && treeElementRoot.getRoot().getLeft() == treeElementRoot)
            {
                treeElementRoot.getRoot().setLeft(null);
            }
            else
            {
                treeElementRoot.getRoot().setRight(null);
            }
        }
        //----------------清空节点结束
        /**
             * 获得当前二叉树长度
             * @return 当前二叉树内元素总个数。
             */
        public int Length
        {
            get
            {
                return _length;
            }
        }
    }
}