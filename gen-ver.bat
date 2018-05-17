@echo off
powershell -executionpolicy remotesigned -File gen-ver.ps1
if not %errorlevel% == 0 pause
