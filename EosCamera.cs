using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using EDSDKLib;

namespace Canon.Eos.Framework
{
    public sealed class EosCamera : EosDisposable
    {
        private IntPtr _camera;
        private EDSDK.EdsDeviceInfo _deviceInfo;
        private bool _sessionOpened = false;

        internal EosCamera(IntPtr camera)
        {
            _camera = camera;
            EDSDK.EdsGetDeviceInfo(_camera, out _deviceInfo);
        }

        public string DeviceDescription
        {
            get { return _deviceInfo.szDeviceDescription; }
        }

        public string PortName
        {
            get { return _deviceInfo.szPortName; }
        }

        public void TakePicture()
        {
            this.SendCommand(EDSDK.CameraCommand_TakePicture, 0);
        }

        private void SendCommand(uint command, int parameter)
        {
            if (!_sessionOpened)
            {
                var openSession = EDSDK.EdsOpenSession(_camera);
                if (openSession != EDSDK.EDS_ERR_OK)
                    throw new EosException(openSession, "Failed to open session.");
                _sessionOpened = true;
            }

            var sendCommand = EDSDK.EdsSendCommand(_camera, command, parameter);
            if (sendCommand != EDSDK.EDS_ERR_OK)
                throw new EosException(sendCommand, string.Format("Failed to send command: {0} with parameter {1}", command, parameter));
        }

        protected internal override void DisposeUnmanaged()
        {
            if (_sessionOpened)
                EDSDK.EdsCloseSession(_camera);

            EDSDK.EdsRelease(_camera);
            base.DisposeUnmanaged();
        }
    }
}
