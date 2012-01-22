using System;
using System.Collections.Generic;
using System.Linq;
using Canon.Eos.Framework;

namespace Canon.Eos.CameraCockpit
{
    public sealed class FrameworkManager
    {
        private EosFramework _framework;

        public event EventHandler CameraAdded;

        public IEnumerable<EosCamera> GetCameras()
        {
            using (var cameras = _framework.GetCameraCollection())
                return cameras.Select(camera => camera).ToArray();
        }

        public void LoadFramework()
        {
            if (_framework == null)
            {
                _framework = new EosFramework();
                _framework.CameraAdded += this.HandleCameraAdded;
            }
        }

        public void ReleaseFramework()
        {
            if (_framework != null)
            {
                _framework.CameraAdded -= this.HandleCameraAdded;
                _framework.Dispose();
            }
        }        

        private void HandleCameraAdded(object sender, EventArgs eventArgs)
        {
            if (this.CameraAdded != null)
                this.CameraAdded(this, eventArgs);
        }
    }
}
