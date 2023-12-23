@echo off
cls

:MAINMENU
echo *********************_Main_Menu_*************************
echo *
echo *   Option 0 : Setup
echo *   Option 1 : Verify Files and Paths
echo *   Option 2 : Disable Music Overhaul
echo *   Option 3 : Navigate to Launcher Menu 
echo *   (DO THIS TO PLAY THE GAME WITHOUT WHEEL CONFIG OR MUSIC OVERHAUL)
echo *
echo *********************************************************

set /p ans="Enter Option Here: "
if "%ans%"=="0" goto setup
if "%ans%"=="1" goto verify
if "%ans%"=="2" goto NoMusic
if "%ans%"=="3" goto LAUNCHER
goto MAINMENU

:setup
start https://ts.thrustmaster.com/download/accessories/pc/unified_drivers/2018_FFD_2.exe
goto LAUNCHER

:verify
echo Verifying files...
if exist "C:\NFSW\Xpadder.exe" (
    echo Xpadder.exe found.
) else (
    echo Xpadder.exe not found.
)

if exist "C:\NFSW\Thrustmaster T80 -NFS World.xpaddercontroller" (
    echo Thrustmaster T80 - NFS World.xpaddercontroller found.
) else (
    echo Thrustmaster T80 - NFS World.xpaddercontroller not found.
)

if exist "C:\NFSW\GameLauncher.exe" (
    echo GameLauncher.exe found.
) else (
    echo GameLauncher.exe not found.
)
pause
goto MAINMENU

:NoMusic
echo Music Overhaul disabled.
goto MAINMENU

:NoMusicLauncher
echo ********************************************************************************************
echo * Welcome to the Need for Speed Underground 2 PS3-PS5 Thrustmaster T80 Setup!              *
echo * We're excited to have you using this new setup. Thank you so much for choosing our fork! *
echo ********************************************************************************************

timeout /t 10

echo We will load your game shortly. Please wait patiently.
timeout /t 10

:: Flushing DNS Resolver Cache
ipconfig /flushdns

:: Opening T80 Wheel Configuration with Admin Privileges
start "" "C:\NFSW\Xpadder.exe" "C:\NFSW\Thrustmaster T80 -NFS World.xpaddercontroller"

:: Opening Game Client
start "" "C:\NFSW\GameLauncher.exe"

timeout /t 5
goto MAINMENU

:LAUNCHER
echo ********************************************************************************************
echo * Welcome to the Need for Speed Underground 2 PS3-PS5 Thrustmaster T80 Setup!              *
echo * We're excited to have you using this new setup. Thank you so much for choosing our fork! *
echo ********************************************************************************************

timeout /t 10

echo We will load your game shortly. Please wait patiently.
timeout /t 10

:: Flushing DNS Resolver Cache
ipconfig /flushdns

:: Opening T80 Wheel Configuration with Admin Privileges
start "" "C:\NFSW\Xpadder.exe" "C:\NFSW\Thrustmaster T80 -NFS World.xpaddercontroller"

:: Opening Game Client
start "" "C:\NFSW\GameLauncher.exe"

:: Waiting for 1 minute and 35 seconds
timeout /t 95

:: Playing music (via NFS inspired YouTube playlist)
start https://www.youtube.com/watch?v=fYh7lp8w0S4&list=PL85wXcWAp0HW5_6pzroq5n8E8_XD5WNDU&pp=gAQBiAQB8AUB
