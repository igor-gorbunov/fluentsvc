# fluentsvc
A set of Windows services managing utilities

Right now SvcWrapper runs Nginx executable as a service.

To install SvcWrapper use InstallUtil.exe from Visual Studio distribution:
InstallUtil.exe /LogFile= /LogToConsole=true SvcWrapper.exe

To uninstall SvcWrapper type:
InstallUtil.exe /uninstall /LogFile= /LogToConsole=true SvcWrapper.exe
