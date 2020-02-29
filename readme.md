# kuvuBot
[![GitHub issues](https://img.shields.io/github/issues/kuvuBot/kuvuBot.svg)](https://github.com/kuvuBot/kuvuBot/issues)
[![GitHub forks](https://img.shields.io/github/forks/kuvuBot/kuvuBot.svg)](https://github.com/kuvuBot/kuvuBot/network)
[![GitHub stars](https://img.shields.io/github/stars/kuvuBot/kuvuBot.svg)](https://github.com/kuvuBot/kuvuBot/stargazers)
[![Discord server](https://discordapp.com/api/guilds/257599205693063168/widget.png?style=shield)](https://discord.gg/WhCjqqj)
[![Discord Bots](https://discordbots.org/api/widget/status/205965341241638912.svg)](https://discordbots.org/bot/205965341241638912)
[![Crowdin](https://d322cqt584bo4o.cloudfront.net/kuvubot/localized.svg)](https://crowdin.kuvubot.xyz/project/kuvubot)

## Links
**Website:** https://kuvuBot.xyz  
**Discord server:** https://discord.gg/WhCjqqj  
**Bot invite:** http://bit.ly/kuvuBotAdd  
**Crowdin:** https://crowdin.kuvubot.xyz/

## Requirements to run kuvuBot
* .NET Core (min. version 3.0)
* MySQL server 

## Running
There are two ways to run kuvuBot:
1. **With website:**
    * Start kuvuBot from kuvuBot.Panel project  
    There are two modes of webserver and you have to specify mode in start command.
        * Production (which runs on port 80 and has debug enabled)
        * Dev (runs on port 5000)  
2. **Without website:**
    * Start kuvuBot from kuvuBot project
    
Keep in mind that both of these projects have **separate** config files (each in its own directory).
Respectively: kuvuBot/config.json and kuvuBot.Panel/config.json 

If you want to use Badosz API module, you have to build BadoszApiModule project and put outpud dll file in kuvuBot/modules or kuvuBot.Panel/modules (depends how you start your bot.)