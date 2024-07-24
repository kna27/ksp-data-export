# KSP Data Export

## Introduction
**[Forum Thread](https://forum.kerbalspaceprogram.com/index.php?/topic/201967-111x-export-flight-data-to-a-csv-file-mod)**

Ever wanted to view your KSP flight data in a graph? Well, this mod allows you to do that! Simply choose which values you would like to log, and all of your flight data will get exported to a CSV file!

You can then open this CSV file in a program such as Excel and make as many charts and graphs as you desire!

## Reporting Bugs
If you encounter any bugs or have any suggestions, report them at https://github.com/kna27/ksp-data-export/issues.

## Installation
This mod works on Windows, MacOS, and Linux.

This mod is available on [CKAN](https://github.com/KSP-CKAN/CKAN), [SpaceDock](https://spacedock.info/mod/2711/KSP%20Data%20Export), and [CurseForge](https://www.curseforge.com/kerbal/ksp-mods/data-export).

1. Download GameData.zip from the [latest release here](https://github.com/kna27/ksp-data-export/releases/latest)
2. Copy the DataExport folder to `YourKSPInstallDirectory/Kerbal Space Program/GameData`

Your directory should look like: `YourKSPInstallDirectory/Kerbal Space Program/GameData/DataExport` if done correctly.

## How to use
Click the mod's icon in flight to view the GUI for the mod.

This video goes in-depth on how to use the mod:

[![Help Video](https://img.youtube.com/vi/3s2SctniVLM/0.jpg)](https://www.youtube.com/watch?v=3s2SctniVLM)

## Support
[Email me with any questions or comments](mailto:krisharora27@gmail.com)

## Contributing
To get the references in the `.csproj` file to work, you need to add an environment variable `KSP` with the value set to the full path of `Kerbal Space Program/KSP_Data/Managed`. For example:
- Windows: `C:\Program Files (x86)\Steam\steamapps\common\Kerbal Space Program\KSP_x64_Data\Managed`
- Linux: `/.steam/debian-installation/steamapps/common/Kerbal Space Program/KSP_Data/Managed`
- MacOS: `/Users/<username>/Library/Application Support/Steam/steamapps/common/Kerbal Space Program/KSP.app/Contents/Resources/Data/Managed`
