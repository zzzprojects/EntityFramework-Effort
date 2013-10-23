@echo OFF

for %%x in (.\Packages\*.nupkg) do echo %%x

set /p apikey=Enter NuGet.org API key:
..\Source\.nuget\NuGet.exe setApiKey %apikey%

for %%x in (.\Packages\*.nupkg) do ..\Source\.nuget\NuGet.exe push ".\%%x"
