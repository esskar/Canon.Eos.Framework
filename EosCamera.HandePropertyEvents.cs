using System;
using EDSDKLib;

namespace Canon.Eos.Framework
{
    partial class EosCamera
    {
        private uint HandlePropertyEvent(uint propertyEvent, uint propertyId, uint param, IntPtr context)
        {
            Console.WriteLine("HandlePropertyEvent fired: " + propertyEvent);
            return EDSDK.EDS_ERR_OK;
        }

        
    }
}
