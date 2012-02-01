using System.Drawing;
using Canon.Eos.Framework.Internal.SDK;

namespace Canon.Eos.Framework
{
    public struct EosFocusPoint
    {
        internal static EosFocusPoint Create(Edsdk.EdsFocusPoint focusPoint)
        {
            return new EosFocusPoint
            {
                Bounds = new Rectangle {
                    X = focusPoint.rect.x,
                    Y = focusPoint.rect.y,
                    Height = focusPoint.rect.height,
                    Width = focusPoint.rect.width,
                },
                IsInFocus = focusPoint.justFocus != 0,
                IsSelected = focusPoint.selected != 0,
                IsValid = focusPoint.valid != 0,
            };
        }

        public Rectangle Bounds { get; private set; }

        public bool IsInFocus { get; private set; }

        public bool IsValid { get; private set; }

        public bool IsSelected { get; private set; }        
    }
}
