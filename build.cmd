@echo off
.nuget\nuget.exe restore
packages\psake.4.4.2\tools\psake.cmd "build.ps1"
