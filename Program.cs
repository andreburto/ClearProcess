using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ClearProcess
{
    static class Program
    {
        public static Creds c;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                c = new Creds();
                c.Load();

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
            catch (Exception ex)
            {
                DbStuff.ErrMsg(ex);
            }
        }
    }
}
