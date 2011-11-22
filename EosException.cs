using System;

namespace Canon.Eos.Framework
{
    public class EosException : Exception
    {
        public EosException(long eosErrorCode, string message)
            : base(message) 
        {
            this.EosErrorCode = eosErrorCode;
        }

        public EosException(long eosErrorCode, string message, Exception innerException)
            : base(message, innerException) 
        {
            this.EosErrorCode = eosErrorCode;
        }

        public long EosErrorCode { get; private set; }        
    }
}
