using UnityEngine;
namespace Sacu.Core
{
    public class SAUnDisplayElement : ISAElement
    {
        public SAUnDisplayElement(string name)
        {
            _name = name;
        }
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