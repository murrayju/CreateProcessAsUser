@echo off
@rem Intended to run from the same directory (e.g. bin); build should copy to bin.
openfiles.exe 1>nul 2>&1
if not %errorlevel% equ 0 goto :fail
sc stop "DemoModernService"
sc delete "DemoModernService"
goto:eof
:fail
echo:
echo Failed. Run as Administrator.
echo:
:eof