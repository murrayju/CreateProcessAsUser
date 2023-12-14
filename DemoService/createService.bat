@echo off
@rem Intended to run from the same directory (e.g. bin); build should copy to bin.
%windir%\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe .\DemoService.exe
sc start DemoService
