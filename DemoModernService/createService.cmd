@echo off
@rem Intended to run from the same directory (e.g. bin); build should copy to bin.
openfiles.exe 1>nul 2>&1
if not %errorlevel% equ 0 goto :fail
sc create "DemoModernService" binpath= "%~dp0DemoModernService.exe" displayname= "murrayju.ProcessExtensions Modern .NET Demo"
sc start "DemoModernService"
goto:eof
:fail
echo:
echo Failed. Run as Administrator.
echo:
:eof
