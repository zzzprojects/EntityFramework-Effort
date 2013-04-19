@echo OFF

set package=
for %%x in (.\Packages\*.nupkg) do if not defined package set "package=%%x"

if not defined package (
    echo NuGet package has not been created!
    exit /b -1
)

set /p apikey=Enter NuGet.org API key:
..\Source\.nuget\NuGet.exe setApiKey %apikey%
..\Source\.nuget\NuGet.exe push ".\%package%"
