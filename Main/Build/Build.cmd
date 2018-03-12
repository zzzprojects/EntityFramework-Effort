@echo OFF
for /F "tokens=2* delims=	 " %%A in ('reg query "HKLM\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Client" /v Version') do set fw_version=%%B
for /f "delims=. tokens=1-3" %%a in ("%fw_version%") do (
  set fw_version_major=%%a
  set fw_version_minor=%%b
  set fw_version_build=%%c
)

set fw_version_ok=
if %fw_version_major% geq 5 ( set fw_version_ok=1 )
if %fw_version_major% equ 4 ( if %fw_version_minor% geq 5 ( set fw_version_ok=1 ))

if not defined fw_version_ok (
    echo .NET Framework 4.5 is not installed! 
    exit /b -1
)

set msbuild=%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\msbuild

if not exist %msbuild% (
    echo MsBuild was not found!
    exit /b -2
)

set EnableNugetPackageRestore=true
%msbuild% Build.targets /v:minimal /maxcpucount /nodeReuse:false %*

timeout /t 2