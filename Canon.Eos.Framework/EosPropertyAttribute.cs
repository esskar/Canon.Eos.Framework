using System;

namespace Canon.Eos.Framework
{
    public class EosPropertyAttribute : Attribute
    {
        public EosPropertyAttribute(uint propertyId)
        {
            this.PropertyId = propertyId;
        }

        public uint PropertyId { get; private set; }
    }
}
