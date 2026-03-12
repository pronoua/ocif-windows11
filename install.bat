@echo off
chcp 65001 >nul
echo ================================================
echo   OCIF Thumbnail Provider — Установка
echo ================================================
echo.

:: Проверка прав администратора
net session >nul 2>&1
if %errorlevel% neq 0 (
    echo [ОШИБКА] Запустите от имени администратора!
    echo Правая кнопка на install.bat -^> "Запуск от имени администратора"
    pause
    exit /b 1
)

set DLL=%~dp0OcifThumb.dll
set REGASM=C:\Windows\Microsoft.NET\Framework64\v4.0.30319\regasm.exe
set GUID={74690D5E-36B5-3E0A-B181-BDA910DD749F}

if not exist "%DLL%" (
    echo [ОШИБКА] OcifThumb.dll не найден рядом с install.bat
    pause
    exit /b 1
)

echo [1/4] Регистрация COM сборки...
"%REGASM%" "%DLL%" /codebase >nul 2>&1
if %errorlevel% neq 0 (
    echo [ОШИБКА] Не удалось зарегистрировать DLL
    pause
    exit /b 1
)
echo       OK

echo [2/4] Регистрация Shell Extension для .pic...
reg add "HKCR\.pic\ShellEx\{e357fccd-a995-4576-b01f-234630154e96}" /ve /d "%GUID%" /f >nul
echo       OK

echo [3/4] Добавление в список разрешённых расширений...
reg add "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Shell Extensions\Approved" /v "%GUID%" /d "OCIF Thumbnail Provider" /f >nul
echo       OK

echo [4/4] Перезапуск Проводника...
taskkill /f /im explorer.exe >nul 2>&1
del /f /q "%localappdata%\Microsoft\Windows\Explorer\thumbcache_*.db" >nul 2>&1
start explorer.exe
echo       OK

echo.
echo ================================================
echo   Готово! Откройте папку с .pic файлами.
echo ================================================
pause
