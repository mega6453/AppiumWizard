﻿namespace Appium_Wizard
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
        public static string ProfilesFilePath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + "\\Resources\\iOS\\Profiles\\";
        public static string iOSServerFilePath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + "\\Resources\\Executables\\iOSServer.exe";
        public static string pListUtilFilePath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + "\\Resources\\Executables\\plistutil.exe";
        public static string iProxyFilePath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + "\\Resources\\Executables\\iproxy.exe";
        public static string iOSFilesPath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + "\\Resources\\iOS\\";

    }
}