## Appium Server not starting?
* Go to Server->Troubleshooter and fix if there's any issue shown.
* If you have installed appium manually other than appium wizard, there's a chance that it may conflict with the custom installed server/drivers. In that case, try Re-Installing everything using Troubleshooter(Enable Show progress window, to see what's the actual issue while installing).
* Go to Server -> Configuration -> Settings and see if the Final command is valid as per the installed server version.

## Android Device not detecting?
* Enable Developer options in your Android phone. Follow https://developer.android.com/studio/debug/dev-options
* Authorize the device when connecting to the PC.

## Issues while running android execution?
* Go to the phone's Developer Options. Disable all permission and security related settings, and grant permissions such as "Install apps over USB," etc. It is not possible to cover all options since they differ based on device brands.
  So, carefully check the options in Developer Options that may affect app installation and app permissions, and enable or disable them based on the option descriptions.
* Go to Server->Restart ADB Server. This might fix the adb timeout error or UIAutomator crashed error.
* Go to Server->Updater and update the UIAutomator driver.
* Delete the "io.appium.uiautomator2.server" and "io.appium.uiautomator2.server.test" apps from the phone and Open the device.
* Restart the device.
* Delete and add the device again.
* Restart Appium Wizard.

## Unable to Start iPhone automation?
* Make sure to Enable Developer Mode in your iPhone and try again. Go to Settings->Privacy & Security->Developer Mode->Turn ON.
#### Still not working? Follow below steps:
* Go to Server->Updater and update the XCUITest driver.
* Delete WebDriverAgent from iPhone and Re-Install again by opening the device.
* Restart the device.
* Delete and add the device again.
* Restart Appium Wizard.
