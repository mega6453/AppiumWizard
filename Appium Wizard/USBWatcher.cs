using System.Management;
using System.Text.RegularExpressions;

namespace Appium_Wizard
{
    public class USBWatcher
    {
        private ManagementEventWatcher connectWatcher;
        private ManagementEventWatcher disconnectWatcher;
        ListView listView1;
        public USBWatcher(ListView listview)
        {
            this.listView1 = listview;
        }
        public void Start()
        {
            var connectQuery = new WqlEventQuery("SELECT * FROM __InstanceCreationEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_USBControllerDevice'");
            connectWatcher = new ManagementEventWatcher(connectQuery);
            connectWatcher.EventArrived += UsbDeviceConnected;
            connectWatcher.Start();

            var disconnectQuery = new WqlEventQuery("SELECT * FROM __InstanceDeletionEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_USBControllerDevice'");
            disconnectWatcher = new ManagementEventWatcher(disconnectQuery);
            disconnectWatcher.EventArrived += UsbDeviceDisconnected;
            disconnectWatcher.Start();
        }



        public void Stop()
        {
            if (connectWatcher != null)
            {
                connectWatcher.Stop();
                connectWatcher.Dispose();
            }

            if (disconnectWatcher != null)
            {
                disconnectWatcher.Stop();
                disconnectWatcher.Dispose();
            }
        }



        private void UsbDeviceConnected(object sender, EventArrivedEventArgs e)
        {
            var targetInstance = (ManagementBaseObject)e.NewEvent.Properties["TargetInstance"].Value;
            var dependent = (string)targetInstance.GetPropertyValue("Dependent");
            var deviceId = dependent.Split(new[] { '\\', '"' }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
            Console.WriteLine("USB device connected: " + deviceId);
            if (!deviceId.Contains('&'))
            {
                listView1.Invoke((MethodInvoker)delegate
                {
                    foreach (ListViewItem item in listView1.Items)
                    {
                        string udidFromList = Regex.Replace(item.SubItems[4].Text, "[^a-zA-Z0-9]", "");
                        if (udidFromList.Equals(deviceId, StringComparison.InvariantCultureIgnoreCase))
                        {
                            string OS = item.SubItems[2].Text;
                            string udid = item.SubItems[4].Text;
                            if (OS.Equals("iOS"))
                            {
                                //iOSMethods.GetInstance().RunWebDriverAgentQuick(udid);
                                //int proxyPort = LoadingScreen.WDAproxyPort;
                                //int screenServerPort = Common.GetFreePort();
                                //iOSAsyncMethods.GetInstance().StartiProxyServer(udid, proxyPort, 8100);
                                //iOSAsyncMethods.GetInstance().StartiProxyServer(udid, screenServerPort, 9100);
                                //Thread.Sleep(5000);
                                //ScreenControl.screenControl.LoadScreen(udid);
                            }
                            //else   // not sure needed or not...
                            //{
                            //    AndroidAsyncMethods.GetInstance().StartUIAutomatorServer(udid);
                            //}
                            item.SubItems[3].Text = "Online";                           
                        }
                    }
                });
            }
        }


        private void UsbDeviceDisconnected(object sender, EventArrivedEventArgs e)
        {
            var targetInstance = (ManagementBaseObject)e.NewEvent.Properties["TargetInstance"].Value;
            var dependent = (string)targetInstance.GetPropertyValue("Dependent");
            var deviceId = dependent.Split(new[] { '\\', '"' }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
            Console.WriteLine("USB device disconnected: " + deviceId);
            if (!deviceId.Contains('&'))
            {
                listView1.Invoke((MethodInvoker)delegate
                {
                    foreach (ListViewItem item in listView1.Items)
                    {
                        string udidFromList = Regex.Replace(item.SubItems[4].Text, "[^a-zA-Z0-9]", "");
                        if (udidFromList.Equals(deviceId,StringComparison.InvariantCultureIgnoreCase))
                        {
                            string OS = item.SubItems[2].Text;
                            string udid = item.SubItems[4].Text;
                            if (OS.Equals("iOS"))
                            {
                                ScreenControl.screenControl.LoadDeviceDisconnected(udid);
                            }
                            item.SubItems[3].Text = "Offline";
                        }
                    }
                });
            }
        }
    }
}
