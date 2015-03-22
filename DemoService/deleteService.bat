@echo off
sc stop DemoService
%windir%\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /u bin\DemoService.exe
