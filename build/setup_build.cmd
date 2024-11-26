cd /d %~dp0
dotnet restore ..\src\Fischless.Relauncher\Fischless.Relauncher.csproj
dotnet publish ..\src\Fischless.Relauncher\Fischless.Relauncher.csproj -c Release -p:PublishProfile=FolderProfile
@pause
