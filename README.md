# 🖼️ OCIF Thumbnail Provider for Windows

> Миниатюры для файлов формата **OCIF** (OpenComputers Image Format, `.pic`) в Проводнике Windows 11.

![Preview](preview.png)

---

## Что это?

[OpenComputers](https://github.com/MightyPirates/OpenComputers) — мод для Minecraft, добавляющий программируемые компьютеры. Эти компьютеры используют собственный графический формат **OCIF v8** для хранения изображений в файлах `.pic`.

По умолчанию Windows показывает для таких файлов пустую иконку. Этот Shell Extension добавляет нормальные миниатюры прямо в Проводник.

---

## Скриншот

| До | После |
|:--:|:-----:|
| ![before](docs/before.png) | ![after](docs/after.png) |

---

## Требования

- Windows 10 / 11 (x64)
- [.NET Framework 4.8](https://dotnet.microsoft.com/download/dotnet-framework/net48) — обычно уже есть в Windows 11

---

## Установка

### Автоматически

1. Скачай [последний релиз](../../releases/latest)
2. Распакуй архив
3. Запусти `install.bat` **от имени администратора**
4. Готово — открой папку с `.pic` файлами в Проводнике

### Вручную

```cmd
:: От имени администратора
"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\regasm.exe" "OcifThumb.dll" /codebase

reg add "HKCR\.pic\ShellEx\{e357fccd-a995-4576-b01f-234630154e96}" /ve /d "{74690D5E-36B5-3E0A-B181-BDA910DD749F}" /f
reg add "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Shell Extensions\Approved" /v "{74690D5E-36B5-3E0A-B181-BDA910DD749F}" /d "OCIF Thumbnail Provider" /f

taskkill /f /im explorer.exe
start explorer.exe
```

---

## Удаление

Запусти `uninstall.bat` от имени администратора, или вручную:

```cmd
"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\regasm.exe" "OcifThumb.dll" /unregister
reg delete "HKCR\.pic\ShellEx\{e357fccd-a995-4576-b01f-234630154e96}" /f
reg delete "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Shell Extensions\Approved" /v "{74690D5E-36B5-3E0A-B181-BDA910DD749F}" /f
```

---

## Сборка из исходников

Нужно:
- [.NET Framework 4.8 Developer Pack](https://dotnet.microsoft.com/download/dotnet-framework/net48)
- [.NET SDK](https://dotnet.microsoft.com/download) (любая версия)

```cmd
git clone https://github.com/pronoua/ocif-windows11.git
cd ocif-windows11
dotnet build OcifThumb.csproj -c Release
```

---

## Как это работает

Shell Extension реализует COM интерфейс `IThumbnailProvider` через библиотеку [SharpShell](https://github.com/dwmkerr/sharpshell).

При открытии папки Проводник вызывает наш обработчик для каждого `.pic` файла. Обработчик декодирует бинарный формат OCIF v8 — палитра из 240 цветов, символы Braille для субпиксельной графики — и возвращает `Bitmap` который Проводник показывает как миниатюру.

---

## Формат OCIF v8

```
[4 bytes] Сигнатура: "OCIF"
[1 byte]  Версия: 0x08
[1 byte]  Ширина - 1 (в символах)
[1 byte]  Высота - 1 (в символах)
[...]     Данные пикселей с RLE-подобным сжатием
```

Каждый символ занимает 2×4 пикселя. Для субпиксельной графики используются символы Брайля (U+2800–U+28FF).

---

## Связанные проекты

- 🐧 [ocif-kde](https://github.com/pronoua/ocif-kde) — то же самое для Linux (KDE6/Dolphin)
- 🎮 [OpenComputers](https://github.com/MightyPirates/OpenComputers) — мод для Minecraft

---

## Лицензия

MIT
