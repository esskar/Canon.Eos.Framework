using System;
using System.Drawing;

namespace Canon.Eos.Framework.Eventing
{
    public class EosPictureInfoEventArgs : EventArgs
    {
        internal EosPictureInfoEventArgs(string pictureFilePath)
        {
            this.PictureFilePath = pictureFilePath;
        }

        public string PictureFilePath { get; private set; }

        public Image GetImage()
        {
            return Image.FromFile(this.PictureFilePath);
        }
    }
}
