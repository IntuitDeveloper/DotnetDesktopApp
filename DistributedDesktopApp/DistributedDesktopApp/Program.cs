using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DistributedDesktopApp
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

            //Fix added to support the IE installed version on the user's machine and not IE7 by default
            var appName = System.Diagnostics.Process.GetCurrentProcess().ProcessName + ".exe";

            int BrowserVer, RegVal;
            RegistryKey Key = null;
            try
            {
                // get the latest installed IE version
                WebBrowser Wb = new WebBrowser();
                BrowserVer = Wb.Version.Major;

                // set the appropriate IE version
                if (BrowserVer >= 11)
                    RegVal = 11001;
                else if (BrowserVer == 10)
                    RegVal = 10001;
                else if (BrowserVer == 9)
                    RegVal = 9999;
                else if (BrowserVer == 8)
                    RegVal = 8888;
                else
                    RegVal = 7000;

               


                //For 64 bit Machine 
                if (Environment.Is64BitOperatingSystem)

                    Key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\\Wow6432Node\\Microsoft\\Internet Explorer\\MAIN\\FeatureControl\\FEATURE_BROWSER_EMULATION", true);
                else  //For 32 bit Machine 
                    Key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\FEATURE_BROWSER_EMULATION", true);

                //If the path is not correct or 
                //If user't have priviledges to access registry 
                if (Key == null)
                {
                    MessageBox.Show("Registry Key for setting IE WebBrowser Rendering Address Not found. Try run the program with administrator's right.");
                    Key.Close();
                }

                else
                {
                    string FindAppkey = Convert.ToString(Key.GetValue(appName));

                    //Check if key is already present 
                    if (FindAppkey == RegVal.ToString())
                    {
                        Key.Close();

                    }
                    else
                    {
                        //set the actual key
                        Key.SetValue(appName, RegVal, RegistryValueKind.DWord);
                        Key.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Registry Key for setting IE WebBrowser Rendering failed to setup");
                MessageBox.Show(ex.Message);
            }
            finally
            {
                //Close the Registry 
                if (Key != null)
                    Key.Close();
            }

            Application.Run(new Main());
        }
    }
}
