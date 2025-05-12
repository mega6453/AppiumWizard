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
* Server Settings - Set server arguments and default capabilities separately for each server 
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
* Option to use pre-installed WDA by right clicking on iPhone (skipping certificate check) (**New**)
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

## Features/Improvements in Queue
* Add cancel button for progress window
* Status bar for Element interaction (WaitForElement, Swipe, etc.)
* Performance and Reliability Improvements
* Error handling
* Use devices remotely
* and more...

## Built With
* .Net C# & Windows Forms
* NodeJS
* Appium, XCUITest, UIAutomator2
* go-ios
* pymobiledevice3
* imobiledevice-net
* zsign
* adb
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

## Icons
* <a href="https://www.flaticon.com/free-icons/left-arrow" title="left arrow icons">Left arrow icons created by Lizel Arina - Flaticon</a>
* <a href="https://www.flaticon.com/free-icons/homepage" title="homepage icons">Homepage icons created by Md Tanvirul Haque - Flaticon</a>
* <a href="https://www.flaticon.com/free-icons/search" title="search icons">Search icons created by Maxim Basinski Premium - Flaticon</a>
* <a href="https://www.flaticon.com/free-icons/pin" title="pin icons">Pin icons created by Pixel perfect - Flaticon</a>
* <a href="https://www.flaticon.com/free-icons/toggle-button" title="toggle button icons">Toggle button icons created by Vectorslab - Flaticon</a>
* <a href="https://www.flaticon.com/free-icons/android" title="android icons">Android icons created by Roundicons - Flaticon</a>
* <a href="https://www.flaticon.com/free-icons/logo" title="logo icons">Logo icons created by Pixel perfect - Flaticon</a>
* <a href="https://www.flaticon.com/free-icons/configuration" title="configuration icons">Configuration icons created by mynamepong - Flaticon</a>
* <a href="https://www.flaticon.com/free-icons/troubleshooting" title="troubleshooting icons">Troubleshooting icons created by Flat Icons - Flaticon</a>
* <a href="https://www.flaticon.com/free-icons/inspector" title="inspector icons">Inspector icons created by HAJICON - Flaticon</a>
* <a href="https://www.flaticon.com/free-icons/team-management" title="team management icons">Team management icons created by pojok d - Flaticon</a>
* <a href="https://www.flaticon.com/free-icons/docx-file" title="docx file icons">Docx file icons created by Shuvo.Das - Flaticon</a>
* <a href="https://www.flaticon.com/free-icons/word-doc" title="word doc icons">Word doc icons created by Roman Káčerek - Flaticon</a>
* <a href="https://www.flaticon.com/free-icons/manual" title="manual icons">Manual icons created by Freepik - Flaticon</a>
* <a href="https://www.flaticon.com/free-icons/info" title="info icons">Info icons created by kumakamu - Flaticon</a>
* <a href="https://www.flaticon.com/free-icons/copy" title="copy icons">Copy icons created by Freepik - Flaticon</a>
* <a href="https://www.flaticon.com/free-icons/linked" title="linked icons">Linked icons created by Freepik - Flaticon</a>
* <a href="https://www.flaticon.com/free-icons/refresh" title="refresh icons">Refresh icons created by Freepik - Flaticon</a>
* <a href="https://www.flaticon.com/free-icons/install" title="install icons">Install icons created by NajmunNahar - Flaticon</a>
* <a href="https://www.flaticon.com/free-icons/restart" title="restart icons">Restart icons created by Paul J. - Flaticon</a>
* <a href="https://www.flaticon.com/free-icons/screenshot" title="screenshot icons">Screenshot icons created by icon_small - Flaticon</a>
* <a href="https://www.flaticon.com/free-icons/more" title="more icons">More icons created by Anggara - Flaticon</a>
* <a href="https://www.flaticon.com/free-icons/signature" title="signature icons">Signature icons created by srip - Flaticon</a>
* <a href="https://www.flaticon.com/free-icons/virus" title="virus icons">Virus icons created by Freepik - Flaticon</a>
* <a href="https://www.flaticon.com/free-icons/discussion" title="discussion icons">Discussion icons created by Freepik - Flaticon</a>
* <a href="https://www.flaticon.com/free-icons/upgrade" title="upgrade icons">Upgrade icons created by Arkinasi - Flaticon</a>
* <a href="https://www.flaticon.com/free-icons/update" title="update icons">Update icons created by Freepik - Flaticon</a>
* <a href="https://www.flaticon.com/free-icons/yes" title="yes icons">Yes icons created by hqrloveq - Flaticon</a>
* <a href="https://www.flaticon.com/free-icons/settings" title="settings icons">Settings icons created by Freepik - Flaticon</a>
* <a href="https://www.flaticon.com/free-icons/execute" title="execute icons">Execute icons created by surang - Flaticon</a>
* <a href="https://www.flaticon.com/free-icons/etc" title="etc icons">Etc icons created by riajulislam - Flaticon</a>
* <a href="https://www.flaticon.com/free-icons/proxy" title="proxy icons">Proxy icons created by Freepik - Flaticon</a>
* <a href="https://www.flaticon.com/free-icons/unlock" title="unlock icons">Unlock icons created by Design Circle - Flaticon</a>
* <a href="https://www.flaticon.com/free-icons/brush" title="brush icons">Brush icons created by Freepik - Flaticon</a>
* <a href="https://www.flaticon.com/free-animated-icons/ui" title="ui animated icons">Ui animated icons created by Freepik - Flaticon</a>
* <a href="https://www.flaticon.com/free-icons/record-button" title="record button icons">record button icons created by sonnycandra - Flaticon</a>
* <a href="https://www.flaticon.com/free-icons/notification" title="notification icons">Notification icons created by Freepik - Flaticon</a>
* <a href="https://www.flaticon.com/free-icons/tick" title="tick icons">Tick icons created by Octopocto - Flaticon</a>
* <a href="https://www.flaticon.com/free-icons/plugin" title="plugin icons">Plugin icons created by prettycons - Flaticon</a>
* <a href="https://www.flaticon.com/free-icons/log" title="log icons">Log icons created by juicy_fish - Flaticon</a>
* <a href="https://www.flaticon.com/free-icons/square" title="square icons">Square icons created by Freepik - Flaticon</a>
* <a href="https://www.flaticon.com/free-icons/seo-and-web" title="seo and web icons">Seo and web icons created by RaftelDesign - Flaticon</a>

