# Как собрать и запустить игру

Для того чтобы собрать игру в единый исполнимый файл (EXE), используйте следующую команду:

```bash
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:IncludeAllContentForSelfExtract=true
