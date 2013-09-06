Final-Bomber
============

![Final-Bomber](http://finalbomber.free.fr/img/final-bomber_image4.jpg)

Description
===========

Final-Bomber is a Dyna Blaster (Bomberman) remake developed in C# with XNA 4.0.
For now, it's only possible to play local games with a total of 5 players (humans or bots).

How to compile it ?
===================
I've just tested it on Windows 7, so I don't know if it will works on another platform. 
If you are on a unix-based OS or on Mac OS, look for Monogame.
If you are on Windows 7 or greater, you need to install **Microsoft XNA Game Studio 4.0** (http://www.microsoft.com/en-us/download/details.aspx?id=23714).
You have to get a version of Microsoft Visual Studio 2010 (Express version is enough => http://www.microsoft.com/visualstudio/eng/downloads#d-2010-express).
If you use Microsoft Visual Studio 2012, you can't directly use XNA and you still need to install VS2010. 
To use XNA 4.0 in VS2012, an explanation is available in the wiki here: https://github.com/Noxalus/Final-Bomber/wiki/How-to-install-XNA-4.0-on-VS2012

Current features
================
* The game can be played in fullscreen or windowed
* Resolution can be changed
* All players can reassign their inputs
* Teleporters can be enabled
* Arrows can be enabled
* It's possible to change the wall percentage
* It's possible to change the probability to have a bonus in a wall
* Sudden death can be activated
* The sudden death's speed can be changed
* The sudden death's beginning can be changed
* Players can choose wich bonuses will be included in the game
* Players can push the bomb

TODO List
=========
* Add the Xbox controller support
* Add LAN and online implementation (Lidgren => https://code.google.com/p/lidgren-network-gen3/)
* Redesign menus (XUI => http://xui.codeplex.com/releases/view/80711)
* Improve the AI behavior
* Add a level editor (HTML5 ?)
* Add a CTF (capture the flag) mode with classes (engineer that can build wall, etc...)
* Multiplatform (Monogame)
* Replay to save (to see again a finished game)
* Boss scripts
* Resources extension
* Split screen for local multiplayer ?

Videos
======
I've uploaded some videos to show the AI progress:
* http://www.youtube.com/watch?v=j8d18H-NKrU
* http://www.youtube.com/watch?v=WCqIGvNPqBE
* http://www.youtube.com/watch?v=tWfutYKStsU
* http://www.youtube.com/watch?v=LH1Lfwke_pg
* http://www.youtube.com/watch?v=F6pJpJsZ7GI
