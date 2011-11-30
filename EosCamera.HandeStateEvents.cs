using System;
using System.Diagnostics;
using System.Windows.Forms;
using EDSDKLib;

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
                case EDSDK.StateEvent_Shutdown:
                    this.OnStateEventShutdown(EventArgs.Empty);
                    break;
            }
            return EDSDK.EDS_ERR_OK;
        }        
    }
}
