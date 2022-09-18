
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
Due to how Core Keeper is as a unity game, the devolopers have protected there source and mono files. To further aid in the no support for modding this game includes, they also have used memory protection tequnies to scramble regions of memory to prevent address pointers. So using some clevor AoB (array of bytes) scanning, we can get around this protection due to some unspecified exploits that exists within their protection.

Included in this application's directory is a file named `build.bat`. This file can be used to compile the applications source code from a single click. You can also download an already compiled version from the releases. After building, your application can be found in the releases directory.

After launching, you will need to put some torches in both your first and last inventory slots (don't use added storage slots from bags, exc).

![start2](https://user-images.githubusercontent.com/33048298/190875320-ac4f5496-2b0f-480c-b7f4-0f7179d2d423.png)

The features for the application is as follows:
![InventoryAbout](https://user-images.githubusercontent.com/33048298/190877327-7828e908-430c-45e4-9546-9c346f642de3.png)
![PlayerAbout](https://user-images.githubusercontent.com/33048298/190877329-170e2c23-2271-4de6-8d4a-e4fafa9f5b43.png)
![EditorAbout](https://user-images.githubusercontent.com/33048298/190877333-99fda451-1cfe-4962-941b-e8bb7d54a30d.PNG)


## How To Add New / Modded Items
This editor has full support for adding modded items or future game release content. The assets folder for all the inventory editors textures can be found within the `.\assets\inventory\` directory. From here you can add new assets. The editor uses a very special type of json'ing to allow for adding future content without the need to manually update the application for each update. If textures do not contain an asset texture, they will show up as a questionmark. 

![01](https://user-images.githubusercontent.com/33048298/190876339-6153add9-0558-4759-969f-a14f2dddbe7f.PNG)

Bellow is a chart to show off filename system for images:

![ItemSupport](https://user-images.githubusercontent.com/33048298/190875665-9977b004-37f9-455f-bb4b-ae3834152468.png)

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

## Download

- [GitHub Releases](https://github.com/RussDev7/CoreKeepersWorkshop/releases)
