![GitHub Downloads (all assets, all releases)](https://img.shields.io/github/downloads/mega6453/AppiumWizard/total)
![GitHub Release](https://img.shields.io/github/v/release/mega6453/AppiumWizard)
![License: MIT/GPL-3.0](https://img.shields.io/badge/license-MIT%2FGPL--3.0-blue)
![GitHub Repo stars](https://img.shields.io/github/stars/mega6453/AppiumWizard?style=plastic&color=green)


<p align="center">
  <img src="logo.jpg" alt="Appium Wizard Logo">
</p>

# Appium Wizard for Mobile Automation Testing

Appium Wizard is a Windows software designed to streamline mobile automation using the open-source Appium server. This user-friendly tool helps testers to seamlessly run mobile automation tests on android and iOS(without depending on Mac machine) platforms in windows. 

:star2: Star :star2: this project to find it easily again, show your appreciation, and help make this project popular! :smiley:



https://github.com/mega6453/AppiumWizard/assets/50325649/1fb5333e-4be0-46c5-b2b6-35f36695f800



## Downloading and Installing
* Download the Appium Wizard zip file from here --> https://github.com/mega6453/AppiumWizard/releases/latest
* Appium Wizard executable is added in a zip file because some browser suspects that this exe is a suspicious file. So, after downloading, extract the exe file and Run it.
* When you run the exe file, you will see "**Microsoft Defender SmartScreen prevented an unrecognized app from starting.** Running this app might put your PC at risk." error. This error is because, this software is not signed with a certificate which involves cost in buying the certificate. Click on **More Info -> Run Anyway to start the installation.**      

## During Installation
* Appium Wizard will Install Appium Server, iOS and Android drivers over the internet using npm. So, Please make sure you have proper internet connection during installation.
  
  ####  During first launch
  First launch will verify the installation of the software components(i.e.Appium Server and drivers).  If any of the component installation is not proper, then Appium Wizard will try 
to install it during the first time launch which may take sometime to complete it. Again here, Please make sure you have proper internet connection during installation.

## How to Use
1. Install Appium Wizard and Launch it.<br>
2. Click Add Device -> Add your iOS/Android device.<br>
3. For iOS : Go to Tools -> iOS profile management and Add your iOS provisioning profiles (Get the profiles from Apple Developer account for your device or Check with your iOS developer)
4. Select the device and Click Open device to open the device reflection.<br>
5. Now start running automation from your automation script. (Make sure you have given the correct appium server port number and device id in your script)

## Features Available
#### Server
* 5 Parallel Appium servers
* Server Settings - Set server arguments, default capabilities & log level separately for each server 
* Update Appium Server and drivers
* Troubleshooter to fix appium and driver issues
* Plugins Manager - Install, Update and Uninstall
#### iOS
* iOS Automation Execution
* iOS Screen Reflection and Screen Control
* iOS Profile management
* IPA Signer
* iOS Proxy selection with 3 methods
* iOS Executor - 2 ways to execute iOS actions
* Option to use pre-installed WDA by right clicking on iPhone (skipping certificate check)
#### Android 
* Android Automation Execution
* Android Screen Reflection and Screen Control
* Use Android 11+ device over Wi-Fi
#### Common
* Status bar for Element interaction (Find element, Click and Send text, etc.)
* Highlight element accessed by Appium in Screen Reflection during execution
* Manage Apps - Install / Launch / Kill / Uninstall / Clear App Data
* Reboot Device
* Unlock Mobile from Screen Control
* Take Device Screenshot
* Record Screen from Screen Control
* Notifications - Enable/Disable Notifications for few events 
* Object Spy - Beta version
* Test Runner (**New**)
* Record and Playback (**New**)

## Built With
* .Net C# & Windows Forms
* NodeJS
* Appium, XCUITest, UIAutomator2
* go-ios
* pymobiledevice3
* imobiledevice-net
* zsign
* adb
* scrcpy
* FFMpeg
* Newtonsoft.Json
* RestSharp
* Innosetup

## Thanks To
* [Appium](https://github.com/appium) - Without Appium, there's no Appium Wizard.
* [danielpaulus](https://github.com/danielpaulus) - For iOS device features in windows.
* [doronz88](https://github.com/doronz88/pymobiledevice3) - For iOS device features in windows. 
* [libimobiledevice-win32](https://github.com/libimobiledevice-win32) - For iOS device features in windows. 
* [zhlynn](https://github.com/zhlynn) - For zsign which helps in iOS app signing.
* [Genymobile](https://github.com/Genymobile/scrcpy) - For scrcpy to screen mirror android device.

## Icons
This project uses icons from external providers.  
Full list and attributions are in [ICONS.md](./ICONS.md).

## NOTE
Appium Wizard is in it's early stage of development. So,expect 
* Unhandled exceptions
* Crashes 
* Performance issues
* Reliability issues
  
The above issues are expected in Appium Wizard software and not in your mobile app under test.

PLEASE CREATE AN ISSUE UNDER ISSUES SECTION(WITH LOGS FROM APPIUM WIZARD->HELP->OPEN LOGS FOLDER), IF YOU ARE OBSERVING ANY ISSUE WHICH WILL HELP TO IMPROVE THE QUALITY OF APPIUM WIZARD. THANKS !

## License

This project is licensed under MIT, GPL-3.0, or Apache-2.0.

- [MIT License](./LICENSE-MIT)
- [GPL 3.0 License](./LICENSE-GPL)
- [Apache License 2.0](./LICENSE-Apache)
  
## Developed By
* [**Meganathan C**](https://mega6453.carrd.co)

## Changelog
* [Changelog](./CHANGELOG.md)

## Troubleshooting Guide
* [Troubleshooting Guide](./TROUBLESHOOTINGGUIDE.md)

## Contributing to the Project
* **Fork the Repo**: Click the "Fork" button at the top right of the repository page to create your own copy of the repository.
* **Make Changes**: Clone your forked repository to your local machine, and make the necessary changes in a new branch. It's a good practice to give your branch a descriptive name related to the changes you're making.
* **Test Your Changes**: Ensure your changes work as expected and do not introduce new issues. Add tests if applicable.
* **Commit Your Changes**: Commit your changes with clear and descriptive commit messages.
* **Push to Your Fork**: Push your changes to your forked repository on GitHub.
* **Create a Pull Request**: Navigate to the original repository and click "New Pull Request." Provide a detailed description of the changes you made and why they are necessary.
