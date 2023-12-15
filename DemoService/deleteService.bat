@echo off
@rem Intended to run from the same directory (e.g. bin); build should copy to bin.
sc stop DemoService
%windir%\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /u .\DemoService.exe
