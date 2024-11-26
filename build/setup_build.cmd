cd /d %~dp0
dotnet restore ..\src\Fischless.Relauncher\Fischless.Relauncher.csproj
dotnet publish ..\src\Fischless.Relauncher\Fischless.Relauncher.csproj -c Release -p:PublishProfile=FolderProfile
7z a publish.7z ..\src\Fischless.Relauncher\bin\Release\net9.0-windows10.0.22621.0\publish\win-x64\* -t7z -mx=5 -mf=BCJ2 -r -y
"C:\Program Files\MicaSetup\makemica" micasetup.json
@pause
