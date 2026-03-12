@echo off
chcp 65001 >nul
echo ================================================
echo   OCIF Thumbnail Provider — Удаление
echo ================================================
echo.

net session >nul 2>&1
if %errorlevel% neq 0 (
    echo [ОШИБКА] Запустите от имени администратора!
    pause
    exit /b 1
)

set DLL=%~dp0OcifThumb.dll
set REGASM=C:\Windows\Microsoft.NET\Framework64\v4.0.30319\regasm.exe
set GUID={74690D5E-36B5-3E0A-B181-BDA910DD749F}

echo [1/3] Отмена регистрации COM сборки...
"%REGASM%" "%DLL%" /unregister >nul 2>&1
echo       OK

echo [2/3] Удаление ключей реестра...
reg delete "HKCR\.pic\ShellEx\{e357fccd-a995-4576-b01f-234630154e96}" /f >nul 2>&1
reg delete "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Shell Extensions\Approved" /v "%GUID%" /f >nul 2>&1
echo       OK

echo [3/3] Перезапуск Проводника...
taskkill /f /im explorer.exe >nul 2>&1
del /f /q "%localappdata%\Microsoft\Windows\Explorer\thumbcache_*.db" >nul 2>&1
start explorer.exe
echo       OK

echo.
echo ================================================
echo   Удалено!
echo ================================================
pause
