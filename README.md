# fluentsvc

A set of Windows services managing utilities

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes.
See deployment for notes on how to deploy the project on a live system.

### Prerequisites

To run the programs in this project you must install Microsoft.NET Framework.

To build the programs in this project you can use Microsoft Visual Studio 2015 Community or Microsoft Visual Studio 2017 Community.
Download the FluentSvc source code, open vs.sln\FluentSvc.sln file in preferred Visual Studio IDE and press F7 button.

Another way to build all the projects is to run MSBuild Command Prompt and type:

```
C:\Program Files (x86)\Microsoft Visual Studio 14.0>msbuild /property:Configuration=Debug ^
More?D:\Projects\fluentsvc.git\vs.sln\FluentSvc.sln
```

## Deployment

There is no need to specifically deploy the generated executables.

I can't redistribute compiled binaries as I don't have a code signing certificate right now. If you intend to redistribute the compiled binaries,
please, digitally sign them with your code signing certificate to prevent using the binaries if they are poisoned with malware.

## SvcWrapper

SvcWrapper is used to run a program as a Windows service. It takes some arguments on install, describing the way to start/stop the program.
This program is much like [kohsuke/winsw](https://github.com/kohsuke/winsw) project, but supports installation through Microsoft.NET InstallUtil
and stores start/stop parameters in Windows registry. Also, it does not support configuration through .xml-file right now.

### Installing

Choose a program to run as a service, place it to a directory, copy SvcWrapper.exe to that directory and install SvcWrapper using InstallUtil,
which is the part of .NET Redistributable. Arguments for the SvcWrapper are supplied through InstallUtil in the form of /NameOfArgument=Value.

Currently supported arguments are:

* **ServiceName** the name of newly created service. The ServiceName cannot be null or have zero length. Its maximum size is 256 characters.
It also cannot contain forward or backward slashes, '/' or '\', or characters from the ASCII character set with value less than decimal value 32.
* **DisplayName** the friendly name that identifies the service to the user.
* **Description** description of the service.
* **StartCommand** the path to executable to run when service is to be started. The path will be quoted before call to process start API if quotes are missing,
since this a security recommendation.
* **StartArguments** the arguments to the start command. The string is passed as is.
* **StopCommand** the path to executable to run when service is to be stopped. The path will be quoted before call to process start API if quotes are missing,
since this a security recommendation.
* **StopArguments** the arguments to the stop command. The string is passed as is.

For example, to install Nginx as a Windows service, run Windows command prompt as an administrator and type (supposed you have put Nginx package
contents and accompanying SvcWrapper executable to "C:\Program Files\nginx"):

```
C:\Windows\system32>C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /ServiceName=Nginx ^
More?/DisplayName="Nginx HTTP Server" /Description="Nginx is a fast and lightweight HTTP/HTTPS web-server" ^
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

## Contributing

Suggestions, ideas and pull requests are welcome.

## Versioning

This project is using [SemVer](http://semver.org/) for versioning. For the versions available, see the [tags](https://github.com/igor-gorbunov/fluentsvc/tags).

## Authors

* **Igor Gorbunov** - *Initial work* - [igor-gorbunov](https://github.com/igor-gorbunov)

## License

This project is licensed under the GPL-3.0 License - see the [LICENSE](LICENSE) file for details.
