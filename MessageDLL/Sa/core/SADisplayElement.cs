using UnityEngine;
namespace Sacu.Core
{
    public class SADisplayElement : MonoBehaviour, ISAElement
    {
        //public DisplayElement(string name)
        //{
        //    _name = name;
        //}
        private string _name;
        public string getName()
        {
            return _name;
        }

        public void setName(string value)
        {
            _name = value;
        }
    }
}