
# CoreKeepersWorkshop

[![Build status](https://ci.appveyor.com/api/projects/status/louecd5l7lvp4hvo?svg=true)](https://ci.appveyor.com/project/RussDev7/corekeepersworkshop) [![GitHub Version](https://img.shields.io/github/tag/RussDev7/CoreKeepersWorkshop?label=GitHub)](https://github.com/RussDev7/CoreKeepersWorkshop) [![Contributors](https://img.shields.io/github/contributors/RussDev7/CoreKeepersWorkshop)](https://github.com/RussDev7/CoreKeepersWorkshop)

![thumbCKWNew](https://user-images.githubusercontent.com/33048298/190870510-69e52e39-fd39-4fea-a705-fdb44dac93df.png)

CoreKeepersWorkshop - The simple GUI inventory editor for Core Keeper. This app further lets you edit basic player features such as name.

## Features
 - [x] Inventory Editor. 
 - [x] Name changer. 
 - [x] Export / Import players. 
 - [x] Minimization support. 
 - [x] Unknown ID debugger. 
 - [x] Modded support.
 - [x] No Game File Replacement. 
 - [x] Future proof resiliency.
 
## How It Works / How Do I Use It?
Due to how Core Keeper is as a unity game, the devolopers have protected their source and mono files. To further aid in the no support for modding this game includes, they also have used memory protection techniques to scramble regions of memory to prevent address pointers. So using some clevor AoB (array of bytes) scanning, we can get around this protection due to some unspecified exploits that exists within their protection.

Included in this application's directory is a file named `build.bat`. This file can be used to compile the applications source code from a single click. You can also download an already compiled version from the releases. After building, your application can be found in the releases directory.

After launching, you will need to put some torches in both your first and last inventory slots (don't use added storage slots from bags, exc).

![start2](https://user-images.githubusercontent.com/33048298/190875320-ac4f5496-2b0f-480c-b7f4-0f7179d2d423.png)

The features for the application is as follows:
![InventoryAbout3](https://user-images.githubusercontent.com/33048298/202617651-b8d8ccfb-4e39-45f9-9fa4-f2a66dcb2797.png)
![PlayerAbout](https://user-images.githubusercontent.com/33048298/190877329-170e2c23-2271-4de6-8d4a-e4fafa9f5b43.png)
![EditorAbout](https://user-images.githubusercontent.com/33048298/190877333-99fda451-1cfe-4962-941b-e8bb7d54a30d.PNG)

## Explanation On Item Variants
Variants are mostly used curently for food items only. In order to find a variant for an item, you first need to determine the base item ID. All items regardless of variant all have a base ID. There is not a unique ID assosiated with each item variant, only the base ID. Refer to the formula below to understand how these are built.

Variant-ID: `xxxxyyyy`\
`x` = ID of first ingredient\
`y` = Second ingredient ID.

![HowVariantsWork](https://user-images.githubusercontent.com/33048298/203685712-03d340d2-ef94-41ad-bd7e-6c4fa8088a1e.png)

## How Does This Mod Work With Memory?
You may be asking yourself, how does this work without even replacing files in my game? To answer as simply as possible, you first need to understand what memory addresses are. Every game and every program on your PC has an "address" which is a physical space in your memory (RAM or CPU registers). This mod scans for these unique memory patterns by their values to to find values like an items ID, amount, variant and pretty much whatever value you can imagine. All text, numbers, and even the positions of enemies are stored in this memory; the only issue is finding it. Core Keeper is a very special game as there is no static way to get the game value from an address each time. Each time you reload your game, even your world, these addresses will no longer point to the values they did before. This game scrambles its memory to protect from people abusing this method so it's quite annoying. This mod uses some clever techniques to overcome this obstacle by something called AoB scanning. This allows us to always find the addresses by a unique pattern in memory, the torches.

To first find the addresses, tools such as cheat engine or other memory scanning software’s are used. Since we cannot save pointers example, `corek.exe +AB12 +AB12 +AB12` etc., you can find unique arrays of bytes that stay the same each time (AoB scanning). Doing a bunch of math and navigating, you can start to build static pointers based off how the memory arranges bytes. This is why you need to have a torch in the first and last slot. Once this array is found, you can start coding around it and adding onto it. Making functions exc. CoreKeeper requires allot more work even so with the addition of memory protection method such as fake honeypot pointers. These addresses are duplicates of one desired value such as the amount of an item. When editing a fake address, there is a chance the memory will re-scramble and you will have to start all over to find once again for another opportunity to find it once again. Below you can see how the values are stored in memory for the inventory.

![HowMemWorks](https://user-images.githubusercontent.com/33048298/203687176-72f493b0-0186-4cea-a5f9-16c3555efb20.png)

## How To Add New / Modded Items
This editor has full support for adding modded items or future game release content. The assets folder contains all the inventory editor's textures. Each item category is divided by it's own subdirectory folder. These folders can be found within the `.\assets\inventory\` directory. From here you can add new assets. The editor uses a very special type of json'ing to allow for adding future content without the need to manually update the application for each update. If the in-game items do not contain an asset texture within this editor, they will show up as a questionmark on the main form. 

![01](https://user-images.githubusercontent.com/33048298/190876339-6153add9-0558-4759-969f-a14f2dddbe7f.PNG)

Below is a chart to show off filename system for images:

![ItemSupportv2](https://user-images.githubusercontent.com/33048298/190885823-8f0b7a7a-0abd-4f45-b11a-76d67c52f466.png)

Here is the list of all the available categories:

 - Tools
 - PlaceableItems
 - Nature
 - Materials
 - Special
 - MobItems
 - BaseBuilding
 - Treasures
 - ElectroMechanics
 - PlantsSeeds
 - Armors
 - Accessories
 - Weapons
 - Consumables
 - Seasonal

## How To Add Custom Skins
Adding background UI skins is now supported. You can add custom skins to the `\assets\UI\` directory by any name. These images must be 695x288. It's very important you keep the inventory boxes where they are, these do not move with the skins. You can cycle through the skins using the "Change Skin" tab-button at the top. These will save when you close and appear when re-opened.

![CK-UISkins](https://user-images.githubusercontent.com/33048298/193999717-3ecc128e-ea28-4367-a8cb-c0081ac011df.gif)

## Download

- [GitHub Releases](https://github.com/RussDev7/CoreKeepersWorkshop/releases)
- [Skin Template](https://raw.githubusercontent.com/RussDev7/CoreKeepersWorkshop/main/src/CoreKeeperInventoryEditor/images/SkinTemplate.png)

## Support & Credits

- [Memory.dll](https://github.com/erfg12/memory.dll)
- [Json.net](https://www.newtonsoft.com/json)
- [UI-Background](https://discord.com/users/229227672121769984/)
- [CK Asset Wiki](https://corekeeper.wikizet.com/en/Core_Keeper_Wiki)
- [Donations](https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=imthedude030@gmail.com&lc=US&item_name=Donation&currency_code=USD&bn=PP%2dDonationsBF)
