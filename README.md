# ClashRoyale-Clone
In this project, I developed a Clash Royale-like clone. It works in a multiplayer format where four units are on our side and four units are on the opponent's side.

Units

Range
Melee
Healer
Aerial (Dragon-Bird-Eagle, etc.)
Using Photon Bolt technology, I created a structure for multiplayer matches between two clients that connect to a server in a server-authorized system. 
Units with different mana-based costs, which can be left in their own areas, can be regenerated on the bottom. Character stats were balanced,
and I used Addressable cloud content delivery technology as a hobby project to improve myself. When the section is finished and the second side is switched,
a popup is given to the user asking if they want to download the 5.4 MB scene over the cloud using the Addressable system. 
The next scene is loaded after receiving input from the other side. The game ends when towers and bases are lost, and the next scene is loaded.

Technologies used:

Photon Bolt for networking
Addressables Cloud Content Delivery
Navmesh AI
Scriptable objects for unit stats
Singleton
Dotween
Observer Pattern
Event based comms.

Assets

Polygon Vikings Assetpack
Epic Toon Fx
Gui mobile pro kit

It was developed with performance issues in mind, and a combat mechanism was provided using distance check methods without collision.
I tested it on mobile platforms and provided a multiplayer gameplay.

During testing, the average frame per second was around 60-75 on several phones.
