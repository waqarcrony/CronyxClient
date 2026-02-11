@echo off
setlocal
set SVC_NAME=Cronyx
set EXE_PATH="%~dp0\Cronyx.exe"

sc create %SVC_NAME% binPath= %EXE_PATH% start= auto
sc description %SVC_NAME% "Background Cronyx service"
sc start %SVC_NAME%