## NOTE
Appium Wizard is in it's early stage of development. So,expect 
* Unhandled exceptions
* Crashes 
* Performance issues
* Reliability issues
  
The above issues are expected in Appium Wizard software and not in your mobile app under test.

PLEASE CREATE AN ISSUE UNDER ISSUES SECTION(WITH LOGS FROM APPIUM WIZARD->HELP->OPEN LOGS FOLDER), IF YOU ARE OBSERVING ANY ISSUE WHICH WILL HELP TO IMPROVE THE QUALITY OF APPIUM WIZARD. THANKS !

## License

This project is dual-licensed under the MIT and GPL 3.0 licenses.

- [MIT License](./LICENSE-MIT)
- [GPL 3.0 License](./LICENSE-GPL)
  
## Developed By
* [**Meganathan C**](https://mega6453.carrd.co)

## Contributing to the Project
* **Fork the Repo**: Click the "Fork" button at the top right of the repository page to create your own copy of the repository.
* **Make Changes**: Clone your forked repository to your local machine, and make the necessary changes in a new branch. It's a good practice to give your branch a descriptive name related to the changes you're making.
* **Test Your Changes**: Ensure your changes work as expected and do not introduce new issues. Add tests if applicable.
* **Commit Your Changes**: Commit your changes with clear and descriptive commit messages.
* **Push to Your Fork**: Push your changes to your forked repository on GitHub.
* **Create a Pull Request**: Navigate to the original repository and click "New Pull Request." Provide a detailed description of the changes you made and why they are necessary.
