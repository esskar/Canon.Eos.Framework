# Canon.Eos.Framework (Alpha Version) 

Canon.Eos.Framework tries to provide a better and more convenient way to integrate the [Canon EOS SDK](http://www.didp.canon-europa.com/) into .NET applications.
You still need to apply for and download the original SDK which includes all the DLLs.

## How to use it
`using System;
using Canon.Eos.Framework;

namespace Canon.Eos.Framework.App
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var framework = new EosFramework())
            {
                using (var cameras = framework.GetCameraCollection())
                {
                    Console.WriteLine("Camera Count: {0}", cameras.Count);
                    foreach (var camera in cameras)
                    {
                        Console.WriteLine("\tDevice Description: {0}", camera.DeviceDescription);
                        Console.WriteLine("\tPort Name:          {0}", camera.PortName);
                        try
                        {
                            camera.TakePicture();
                        }
                        catch (EosException eos)
                        {
                            Console.Error.WriteLine("Last Command Failed with error {0}, {1}", eos.EosErrorCode, eos);
                        }

                        Console.WriteLine();
                    }
                }
            }
        }
    }
}`

