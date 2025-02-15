# TBot
OGame Bot

[![GitHub all releases](https://img.shields.io/github/downloads/ogame-tbot/TBot/total)](https://github.com/ogame-tbot/TBot/releases/latest)
[![GitHub release (latest SemVer)](https://img.shields.io/github/v/release/ogame-tbot/TBot)](https://github.com/ogame-tbot/TBot/releases/latest)
[![Discord](https://img.shields.io/discord/801453618770214923)](https://discord.gg/NZSaY4aQ7J)

TBot is a .NET 5 [OGame](https://lobby.ogame.gameforge.com/) bot based on [ogamed deamon](https://github.com/alaingilbert/ogame) by alaingilbert

TBot supports Ogame v8!

Feel free to publish issues or pull requests

[![Stories in Ready](https://discordapp.com/api/guilds/801453618770214923/widget.png?style=banner2)](https://discord.gg/NZSaY4aQ7J)

## Disclaimer

Scripting and botting are forbidden by the OGame rules.
I adopt a series of measures to prevent detection and thus ban, but I cannot, and never will, guarantee anything.
Use this bot at your own risk!! Any testing is very much appreciated!

## Features

* Defender: TBot checks periodically for incoming attacks
  * Autofleet: TBot deploys your endangered fleet and resources to your closest celestial
    * Recall: TBot can autorecall the fleet
  * MessageAttacker: TBot sends a message to the attacker(s). The message is picked randomly from the array given in settings.json
  * SpyAttacker: TBot automatically spies attacker with set number of probes
  * Alarm: TBot plays a nasty sound if under attack
  * TelegramMessenger: TBot sends you a notice if under attack (requires additional configuration, see [below](#telegram))
* Expeditions: TBot will handle them for you
  * AutoSendExpeditions: TBot automatically optimizes expeditions for your account, sending them from the origin(s) setted in settings.json
* Brain: TBot has a series of extra functionalities
  * AutoCargo: TBot checks wether your celestials have enough capacity to displace the resources. If not, TBot builds ships for you (preferred type taken from settings.json)
  * AutoRepatriate: TBot periodically repatriates all your resources to a single drop celestial (guess where you can specify which...)
  * AutoMine (tnx to Stewie): Tbot will develop your colonies up to the levels given in settings.json. Transports are not implemented yet, so you should provide resources manually to speed up.
  * BuyOfferOfTheDay: TBot can buy the daily item from the Trader (check intervals are implemented so you can configure shorter check times when there is the specific event)
* AutoHarvest: TBot will harvest expedition debris in your celestials' systems as well as your own DFs
* SleepMode: TBot will not interact with your account between the hours specified in settings.json
* Local Proxy (tnx to ogamed and CrynogarTM for the hint): Tbot allows you to play in your browser
  * Insert the hostname of the machine you'll run TBot onto in the settings.json (i.e.: localhost, or the local ip of a computer on your local network such as 192.168.X.X)
  * Navigate with your browser to http://*hostname:port*/game/index.php (remember to change hostname and port with the ones you specified in settings.json)
  * Pay attention: TBot is not aware of what you do in the browser, it will do his job regardless of you playing manually, so keep an eye on the console
* Proxy: TBot supports routing your traffic through a proxy
  * Fill the settings in settings.json. The settings are quite self-explainatory. If you need assistance ping me on Discord

## Settings Hot Reload

TBot supports the editing of the settings even while it is running. It will take care of turning on and off features as well of the specific feature config settings.
  
## Running on Windows

* Download and unzip latest release.
* Insert you credentials in settings.json
  * Under "Universe" type your universe name with leading capital letter, i.e.: Andromeda, Bellatrix etc...
  * Under "Language" type your universe community code. You can find it by logging to your account and analyzing the url, such as s161-us.ogame.gameforge.com => us
* Configure the bot by editing all settings.json fields
  * All config options are sorted by feature, [check which features you](#features) want and configure them before activating
* Make sure you have installed the [.NET 5 runtime](https://dotnet.microsoft.com/download/dotnet/5.0) for your platform
* Run TBot.exe

## Running on Linux/MacOS

* Open a terminal (if you are in a desktop environment)
* Download latest release for your platform.
  * `wget https://github.com/ogame-tbot/TBot/releases/download/VERSION/TBot-VERSION-PLATFORM.zip` (*change the filename to your real one!*)
* Unzip the downloaded file
  * `unzip TBot-VERSION-PLATFORM.zip -d TBot` (*change the filename to your real one!*)
* Make ogamed executable (*this is only required on first run or update*)
  * `chmod +x ogamed`
* Make TBot executable (*this is only required on first run or update*)
  * `chmod +x TBot`
* Insert you credentials in settings.json
  * Under "Universe" type your universe name **with leading capital letter**, i.e.: Andromeda, Bellatrix etc...
  * Under "Language" type your universe community code. You can find it by logging to your account and analyzing the url, such as s161-us.ogame.gameforge.com => us
* Configure the bot by editing all settings.json fields
  * All config options are sorted by feature, [check which features](#features) you want and configure them before activating
* Make sure you have installed the [.NET 5 runtime](https://dotnet.microsoft.com/download/dotnet/5.0) for your platform
* Run TBot
  * `./TBot`

### Telegram
TBot supports automated Telegram messaging. In order to enable it, you need to follow theese steps:
* Create a new Telegram bot
  * Write "/new_bot" to [@botfather](https://t.me/botfather)
  * Follow the instructions given by BotFather, assigning a name and an username for the bot (theese are not important, set them to whatever you like)
  * BotFather will send you a message containing the API Key you need
  * Insert the newly obtained API Key in settings.json under TelegramMessenger.API
* Get your ChatID
  * Write "/start" to [@getmyid_bot](https://t.me/getmyid_bot)
  * It will answer you a message containing your user ID and chat ID (WARNING: you need the USER ID)
  * Insert the newly obtained ID in settings.json under TelegramMessenger.ChatId

### Captcha solving
TBot, being based on ogamed, supports the Ninja Captcha Autoresolve service. Just follow [this guide](https://github.com/alaingilbert/ogame/wiki/auto-captcha-using-ninja-solver) and insert the obtained APIKey in settings.json
  
## Development Plans
Sleep mode and a better auto fleet save are the next features I plan to write.

Also, a proper documentation about how to deal with settings.json should be written sooner or later.

As for translations, at the moment the bot is not really suitable for "production" and should be used only for testing and hacking purposes. If you are a dev, you can probably cope with my brokenish English. If and when this project becomes something bigger than what it currently is, I may reconsider this.

Feel free to give suggestions posting an Issue or joining the Discord chat.

## Building

I write and build TBot with Visual Studio 2019 Community Edition, thus .NET 5 SDK is enough for command line compilation.

Releases are automated by GitHub Actions, take a look at the [workflows](https://github.com/ogame-tbot/TBot/tree/master/.github/workflows) if you are interested on the build process.
  
## Portability

TBot is currently developed and mantained for Windows 64bit, Windows 32bit, Linux x86_64, MacOS 64bit, Linux ARMv7 and Linux ARM64.

MacOS ARM will be supported when .NET 6 will be officially released
