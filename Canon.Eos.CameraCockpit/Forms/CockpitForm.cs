using System;
using System.Drawing;
using System.Windows.Forms;
using Canon.Eos.CameraCockpit.Properties;
using Canon.Eos.Framework;
using Canon.Eos.Framework.Eventing;

namespace Canon.Eos.CameraCockpit.Forms
{
    public partial class CockpitForm : Form
    {
        private readonly FrameworkManager _manager;

        public CockpitForm()
        {
            _manager = new FrameworkManager();
            _manager.CameraAdded += this.HandleCameraAdded;
            this.InitializeComponent();
        }
        
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.StartUp();                    
            this.LoadCameras();
        }

        private EosCamera GetSelectedCamera()
        {
            return _cameraCollectionComboBox.SelectedIndex >= 0 ? _cameraCollectionComboBox.SelectedItem as EosCamera : null;
        }

        private void HandleCameraAdded(object sender, EventArgs e)
        {
            this.LoadCameras();
        }

        private void HandleCameraShutdown(object sender, EventArgs e)
        {
            this.LoadCameras();
        }

        private void HandlePictureTaken(object sender, EosImageEventArgs e)
        {
            this.UpdatePicture(e.GetImage());
        }

        private void HandleTakePictureButtonClick(object sender, EventArgs e)
        {
            var camera = this.GetSelectedCamera();
            if (camera != null)
                camera.TakePicture();
        }                

        private void LoadCameras()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(this.LoadCameras));
            }
            else
            {
                _cameraCollectionComboBox.Items.Clear();
                foreach (var camera in _manager.GetCameras())
                {
                    var iq = camera.ImageQuality;

                    camera.Shutdown += this.HandleCameraShutdown;
                    camera.PictureTaken += this.HandlePictureTaken;
                    _cameraCollectionComboBox.Items.Add(camera);
                }
                if (_cameraCollectionComboBox.Items.Count > 0)
                    _cameraCollectionComboBox.SelectedIndex = 0;
            }
        }        

        private void StartUp()
        {
            try
            {
                _manager.LoadFramework();
            }
            catch(EosException eosex)
            {
                MessageBox.Show(eosex.ToString(), Resources.FrameworkLoadError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }            
        }

        private void TearDown()
        {
            _manager.ReleaseFramework();
        }

        private void UpdatePicture(Image image)
        {
            if (this.InvokeRequired)
                this.Invoke(new Action(() => this.UpdatePicture(image)));
            else
                _pictureBox.Image = image;
        }
    }
}
