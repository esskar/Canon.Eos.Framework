using System;
using System.Diagnostics;
using Canon.Eos.Framework.Internal;

namespace Canon.Eos.Framework
{
    partial class EosCamera
    {
        private void OnStateEventShutdown(EventArgs eventArgs)
        {
            if (this.Shutdown != null)
                this.Shutdown(this, eventArgs);
        }

        private uint HandleStateEvent(uint stateEvent, uint param, IntPtr context)
        {
            Debug.WriteLine("HandleStateEvent fired: " + stateEvent);
            switch (stateEvent)
            {
                case Edsdk.StateEvent_Shutdown:
                    this.OnStateEventShutdown(EventArgs.Empty);
                    break;
            }
            return Edsdk.EDS_ERR_OK;
        }        
    }
}
