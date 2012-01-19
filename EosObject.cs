using System;
using System.Runtime.InteropServices;
using System.Text;
using Canon.Eos.Framework.Internal;

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
            EosAssert.NotOk(Edsdk.EdsGetPropertyData(this.Handle, propertyId, 0, out data), 
                string.Format("Failed to get property integer data: propertyId {0}", propertyId));
            return data;
        }

        protected string GetPropertyStringData(uint propertyId)
        {
            string data;
            EosAssert.NotOk(Edsdk.EdsGetPropertyData(this.Handle, propertyId, 0, out data), 
                string.Format("Failed to get property string data: propertyId {0}", propertyId));
            return data;
        }        

        protected void SetPropertyIntegerData(uint propertyId, long data)
        {
            EosAssert.NotOk(Edsdk.EdsSetPropertyData(this.Handle, propertyId, 0, Marshal.SizeOf(typeof(uint)), (uint)data), 
                string.Format("Failed to set property integer data: propertyId {0}, data {1}", propertyId, data));
        }

        private static byte[] ConvertStringToBytes(string data)
        {
            return Encoding.ASCII.GetBytes(data + "\0");
        }

        protected void SetPropertyStringData(uint propertyId, string data, int maxByteLength)
        {
            var bytes = EosObject.ConvertStringToBytes(data);
            if (bytes.Length > maxByteLength)
                throw new ArgumentException(string.Format("'{0}' converted to bytes is longer than {1}.", data, maxByteLength), "data");

            EosAssert.NotOk(Edsdk.EdsSetPropertyData(this.Handle, propertyId, 0, bytes.Length, bytes),
                string.Format("Failed to set property string data: propertyId {0}, data {1}", propertyId, data));
        }
    }
}
