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
                            if (ScreenControl.webview2.ContainsKey(udid))
                            {
                                if (OS.Equals("iOS"))
                                {
                                    int screenPort = ScreenControl.devicePorts[udid].Item1;
                                    int proxyPort = ScreenControl.devicePorts[udid].Item2;
                                    Common.KillProcessByPortNumber(proxyPort);
                                    Common.KillProcessByPortNumber(screenPort);
                                    iOSAsyncMethods.GetInstance().StartiOSProxyServer(udid, proxyPort, 8100);
                                    iOSAsyncMethods.GetInstance().StartiOSProxyServer(udid, screenPort, 9100);
                                    bool isRunning = !iOSMethods.GetInstance().IsWDARunning(proxyPort).Contains("nosession");
                                    if (!isRunning)
                                    {
                                        iOSMethods.GetInstance().RunWebDriverAgentQuick(udid);
                                    }
                                    bool isLoaded = Common.IsLocalhostLoaded("http://localhost:" + screenPort);
                                    int count = 0;
                                    while (!isLoaded && count == 5)
                                    {
                                        isLoaded = Common.IsLocalhostLoaded("http://localhost:" + screenPort);
                                        if (!isLoaded)
                                        {
                                            Thread.Sleep(2000);
                                        }
                                        count++;
                                    }
                                    ScreenControl.screenControl.LoadScreen(udid, screenPort);
                                }
                                else
                                {
                                    //int screenPort = ScreenControl.devicePorts[udid].Item1;
                                    //int proxyPort = ScreenControl.devicePorts[udid].Item2;
                                    //AndroidMethods.GetInstance().StopUIAutomator(udid);
                                    //AndroidAsyncMethods.GetInstance().StartUIAutomatorServer(udid);
                                    //AndroidMethods.GetInstance().StartAndroidProxyServer(proxyPort, 6790, udid);
                                    //AndroidMethods.GetInstance().StartAndroidProxyServer(screenPort, 7810, udid);
                                    //AndroidAPIMethods.DeleteSession(proxyPort);
                                    //string sessionIdCreatedForScreenServer = AndroidAPIMethods.CreateSession(proxyPort, screenPort);
                                    //if (OpenDevice.deviceSessionId.ContainsKey(udid))
                                    //{
                                    //    OpenDevice.deviceSessionId[udid] = sessionIdCreatedForScreenServer;
                                    //}
                                    //else
                                    //{
                                    //    OpenDevice.deviceSessionId.Add(udid, sessionIdCreatedForScreenServer);
                                    //}
                                    //---------------------------------------
                                    //int screenServerPort = 0;
                                    //int forwardedScreenPort = AndroidMethods.GetInstance().GetForwardedPort(udid, 7810);
                                    //if (forwardedScreenPort != -1)
                                    //{
                                    //    screenServerPort = forwardedScreenPort;
                                    //}
                                    //else
                                    //{
                                    //    screenServerPort = Common.GetFreePort();
                                    //    AndroidMethods.GetInstance().StartAndroidProxyServer(screenServerPort, 7810, udid);
                                    //}
                                    //---------------------------------------
                                    //bool isRunning = AndroidMethods.GetInstance().IsUIAutomatorRunning(udid);
                                    //if (isRunning)
                                    //{                                    
                                    //    AndroidAPIMethods.DeleteSession(proxyPort);
                                    //    string sessionIdCreatedForScreenServer = AndroidAPIMethods.CreateSession(proxyPort, screenPort);
                                    //    if (!sessionIdCreatedForScreenServer.Equals("nosession"))
                                    //    {
                                    //        if (OpenDevice.deviceSessionId.ContainsKey(udid))
                                    //        {
                                    //            OpenDevice.deviceSessionId[udid] = sessionIdCreatedForScreenServer;
                                    //        }
                                    //        else
                                    //        {
                                    //            OpenDevice.deviceSessionId.Add(udid, sessionIdCreatedForScreenServer);
                                    //        }
                                    //    }
                                    //}
                                    //else
                                    //{
                                    //    AndroidAsyncMethods.GetInstance().StartUIAutomatorServer(udid);
                                    //}  
                                    //---------------------------------------
                                    int proxyPort = Common.GetFreePort();
                                    int screenPort = Common.GetFreePort();
                                    AndroidMethods.GetInstance().StartAndroidProxyServer(proxyPort, 6790, udid);
                                    AndroidMethods.GetInstance().StartAndroidProxyServer(screenPort, 7810, udid);
                                    AndroidAsyncMethods.GetInstance().StartUIAutomatorServer(udid);
                                    Thread.Sleep(5000);
                                    //bool isLoaded = Common.IsLocalhostLoaded("http://localhost:" + screenPort);
                                    //int count = 0;
                                    //while (!isLoaded && count == 5)
                                    //{
                                    //    isLoaded = Common.IsLocalhostLoaded("http://localhost:" + screenPort);
                                    //    if (!isLoaded)
                                    //    {
                                    //        Thread.Sleep(2000);
                                    //    }
                                    //    count++;
                                    //}
                                    ScreenControl.screenControl.LoadScreen(udid, screenPort);
                                }
                            }
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
                            item.SubItems[3].Text = "Offline";
                            if (ScreenControl.webview2.ContainsKey(udid))
                            {
                                ScreenControl.screenControl.LoadDeviceDisconnected(udid);
                            }                            
                        }
                    }
                });
            }
        }
    }
}
