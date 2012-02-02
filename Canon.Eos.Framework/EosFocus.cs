using System.Drawing;
using Canon.Eos.Framework.Internal.SDK;

namespace Canon.Eos.Framework
{
    public struct EosFocus
    {
        internal static EosFocus Create(Edsdk.EdsFocusInfo focus)
        {
            var focusPoints = new EosFocusPoint[focus.pointNumber];
            for (var i = 0; i < focusPoints.Length; ++i)
                focusPoints[i] = EosFocusPoint.Create(focus.focusPoint[i]);
            
            return new EosFocus
            {
                Bounds = new Rectangle {
                    X = focus.imageRect.x,
                    Y = focus.imageRect.y,
                    Height = focus.imageRect.height,
                    Width = focus.imageRect.width,
                },                
                ExecuteMode = focus.executeMode,                
                FocusPoints = focusPoints
            };
        }

        public Rectangle Bounds { get; private set; }

        public long ExecuteMode { get; private set; }

        public EosFocusPoint[] FocusPoints { get; private set; }
    }
}
