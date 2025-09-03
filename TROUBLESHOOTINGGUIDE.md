## Android Device not detecting?
Enable Developer options in your Android phone. Follow https://developer.android.com/studio/debug/dev-options

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
