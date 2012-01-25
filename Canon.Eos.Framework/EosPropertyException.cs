using System;

namespace Canon.Eos.Framework
{
    public class EosPropertyException : EosException
    {
        internal EosPropertyException(long eosErrorCode)
            : base(eosErrorCode)
        {            
        }

        internal EosPropertyException(long eosErrorCode, string message)
            : base(eosErrorCode, message) 
        {            
        }

        internal EosPropertyException(long eosErrorCode, string message, Exception innerException)
            : base(eosErrorCode, message, innerException) { }

        public uint PropertyId { get; internal set; }

        public object PropertyValue { get; internal set; }
    }
}
