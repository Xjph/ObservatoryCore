dotnet build ./ObservatoryFramework/ObservatoryFramework.csproj %*
dotnet build ./ObservatoryExplorer/ObservatoryExplorer.csproj %*
dotnet build ./ObservatoryBotanist/ObservatoryBotanist.csproj %*
IF EXIST ..\NetCoreAudio\NetCoreAudio\NetCoreAudio.csproj (
    dotnet build ../NetCoreAudio/NetCoreAudio/NetCoreAudio.csproj -c Release
    dotnet build ./ObservatoryHerald/ObservatoryHerald.csproj %* 
)
