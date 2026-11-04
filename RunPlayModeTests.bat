@echo off
REM ============================================================
REM  Chay toan bo PlayMode test cua Dungeon Escape 2D khong can mo Unity.
REM  - Dat file nay o thu muc goc project (cung cap voi Assets).
REM  - DONG Unity Editor truoc khi chay.
REM  - Sua dong UNITY ben duoi neu Unity cai o cho khac.
REM ============================================================

set UNITY="C:\Program Files\Unity\Hub\Editor\6000.3.6f1\Editor\Unity.exe"
set PROJECT=%~dp0.
set OUT=%~dp0TestResults

if not exist %UNITY% (
    echo Khong tim thay Unity tai %UNITY%
    echo Mo Unity Hub ^> Installs ^> banh rang ^> Show in Explorer de lay dung duong dan.
    pause
    exit /b 1
)

if not exist "%OUT%" mkdir "%OUT%"

echo Dang chay PlayMode test... (mat vai phut)
%UNITY% -batchmode -projectPath "%PROJECT%" -runTests -testPlatform PlayMode ^
    -testResults "%OUT%\playmode-results.xml" -logFile "%OUT%\unity.log"

set CODE=%ERRORLEVEL%
echo.
if %CODE%==0 echo KET QUA: TAT CA TEST DAT
if %CODE%==2 echo KET QUA: CO TEST KHONG DAT - xem %OUT%\playmode-results.xml
if %CODE%==3 echo KET QUA: LOI KHI CHAY - xem %OUT%\unity.log
echo.
pause
exit /b %CODE%
