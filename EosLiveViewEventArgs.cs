using System;
using System.Drawing;

namespace Canon.Eos.Framework
{
    [Obsolete("Use EosLiveViewEventArgs instead.")]
    public class LiveViewEventArgs : EventArgs 
    { 
        internal LiveViewEventArgs(Image image)
        {
            this.Image = image;
        }

        public Image Image { get; private set; }
    }

    public class EosLiveViewEventArgs : LiveViewEventArgs
    {
        internal EosLiveViewEventArgs(Image image)
            : base(image) { }
    }    
}
