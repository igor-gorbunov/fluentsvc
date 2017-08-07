# fluentsvc

A set of Windows services managing utilities

## SvcWrapper

SvcWrapper is used to run a program as a Windows service. It takes some arguments on install, describing the way to start/stop the program.

### Installing

Choose a program to run as a service, place it to a directory, copy SvcWrapper.exe to that directory and install SvcWrapper using InstallUtil,
which is the part of .NET Redistributable. Arguments for the SvcWrapper are supplied through InstallUtil in the form of /NameOfArgument=Value.

For example, to install Nginx as a Windows service, run Windows command prompt as an administrator and type (supposed you have put Nginx package
contents and accompanying SvcWrapper executable to "C:\Program files\nginx"):

```
C:\Windows\system32>C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /ServiceName=Nginx ^
More?/DisplayName="Nginx HTTP Server" ^
More?/Description="Nginx is a fast and lightweight HTTP/HTTPS web-server" ^
More?/StartCommand="C:\Program Files\nginx\nginx.exe" /StartArguments="-p \"C:\Program Files\nginx\"" ^
More?/StopCommand="C:\Program Files\nginx\nginx.exe" /StopArguments="-s stop -p \"C:\Program Files\nginx\"" ^
More?"C:\Program Files\nginx\SvcWrapper.exe"
```

To uninstall the service type:

```
C:\Windows\system32>C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe ^
More?/uninstall /ServiceName=Nginx "C:\Program Files\nginx\SvcWrapper.exe"
```

## SvcList

SvcList is used to list installed Windows services.
