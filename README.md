<p align="center">
  <img src="https://github.com/mega6453/AppiumWizard/blob/master/Appium%20Wizard/Resources/Images/appium%20wizard%20small.jpg" alt="Appium Wizard Logo">
</p>

# Appium Wizard for Mobile Automation Testing

Appium Wizard is a Windows software designed to streamline mobile automation using the open-source Appium server. This user-friendly tool helps testers to seamlessly run mobile automation tests on android and iOS(without depending on Mac machine) platforms in windows.

## During Installation
Please provide necessary permission when system prompts. Because,
1. If NodeJS not installed in your system, Alpium Wizard will download and install it.
2. If Appium Sever not installed, Appium Wizard will Install it.
3. Appium Wizard will Install iOS and Android drivers.
4. Appium Wizard will Install WSL for iOS app signing and run automation.

## How to Use
1. Install Appium Wizard and Launch it.<br>
2. Click Add Device -> Add your iOS/Android device.<br>
3. For iOS : Go to Tools -> iOS profile management and Add your iOS provisioning profiles (Get the profiles from Apple Developer account for your device or Check with your iOS developer)
4. Select the device and Click Open device to open the device reflection.<br>
5. Now start running automation from your automation script. (Make sure you have given the correct appium server port number and device id in your script)

## NOTE
Appium Wizard is in it's early stage of development.So,expect 
* Unhandled exceptions
* Crashes 
* Performance issues
* Reliability issues
The above issues are expected in Appium Wizard software and not in your mobile app under test.

PLEASE CREATE AN ISSUE UNDER ISSUES SECTION IF YOU ARE OBSERVING ANY ISSUE WHICH WILL HELP TO IMPROVE THE QUALITY OF APPIUM WIZARD. THANKS !

## Features Available

* Upto 5 Parallel Appium servers
* iOS Automation Execution
* iOS Screen Reflection and Screen Control
* iOS Profile management
* IPA Installation
* Android Automation Execution
* Android Screen Reflection and Screen Control
* APK Installation
* Server Management
* Troubleshooter
* etc.

## Features/Improvements in Queue
* Show Action + current element(e.g. Click on //someElement) text in status bar based on appium execution
* Handle resources on Application exit
* Highlight element accessed by Appium in Screen Reflection
* Unlock Mobile using Screen Control
* Performance and Reliability Improvements
* Error handling

## Known Issues (fix in progress)
* Android Screen control has some issues.
* iOS 17 not supported 
* Performance issues

## Built With
* .Net C# & Windows Forms 
* NodeJS
* Appium
* go-ios 
* imobiledevice-net 
* wsl
* adb
* Newtonsoft.Json
* RestSharp
* Innosetup

## Thanks To
* [danielpaulus](https://github.com/danielpaulus) - For the excellent go-ios cli tool which provides "operating system independent implementation of iOS device features".
* [libimobiledevice-win32](https://github.com/libimobiledevice-win32)

## Authors
* [**Meganathan C**](https://mega6453.carrd.co)

