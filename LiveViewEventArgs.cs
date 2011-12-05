using System;
using System.Drawing;

namespace Canon.Eos.Framework
{
    public class LiveViewEventArgs : EventArgs
    {
        internal LiveViewEventArgs(Image image)
        {
            this.Image = image;
        }

        public Image Image { get; private set; }
    }
}
