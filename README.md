[screenshot]: images/screen.jpg
# DEFCON Level Display

Requirements: .NET Framework 4.6.2

When started, the program retrieves the current DEFCON (Defense Condition, alert state used by the United States Armed Forces, see [defconlevel.com](https://www.defconlevel.com) for more information) level from [defconlevel.com](https://www.defconlevel.com) and shows it as an icon in the tray. When you hover over the icon, it displays text indicating the level and exercise term.

The program has an autostart system. If you right click on the tray icon, you can check/uncheck "Autostart". Enabling autostart moves files to %appdata%/DEFCON and adds a key to the registry so DEFCON tray icon will automatically appear after system restart.

The program refreshes the DEFCON level at each startup and every 15 minutes.

Special thanks to https://www.defconlevel.com

-----------------------------------------------------------------------------------
![screenshot][screenshot]