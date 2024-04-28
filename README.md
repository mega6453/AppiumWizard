[![.NET Core Desktop](https://github.com/mega6453/AppiumWizard/actions/workflows/main.yml/badge.svg)](https://github.com/mega6453/AppiumWizard/actions/workflows/main.yml)
<p align="center">
  <img src="logo.jpg" alt="Appium Wizard Logo">
</p>

# Appium Wizard for Mobile Automation Testing

Appium Wizard is a Windows software designed to streamline mobile automation using the open-source Appium server. This user-friendly tool helps testers to seamlessly run mobile automation tests on android and iOS(without depending on Mac machine) platforms in windows.



https://github.com/mega6453/AppiumWizard/assets/50325649/1fb5333e-4be0-46c5-b2b6-35f36695f800



## Downloading and Installing
* Appium Wizard executable is added in a zip file because some browser thinks that this exe is a suspicious file. So, after downloading, extract the exe file and Run it.
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

## NOTE
Appium Wizard is in it's early stage of development. So,expect 
* Unhandled exceptions
* Crashes 
* Performance issues
* Reliability issues
  
The above issues are expected in Appium Wizard software and not in your mobile app under test.

PLEASE CREATE AN ISSUE UNDER ISSUES SECTION, IF YOU ARE OBSERVING ANY ISSUE WHICH WILL HELP TO IMPROVE THE QUALITY OF APPIUM WIZARD. THANKS !

## Features Available

* Upto 5 Parallel Appium servers
* iOS Automation Execution
* iOS Screen Reflection and Screen Control
* iOS Profile management
* IPA Signer
* Android Automation Execution
* Android Screen Reflection and Screen Control
* Server Management
* Troubleshooter
* Status bar for Element interaction (Find element, Click and Send text as of now)
* Mangage Apps - Install / Launch / Kill / Uninstall
* Take Device Screenshot
* Reboot Device
* etc.

## Features/Improvements in Queue
* Add cancel button for progress window
* Update Appium Server and drivers from Appium Wizard
* Status bar for Element interaction (WaitForElement, Swipe, etc.)
* Handle resources on Application exit
* Highlight element accessed by Appium in Screen Reflection
* Unlock Mobile using Screen Control
* Performance and Reliability Improvements
* Error handling

## Known Issues (fix in progress)
* iOS 17 not supported 
* Performance issues

## Built With
* .Net C# & Windows Forms 
* NodeJS
* Appium, XCUITest, UIAutomator2
* go-ios 
* imobiledevice-net
* zsign
* adb
* Newtonsoft.Json
* RestSharp
* Innosetup

## Thanks To
* [danielpaulus](https://github.com/danielpaulus) - For the excellent go-ios cli tool which provides "operating system independent implementation of iOS device features".
* [libimobiledevice-win32](https://github.com/libimobiledevice-win32) - For iOS device features in windows. 
* [zhlynn](https://github.com/zhlynn) - For zsign which helps in iOS app signing.
* [Dadoum](https://github.com/Dadoum/zsign-Windows) - For providing zsign windows executable.

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

## Developed By
* [**Meganathan C**](https://mega6453.carrd.co)

## Want to add features or fix things?
* Fork the Repo
* Make changes
* Create a pull request
