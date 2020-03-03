using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using System.Windows.Media;
using System.Net.Sockets;
using System.Net;
using System.Collections;

namespace oosServerChanger {
    static class Utils {
        public static int GetAnAvailablePort() {
            Random random = new Random(Guid.NewGuid().GetHashCode());
            int ip = random.Next(11451, 65535);
            int tryTimes = 0;
            while (IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners().Any(p => p.Port == ip)) {
                ip = random.Next(11451, 65535);
                tryTimes++;
                if (tryTimes > 100) return -1;
            }
            return ip;
        }

        public static long MyPing(string ip) {
            try {
                PingReply reply = new Ping().Send(ip, 1000);
                if (reply != null) {
                    return reply.RoundtripTime;
                }
            } catch { return 0; }
            return 0;
        }

        public static int MyUdpPing(string ipString) {
            try {
                UdpClient udpClient = new UdpClient(GetAnAvailablePort());
                udpClient.Client.SendTimeout = 1000;
                udpClient.Client.ReceiveTimeout = 3000;

                IPAddress ip = IPAddress.Parse(ipString); 
                IPEndPoint RemoteIpEndPoint = new IPEndPoint(ip, 0);
                Byte[] sendBytes = Encoding.ASCII.GetBytes("}}}}}}}}}}}}1");

                DateTime beforDT = System.DateTime.Now;
                    udpClient.Connect(ip, 5055);
                    udpClient.Send(sendBytes, sendBytes.Length);
                    Byte[] receiveBytes = udpClient.Receive(ref RemoteIpEndPoint);
                DateTime afterDT = System.DateTime.Now;

                udpClient.Close();

                return (int)(Math.Round(afterDT.Subtract(beforDT).TotalMilliseconds, 0));
            } catch (Exception e) {
                return 0;
            }
        }

        public static bool SetServer(string serverName) {
            string valueName = "";
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true).OpenSubKey("Behold Studios", true).OpenSubKey("Out of Space", true);
            string[] allValueNames = key.GetValueNames();
            foreach (var value in allValueNames) {
                if (value.Contains("photonRegion")) {
                    valueName = value;
                    break;
                }
            }
            if (valueName == "") {
                return false;
            }
            key.SetValue(valueName, System.Text.Encoding.Default.GetBytes(serverName + "\0"));
            return true;
        }

        public static string GetServer() {
            string valueName = "";
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true).OpenSubKey("Behold Studios", true).OpenSubKey("Out of Space", true);
            string[] allValueNames = key.GetValueNames();
            foreach (var value in allValueNames) {
                if (value.Contains("photonRegion")) {
                    valueName = value;
                    break;
                }
            }
            return valueName == "" ? "" : Encoding.Default.GetString((byte[])key.GetValue(valueName));
        }

        public static void SetThemeUseingWindowsTheme() {
            var paletteHelper = new PaletteHelper();
            ITheme theme = paletteHelper.GetTheme();

            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true).OpenSubKey("Microsoft", true)
                .OpenSubKey("Windows", true).OpenSubKey("CurrentVersion", true).OpenSubKey("Themes")
                .OpenSubKey("Personalize", true) ;
            theme.SetBaseTheme(
                ((int)(key.GetValue("AppsUseLightTheme")) == 1) ? Theme.Light : Theme.Dark
            );
            theme.SetPrimaryColor((Color)ColorConverter.ConvertFromString(SystemParameters.WindowGlassBrush.ToString()));
            paletteHelper.SetTheme(theme);
        }

        public static void SetTheme(string baseTheme = "", string primaryColor="", string secondColor="") {

        }
    }
}
