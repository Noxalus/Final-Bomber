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

If you are on a unix-based OS, look for Monogame.

If you are on Windows 7 or greater, you need to install **Microsoft XNA Game Studio 4.0** (http://www.microsoft.com/en-us/download/details.aspx?id=23714).

You have to get a version of Microsoft Visual Studio 2010 (Express version is enough => http://www.microsoft.com/visualstudio/eng/downloads#d-2010-express).

If you use Microsoft Visual Studio 2012, you can't directly use XNA and you still need to install VS2010. 

To use XNA 4.0 in VS2012, an explanation is available in the wiki here: https://github.com/Noxalus/Final-Bomber/wiki/How-to-install-XNA-4.0-on-VS2012 (same manipulation for VS2013).

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

* Add the Xbox controller support (with many controllers)
* Add LAN and online implementation (Lidgren => https://code.google.com/p/lidgren-network-gen3/)
* Redesign menus (XUI => http://xui.codeplex.com/releases/view/80711)
* Improve the AI behavior
* Add a level editor (HTML5 ?)
* Add a CTF (capture the flag) mode with classes (engineer that can build wall, etc...)
* Multiplatform (at least Linux and Mac support => Monogame)
* Replay to save (to see again a finished game)
* Boss scripts (LUA)
* Easy resources extension (window skins, sprites, etc)
* Split screen for local multiplayer ?
* Add logging for networking part (Log4Net => http://logging.apache.org/log4net/index.html)
* GUI (with Squid ? => http://www.ionstar.org/)

Videos
======
I've uploaded some videos to show the AI progress:
* http://www.youtube.com/watch?v=j8d18H-NKrU (first AI test)
* http://www.youtube.com/watch?v=WCqIGvNPqBE (second AI test)
* http://www.youtube.com/watch?v=tWfutYKStsU (third AI test)
* http://www.youtube.com/watch?v=LH1Lfwke_pg (first battle between 5 AI)
* http://www.youtube.com/watch?v=F6pJpJsZ7GI (second battle between 5 AI)
* http://www.youtube.com/watch?v=tu7cocmqBj8 (first networking test)
* http://www.youtube.com/watch?v=tSVHtYrGvsg (second networking test)
* http://www.youtube.com/watch?v=gkYElxHD3Q8 (simple 2D camera)
* http://www.youtube.com/watch?v=RP-6EF0NYJg (simple lobby screen)

Dev blog (french)
=================
You can find a dev blog with technical articles here: https://final-bomber.blogspot.com

Documentation
=============
A complete documentation generated with Doxygen is available here: http://noxalus.github.io/Final-Bomber

Old repository
==============
This project didn't start here, it was hosted by Google Code before.

You can found the old repository here: https://code.google.com/p/final-bomber/
