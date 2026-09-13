# AimLabsUltimateUnlocker (for AimLabs 2.0)
<img width="1145" height="99" alt="image" src="https://github.com/user-attachments/assets/732bb829-13b5-462c-8352-3f5c267c06d2" />
A Unity mod for AimLabs that enables AimLabs Pro (previously AimLabs+) subscription for free, unlocks paid skins, and more.

Players shouldn't have to pay just to improve.

Working on AimLabs version 2.0.2-8213. May work for newer versions.

Only usable with MelonLoader 0.7.3 or newer.

[Link to the MelonLoader Installer](https://github.com/LavaGang/MelonLoader/releases/latest)

## NOTICE - READ FIRST
I am not responsible for any punishments placed on your AimLabs account for using this mod.

With the new update, new anti-cheats have been created.

While this mod only changes things on the client, it should be used with caution.

## Features
Any AimLabs features that requires payment has been modified to work for free.

Also included with the mod are some unreleased features/content currently in the 2.0 update.
### What works
* AimLabs Pro subscription status
* Every item unlocked (includes unreleased items)
	* You can now view these at (Profile) -> (Customize) OR (Inventory)
	* Some categories are currently **client-sided**, meaning only you can see them. This includes:
   		* Profile pictures
     	* Banners
      	* Any item that is [unreleased/hidden](unreleased_and_hidden_content_2.0.2.txt) (see section below)  
	* If you believe there is an item you want and it's missing, create an Issue explaining so.
* Valorant Training Center Freeplay and Analysis
* Valorant Training Center modifiers (Crosshair Placement, Movement Accuracy, Peek Efficiency)
* Shot Vision
* Sensitivity Finder
* Pro Training Programs and Guides
* Task Tuning
* Pro Metrics (after finishing a task)
* Adaptive Tasks
* 50% off when buying Credits
* AimLabs Pro Chat


### What partially works
* Valorant Training Center "Run It Back"
	* Doesn't work as the only way to get replays is to go to the Valorant Stats Center (external website).
 	* Currently not possible to fake subscription on said website.
  	* Only way this feature will work is if you were already subscribed to AimLabs Pro and have exported replays before your subscription has expired.

### What doesn't work
* Discovery AI
	* Errors out due to unauthorized access.
 * Coach Analysis (used to work before 2.0)
 	* Errors out due to "taking too long". 
 * Pro Metrics AI Chatbot (used to work before 2.0)
 	* Errors out due to "taking too long".

## Unreleased Content
I took the time to add any unreleased content that the game offers as of version 2.0.2-8213.

Keep in mind that these items are extracted and manually put back into the game, therefore some items may not be visible to anyone but you.

You can read the [list of unreleased/hidden content](unreleased_and_hidden_content_2.0.2.txt) located in the repository files to see what may be affected.

Some unreleased/hidden items may not contain any models, icons, and/or descriptions. These are intended and is not a bug caused by the mod.


## Other features
A notification system that alerts the player anything about the mod.
<img width="557" height="130" alt="image" src="https://github.com/user-attachments/assets/366b076f-8f5f-4f72-9265-709a9aa6bc69" />

## Installation
1. Download the [MelonLoader installer](https://github.com/LavaGang/MelonLoader/releases/tag/v0.7.1) and patch AimLabs with MelonLoader (0.7.1 or latest version works).
2. Download the [latest release of the mod](https://github.com/accountrev/AimLabsUltimateUnlocker/releases/latest) for MelonLoader.
3. Once AimLabs has been patched with MelonLoader, run it once to generate libraries. Wait for the menus to fully start up.
4. Close AimLabs after main menu loads.
5. Open AimLabs directory, and copy the mod over to the Mods folder.
6. Run AimLabs and verify mod works by:
	1. Seeing the notification on the bottom left that the mod has successfully loaded.
 	2. Seeing the orange "AimLabsUltimateUnlocker" tab at the topbar.

## Building
Building requires the game to be patched with MelonLoader and have their library .dlls generated for usage in the code.

This is usually done by running the game once for them to generate.

Everything is configured for you to use and build except AimLabsDirectory.props, where you will need to point the path to your game.

Once done, build by running **dotnet build**.

## Images
<img width="1920" height="899" alt="image" src="https://github.com/user-attachments/assets/df681fda-d972-42c9-b75b-adee36d62436" />
<img width="1920" height="1080" alt="image" src="https://github.com/user-attachments/assets/c006a63d-572b-4314-8c6a-084b95b3e11f" />
<img width="1170" height="654" alt="image" src="https://github.com/user-attachments/assets/43f5a24b-b580-4e37-aeee-72ea931935d9" />
<img width="1170" height="654" alt="image" src="https://github.com/user-attachments/assets/eb3ec55d-6e56-40eb-862a-dd6a4f57294d" />
<img height="650" alt="image" src="https://github.com/user-attachments/assets/2a5f7a0e-c2de-4fbf-8044-027585badd44" /> <img height="650" alt="image" src="https://github.com/user-attachments/assets/046176bb-f7a2-466a-82db-f269df185786" /> <img height="650" alt="image" src="https://github.com/user-attachments/assets/f5da9e05-50a2-4b38-98cc-9f128bb4b238" />
