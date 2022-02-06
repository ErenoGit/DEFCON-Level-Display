using DEFCON_Level_Display.Properties;
using Microsoft.Win32;
using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace DEFCON_Level_Display
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new DefconLevelDisplayContext());
        }
    }


    public class DefconLevelDisplayContext : ApplicationContext
    {
        NotifyIcon trayIcon;
        int defconLevel = -1;
        int newDefconLevel = -1;
        MenuItem menuItemAustart, menuItemExit;

        public void SetDefconLevel(int defcon)
        {
            defconLevel = defcon;
            switch (defcon)
            {
                case 5:
                    trayIcon.Icon = Resources.DEFCON5;
                    trayIcon.Text = "Current DEFCON: 5 (FADE OUT)";
                    break;
                case 4:
                    trayIcon.Icon = Resources.DEFCON4;
                    trayIcon.Text = "Current DEFCON: 4 (DOUBLE TAKE)";
                    break;
                case 3:
                    trayIcon.Icon = Resources.DEFCON3;
                    trayIcon.Text = "Current DEFCON: 3 (ROUND HOUSE)";
                    break;
                case 2:
                    trayIcon.Icon = Resources.DEFCON2;
                    trayIcon.Text = "Current DEFCON: 2 (FAST PACE)";
                    break;
                case 1:
                    trayIcon.Icon = Resources.DEFCON1;
                    trayIcon.Text = "Current DEFCON: 1 (COCKED PISTOL)";
                    break;
                case -1:
                    trayIcon.Icon = Resources.WARNING;
                    trayIcon.Text = "Problem with loading current DEFCON!";
                    break;
            }
        }
        public DefconLevelDisplayContext()
        {
            menuItemAustart = new MenuItem("Autostart", Autostart);
            menuItemAustart.Checked = CheckIsAutostartEnabled();
            menuItemExit = new MenuItem("Exit", Exit);

            trayIcon = new NotifyIcon()
            {
                Icon = Resources.WARNING,
                ContextMenu = new ContextMenu(new MenuItem[] {
                    menuItemAustart,
                    menuItemExit
                }),
                Visible = true
            };

            defconLevel = DownloadAndCheckCurrentDefcon();
            SetDefconLevel(defconLevel);

            //infinite timer - every 15 min
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 900000; //15 min = 900000 ms
            timer.Elapsed += timer_Elapsed;
            timer.Start();
            //
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            newDefconLevel = DownloadAndCheckCurrentDefcon();
            if(newDefconLevel != defconLevel)
                SetDefconLevel(newDefconLevel);
        }

        void Exit(object sender, EventArgs e)
        {
            trayIcon.Visible = false;
            Application.Exit();
        }

        void Autostart(object sender, EventArgs e)
        {
            if(menuItemAustart.Checked)
            {
                RegistryKey rkey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
                rkey.DeleteValue("DEFCONLevelDisplay", false);

                menuItemAustart.Checked = false;
            }
            else
            {
                Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DEFCON"));
                if(Application.ExecutablePath != Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DEFCON", "DEFCON Level Display.exe"))
                    File.Copy(Application.ExecutablePath, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DEFCON", "DEFCON Level Display.exe"), true);

                RegistryKey rkey = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
                rkey.SetValue("DEFCONLevelDisplay", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DEFCON", "DEFCON Level Display.exe"));
                menuItemAustart.Checked = true;
            }
        }

        bool CheckIsAutostartEnabled()
        {
            RegistryKey rkey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
            object o = rkey.GetValue("DEFCONLevelDisplay", null);

            if (o != null)
                return true;
            else
                return false;
        }

        public int DownloadAndCheckCurrentDefcon()
        {
            try
            {
                Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DEFCON"));

                using (WebClient webClient = new WebClient())
                {
                    webClient.DownloadFile("https://www.defconlevel.com/images/current.webp", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DEFCON", "currentDEFCON.webp"));
                }

                string actualDefconMd5 = GetMD5Checksum(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DEFCON", "currentDEFCON.webp"));

                if (actualDefconMd5 == "6F6858084E1B1A87FF70A6266C4A8E21")
                    return 5;
                else if (actualDefconMd5 == "AC9DFD21D3F309A1718DD313D20C0317")
                    return 4;
                else if (actualDefconMd5 == "6427885D37685F526FD7A2758998A8CF")
                    return 3;
                else if (actualDefconMd5 == "CB8226B38CED0ADD81AA87AD5B8B9FF0")
                    return 2;
                else if (actualDefconMd5 == "D27C0A0085FD3FE81444558FCEE0497A")
                    return 1;
                else
                    return -1;
            }
            catch
            {
                return -1;
            }
        }

        private string GetMD5Checksum(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "");
                }
            }
        }
    }
}
