# Elite Observatory **Core**
Tool for reading/monitoring Elite Dangerous journals for interesting objects. Successor to the original Elite Observatory, rewritten from scratch using .NET 5.0 and AvaloniaUI.

## How To Use
Observatory Core differs from the originsl Elite Observatory in that it is a fully generic reader for the Elite Dangerous journal files, passing those events along to plugins to do the actual work.

For specifics on what each plugin does, please refer to their specific github repositories:
* [Explorer](https://github.com/Xjph/ObservatoryExplorer)
* [Botanist](https://github.com/Xjph/ObservatoryBotanist)

## Prerequisites for use
.NET 5, and by extension one of its [supported OSes](https://github.com/dotnet/core/blob/main/release-notes/5.0/5.0-supported-os.md).

## Prerequisites for building
C# 9.0, .NET 5.0, [AvaloniaUI 0.10.3](https://github.com/AvaloniaUI/Avalonia).
