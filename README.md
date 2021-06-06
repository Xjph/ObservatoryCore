# Elite Observatory *Core*
Tool for reading/monitoring Elite Dangerous journals for interesting objects. Successor to the original Elite Observatory, rewritten from scratch using .NET 5.0 and AvaloniaUI.

## *IMPORTANT*
Observatory Core and it's associated plugins are currently in an alpha state and are neither feature-complete nor using a finalised UI. Over the coming weeks I will outline in detail what functionality is missing *but still planned*, and begin work on getting it to feature parity with the original Elite Observatory.

Omissions to current functionality include:
* Voice notification
* Checks based on surface materials (yes, there are checkboxes for them in the Explorer setting, they do not do anything)
* Having an application icon
* Integration with Frontier's Companion API
* Integration with Telegram
* Data submission to IGAU
* More that I'm just not thinking of right now

## How To Use
Observatory Core differs from the originsl Elite Observatory in that it is a fully generic reader for the Elite Dangerous journal files, passing those events along to plugins to do the actual work.

Observatory will attempt to locate your Elite Dangerous journal location in its default location, though other locations can be specified from the settings in the "Core" application tab. Due to the variety of possible system configurations this automatic detection does not function under Linux, so all Linux users will need to browse for the correct location.

The "Read All" button will read all journal files in the specified folder, handing their contents over to all loaded worker plugins. "Start Monitor" will begin watching files in the journal folder for changes, and pass new journal lines as they are created, as well as all changes to the status.json file, over to the worker plugins.

In addition to updating the content of their respective UI tabs, some workers can also send notifications, which will be distributed to all notifier plugins, as well as be passed to Observatory Core's native notification popup.

For specifics on what each plugin does, please refer to their specific github repositories:
* [Explorer](https://github.com/Xjph/ObservatoryExplorer)
* [Botanist](https://github.com/Xjph/ObservatoryBotanist)

For information on how to create a plugin, refer to the repository for [ObservatoryFramework](https://github.com/Xjph/ObservatoryFramework).

## Prerequisites for use
.NET 5, and by extension one of its [supported OSes](https://github.com/dotnet/core/blob/main/release-notes/5.0/5.0-supported-os.md).

## Prerequisites for building
C# 9.0, .NET 5.0, [AvaloniaUI 0.10.3](https://github.com/AvaloniaUI/Avalonia).
