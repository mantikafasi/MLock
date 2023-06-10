# MLock 
Very simple lock screen app for windows which support USB key and password.

https://github.com/mantikafasi/MLock/assets/67705577/99b12ef2-5bfa-49c4-bdae-295e3f98f0a3

# Features
- [x] Unlockable with USB key (SD card etc works too)
- [x] Unlockable with password
- [x] Disables task manager
- [x] Disables keyboard, mouse completely




## How to use

1. Download the latest release from Github Releases
2. Run MLockConfigurator.exe
3. If you want to use USB key, click "USB Unlocking" and press "Generate" to create a key file. This will create privateKey.xml in directory, store privateKey.xml in somewhere safe since its only way to create new USB keys.
4. If using USB key, select USB from drowdown menu and press "Generate USB Key"
5. Fill other fields and press "Install MLock Config". This will save your config.json and publicKey to %appdata%/MLock
6. Run MLock.exe as admin, if you want to run at startup create a task scheluder or create a regedit key

Incase you get locked out, from different account with admin permissions you can delete config.json inside C:/Users/YourUser/AppData/Roaming/MLock

## TODO
- [ ] Add more unlock methods (Like from mobile app)
- [ ] Design a better UI (current one is terrible)
- [ ] Add option to configurator to run program at startup
- [ ] Add github actions to create releases automatically
- [ ] Maybe add option to remove sound when locked
- [ ] Log Failures to file
