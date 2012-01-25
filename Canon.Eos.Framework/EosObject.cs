using System;
using System.Runtime.InteropServices;
using Canon.Eos.Framework.Extensions;
using Canon.Eos.Framework.Helper;
using Canon.Eos.Framework.Internal;
using Canon.Eos.Framework.Internal.SDK;

namespace Canon.Eos.Framework
{
    public abstract class EosObject : EosDisposable
    {
        private readonly IntPtr _handle;

        internal EosObject(IntPtr handle)
        {
            _handle = handle;
        }

        protected internal IntPtr Handle
        {
            get { return _handle; }
        }

        public string Artist
        {
            get { return this.GetPropertyStringData(Edsdk.PropID_Artist); }
        }        

        public string Copyright
        {
            get { return this.GetPropertyStringData(Edsdk.PropID_Copyright); }
        }

        public string FirmwareVersion
        {
            get { return this.GetPropertyStringData(Edsdk.PropID_FirmwareVersion); }
        }

        public string OwnerName
        {
            get { return this.GetPropertyStringData(Edsdk.PropID_OwnerName); }
        }

        public string ProductName
        {
            get { return this.GetPropertyStringData(Edsdk.PropID_ProductName); }
        }        

        public string SerialNumber
        {
            get { return this.GetPropertyStringData(Edsdk.PropID_BodyIDEx); }
        }

        protected internal override void DisposeUnmanaged()
        {
            Edsdk.EdsRelease(_handle);
            base.DisposeUnmanaged();
        }

        protected long GetPropertyIntegerData(uint propertyId)
        {
            uint data;
            this.Assert(Edsdk.EdsGetPropertyData(this.Handle, propertyId, 0, out data), 
                string.Format("Failed to get property integer data: propertyId {0}", propertyId), propertyId);
            return data;
        }

        protected string GetPropertyStringData(uint propertyId)
        {
            string data;
            this.Assert(Edsdk.EdsGetPropertyData(this.Handle, propertyId, 0, out data), 
                string.Format("Failed to get property string data: propertyId {0}", propertyId), propertyId);
            return data;
        }

        protected void SetPropertyIntegerData(uint propertyId, long data)
        {
            this.Assert(Edsdk.EdsSetPropertyData(this.Handle, propertyId, 0, Marshal.SizeOf(typeof(uint)), (uint)data),
                string.Format("Failed to set property integer data: propertyId {0}, data {1}", propertyId, data),
                propertyId, data);
        }

        protected void SetPropertyStringData(uint propertyId, string data, int maxByteLength)
        {
            var bytes = Util.ConvertStringToBytesWithNullByteAtEnd(data);
            if (bytes.Length > maxByteLength)
                throw new ArgumentException(string.Format("'{0}' converted to bytes is longer than {1}.", data, maxByteLength), "data");

            this.Assert(Edsdk.EdsSetPropertyData(this.Handle, propertyId, 0, bytes.Length, bytes),
                string.Format("Failed to set property string data: propertyId {0}, data {1}", propertyId, data),
                propertyId, data);
        }
    }
}
