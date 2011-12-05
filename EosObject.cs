using System;
using System.Runtime.InteropServices;
using System.Text;
using EDSDKLib;

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
            get { return this.GetPropertyStringData(EDSDK.PropID_Artist); }
        }        

        public string Copyright
        {
            get { return this.GetPropertyStringData(EDSDK.PropID_Copyright); }
        }

        public string FirmwareVersion
        {
            get { return this.GetPropertyStringData(EDSDK.PropID_FirmwareVersion); }
        }

        public string OwnerName
        {
            get { return this.GetPropertyStringData(EDSDK.PropID_OwnerName); }
        }

        public string ProductName
        {
            get { return this.GetPropertyStringData(EDSDK.PropID_ProductName); }
        }        

        public string SerialNumber
        {
            get { return this.GetPropertyStringData(EDSDK.PropID_BodyIDEx); }
        }

        protected internal override void DisposeUnmanaged()
        {
            EDSDK.EdsRelease(_handle);
            base.DisposeUnmanaged();
        }

        protected long GetPropertyIntegerData(uint propertyId)
        {
            uint data;
            EosAssert.NotOk(EDSDK.EdsGetPropertyData(this.Handle, propertyId, 0, out data), 
                string.Format("Failed to get property integer data: propertyId {0}", propertyId));
            return data;
        }

        protected string GetPropertyStringData(uint propertyId)
        {
            string data;
            EosAssert.NotOk(EDSDK.EdsGetPropertyData(this.Handle, propertyId, 0, out data), 
                string.Format("Failed to get property string data: propertyId {0}", propertyId));
            return data;
        }        

        protected void SetPropertyIntegerData(uint propertyId, long data)
        {
            EosAssert.NotOk(EDSDK.EdsSetPropertyData(this.Handle, propertyId, 0, Marshal.SizeOf(typeof(uint)), (uint)data), 
                string.Format("Failed to set property integer data: propertyId {0}, data {1}", propertyId, data));
        }

        protected void SetPropertyStringData(uint propertyId, string data, int maxByteLength)
        {
            var bytes = Encoding.ASCII.GetBytes(data + "\0");
            if (bytes.Length > maxByteLength)
                throw new ArgumentException(string.Format("'{0}' converted to bytes is longer than {1}.", data, maxByteLength), "data");

            EosAssert.NotOk(EDSDK.EdsSetPropertyData(this.Handle, propertyId, 0, bytes.Length, bytes),
                string.Format("Failed to set property string data: propertyId {0}, data {1}", propertyId, data));
        }
    }
}
