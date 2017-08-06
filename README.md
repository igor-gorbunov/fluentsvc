# fluentsvc

A set of Windows services managing utilities

Right now SvcWrapper runs Nginx executable as a service.

### Installing

Download Nginx package, place its contents to "C:\Program Files\nginx" directory, copy SvcWrapper.exe to the Nginx directory and install SvcWrapper using InstallUtil,
which is the part of .NET Redistributable. For example, for .NET 4.0 run Windows command prompt as an administrator and type:

```
C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /ServiceName=Nginx /DisplayName="Nginx HTTP/HTTPS server" /Description="Simple Nginx service wrapping utility." "C:\Program Files\nginx\SvcWrapper.exe"
```

To uninstall SvcWrapper type:

```
C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /uninstall /ServiceName=Nginx "C:\Program Files\nginx\SvcWrapper.exe"
```
