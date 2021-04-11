using System;

namespace UnityEngine
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
    public class AttributeGalore : PropertyAttribute
    {
        public readonly string header;

        public AttributeGalore(string header)
        {
            this.header = header;
        }
    }
}