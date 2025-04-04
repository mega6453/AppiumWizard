namespace Appium_Wizard
{
    public static class FilesPath
    {
        public static string adbFilePath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + "\\Resources\\Executables\\adb.exe";
        public static string aaptFilePath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + "\\Resources\\Executables\\aapt.exe";
        public static string serverAPKFilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\.appium\\node_modules\\appium-uiautomator2-driver\\node_modules\\appium-uiautomator2-server\\apks\\";
        public static string settingsAPKFilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\.appium\\node_modules\\appium-uiautomator2-driver\\node_modules\\io.appium.settings\\apks\\";
        public static string executablesFolderPath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + "\\Resources\\Executables\\";
        public static string databaseFilePath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + "\\Resources\\Data\\appiumwizard.db";
        public static string opensslFilePath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + "\\Resources\\iOS\\openssl.exe";
        public static string AppleDeviceTypesFilePath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + "\\Resources\\iOS\\Apple_mobile_device_types.txt";
        public static string ProfilesFilePath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + "\\Resources\\iOS\\Profiles\\";
        public static string iOSServerFilePath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + "\\Resources\\Executables\\iOSServer.exe";
        public static string iDeviceInfoFilePath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + "\\Resources\\Executables\\ideviceinfo.exe";
        public static string pListUtilFilePath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + "\\Resources\\Executables\\plistutil.exe";
        public static string iProxyFilePath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + "\\Resources\\Executables\\iproxy.exe";
        public static string iOSFilesPath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + "\\Resources\\iOS\\";
        public static string WDAFilePath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + "\\Resources\\iOS\\wda.ipa";
        public static string serverInstalledPath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + "\\Resources\\Server";
        public static string nodeZipPath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + "\\Resources\\Server\\Backup\\node.zip";
        public static string zipExtractorFilePath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + "\\Resources\\Server\\Backup\\7za.exe";
        public static string pymd3FilePath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + "\\Resources\\Executables\\iOSServerPy\\iOSServerPy.exe";
        public static string FFMpegFilePath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + "\\Resources\\Executables\\FFMpeg\\";
        public static string logsFilesPath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + "\\Resources\\Logs\\";
        public static string remoteExecutionPath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + "\\Resources\\RemoteExecution\\";
    }
}
