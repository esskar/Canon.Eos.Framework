using System;
using System.Drawing;

namespace Canon.Eos.Framework.Eventing
{
    public class EosLivePictureEventArgs : EventArgs
    {
        internal EosLivePictureEventArgs(Image image)
        {
            this.Image = image;
        }

        public Image Image { get; private set; }
    }    
}
