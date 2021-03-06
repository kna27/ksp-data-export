# KSP Data Export

## Introduction

**[Forum Thread](https://forum.kerbalspaceprogram.com/index.php?/topic/201967-111x-export-flight-data-to-a-csv-file-mod)**

Ever wanted to view your KSP flight data in a graph? Well, this mod allows you to do that! Simply choose which values you would like to log, and all of your flight data will get exported to a CSV file!

You can then open this CSV file in a program such as Excel and make as many charts and graphs as you desire!

## Reporting Bugs

If you encounter any bugs or have any suggestions, report them at https://github.com/kna27/ksp-data-export/issues.

## Installation
This mod works on Windows, Linux, and Mac.

This mod is now available on [CKAN](https://github.com/KSP-CKAN/CKAN), [SpaceDock](https://spacedock.info/mod/2711/KSP%20Data%20Export), and [CurseForge](https://www.curseforge.com/kerbal/ksp-mods/data-export).

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

To get the references in the csproj to work, all you need to do is add an environment variable on your machine. The process to do this is different on every OS. Make an env var with the name `KSP`, with a value of the path to `Path/To/Kerbal Space Program/KSP_Data/Managed`. This should be the full path. On Windows, for me it is `C:\Program Files (x86)\Steam\steamapps\common\Kerbal Space Program\KSP_x64_Data\Managed`. And on Linux, it is `/.steam/debian-installation/steamapps/common/Kerbal Space Program/KSP_Data/Managed`. Figure out what it is for you, and then you should be good to go.
