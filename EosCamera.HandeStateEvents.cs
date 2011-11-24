using System;
using EDSDKLib;

namespace Canon.Eos.Framework
{
    partial class EosCamera
    {
        private uint HandleStateEvent(uint stateEvent, uint param, IntPtr context)
        {
            Console.WriteLine("HandleStateEvent fired: " + stateEvent);
            return EDSDK.EDS_ERR_OK;
        }

    }
}
