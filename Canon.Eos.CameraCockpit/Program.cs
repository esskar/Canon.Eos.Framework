using System;
using System.Windows.Forms;
using Canon.Eos.CameraCockpit.Forms;

namespace Canon.Eos.CameraCockpit
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new CockpitForm());            
        }        
    }
}
