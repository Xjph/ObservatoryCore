# Elite Observatory *Core*
Tool for reading/monitoring Elite Dangerous journals for interesting objects. Successor to the original Elite Observatory.

### [Download Links Here](https://observatory.xjph.net/release)

## *IMPORTANT*
Observatory Core is a perpetual work-in-progress. New features are being developed and even the current ones may experience a degree of flux.

Noteworthy planned functionality includes, but is not limited to:
* Integration with Frontier's Companion API
* Data submission to IGAU
* Data submission to EDDN
* *And more...*

## How To Use
For detailed information about Elite Observatory and how to use it please see [the official docs page here](https://observatory.xjph.net/).

## Prerequisites for use
Ideally just a copy of Elite Dangerous. The installer should grab necessary prerequisites for you, but if for some reason that fails you can manually install the .NET 8 Desktop runtime.

The portable build has no prerequisites due to bundling the .NET runtime along with the program, though this does make the exe commensurately larger.

## Prerequisites for building
C# 9.0, .NET 8.0, and [ObservatoryFramework](https://observatory.xjph.net/framework).

### Linux

1. Install the .NET 8.0 sdk: [MS .NET Install scripts](https://dotnet.microsoft.com/en-us/download/dotnet/scripts)

2. Build the project using the Makefile (just run `make`)

3. Create a wineprefix (at least wine 10)

4. Run the winesetup (with WINEPREFIX env set): `make winesetup`

5. Finally, you can run the app: `wine bin/ObservatoryCore.exe`
