@echo off
chcp 65001 > nul

set mypath=%~dp0
set cur_path=%mypath:~0,-1%

set company=NtreevSoft
set configuration=Release
set copyright="Copyright © Ntreev Soft 2018"
set majorVersion=3
set minorVersion=6
set versionPath=%cur_path%\version.txt 

"%NSDEPLOY%\version-builder.exe" "%majorVersion%.%minorVersion%" -b -r -q --output-path "%versionPath%"
set result=%errorlevel% 
if not %result%==0 goto fail

echo %version%
set /p version=<"%versionPath%"
"%NSDEPLOY%\version-builder.exe" %version% --output-path "%cur_path%\common\Ntreev.Crema.AssemblyInfo\AssemblyInfo.cs" --configuration %configuration% --company %company% --copyright %copyright%
set result=%errorlevel% 
if not %result%==0 goto fail

goto end

:fail
echo Version generation failed.
pause 
exit /b 1 
 
:end 
exit /b 0 
