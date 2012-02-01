using System;
using System.Runtime.InteropServices;
using Canon.Eos.Framework.Extensions;
using Canon.Eos.Framework.Helper;
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

        [EosProperty(Edsdk.PropID_Artist)]
        public string Artist
        {
            get { return this.GetPropertyStringData(Edsdk.PropID_Artist); }
        }        

        [EosProperty(Edsdk.PropID_Copyright)]
        public string Copyright
        {
            get { return this.GetPropertyStringData(Edsdk.PropID_Copyright); }
        }

        [EosProperty(Edsdk.PropID_FocusInfo)]
        public EosFocus Focus
        {
            get
            {
                int dataSize;
                Edsdk.EdsDataType dataType;
                this.Assert(Edsdk.EdsGetPropertySize(this.Handle, Edsdk.PropID_FocusInfo, 0, out dataType, out dataSize),
                    "Failed to get property size for FocusInfo.", Edsdk.PropID_FocusInfo);

                if (dataType != Edsdk.EdsDataType.FocusInfo)
                    throw new EosException(-1, "Returned DataType was not FocusInfo.");

                var ptr = Marshal.AllocHGlobal(dataSize);
                try
                {
                    this.Assert(Edsdk.EdsGetPropertyData(this.Handle, Edsdk.PropID_FocusInfo, 0, dataSize, ptr),
                        "Failed to get FocusInfo.");
                    return EosFocus.Create((Edsdk.EdsFocusInfo)Marshal.PtrToStructure(ptr, typeof(Edsdk.EdsFocusInfo)));
                }
                finally
                {
                    Marshal.FreeHGlobal(ptr);
                }
            }
        }

        [EosProperty(Edsdk.PropID_FirmwareVersion)]
        public string FirmwareVersion
        {
            get { return this.GetPropertyStringData(Edsdk.PropID_FirmwareVersion); }
        }

        [EosProperty(Edsdk.PropID_OwnerName)]
        public string OwnerName
        {
            get { return this.GetPropertyStringData(Edsdk.PropID_OwnerName); }
        }

        [EosProperty(Edsdk.PropID_ProductName)]
        public string ProductName
        {
            get { return this.GetPropertyStringData(Edsdk.PropID_ProductName); }
        }        

        [EosProperty(Edsdk.PropID_BodyIDEx)]
        public string SerialNumber
        {
            get { return this.GetPropertyStringData(Edsdk.PropID_BodyIDEx); }
        }

        [EosProperty(Edsdk.PropID_WhiteBalance)]
        public EosWhiteBalance WhiteBalance
        {
            get { return (EosWhiteBalance)this.GetPropertyIntegerData(Edsdk.PropID_WhiteBalance); }
            set { this.SetPropertyIntegerData(Edsdk.PropID_WhiteBalance, (long)value); }
        }
        
        protected internal override void DisposeUnmanaged()
        {
            Edsdk.EdsRelease(_handle);
            base.DisposeUnmanaged();
        }

        protected string GetPropertyDescription(uint propertyId)
        {
            Edsdk.EdsPropertyDesc desc;
            this.Assert(Edsdk.EdsGetPropertyDesc(this.Handle, propertyId, out desc),
                string.Format("Failed to get property description for data: propertyId {0}", propertyId), propertyId);
            return null;
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

        protected void SetPropertyIntegerArrayData(uint propertyId, uint[] data)
        {
            this.Assert(Edsdk.EdsSetPropertyData(this.Handle, propertyId, 0, Marshal.SizeOf(typeof(uint))*data.Length, data),
                string.Format("Failed to set property integer array data: propertyId {0}, data {1}", propertyId, data),
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
