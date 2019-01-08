[![AppVeyor](https://ci.appveyor.com/api/projects/status/ifjrmddnbmedidb0?svg=true&style=flat)](https://ci.appveyor.com/project/Epoque1/mimick-fody) 
[![NuGet Status](http://img.shields.io/nuget/v/Mimick.Fody.svg?style=flat&max-age=86400)](https://www.nuget.org/packages/Mimick.Fody/)

## This is an add-in for [Fody](http://github.com/Fody/Fody)

![Mimick](https://github.com/Epoque1/Mimick.Fody/raw/master/icon.png)

This framework introduces automated behaviour when developing an application, and provides dependency injection support, and dependency resolution.

## Usage

See also [Fody usage](http://github.com/Fody/Fody#usage).

### NuGet

The framework is available for installation from the NuGet package manager. Either install the framework through the Visual Studio package manager interface, or by running the following commands:

```powershell
PM> Install-Package Fody
PM> Install-Package Mimick.Fody
```

### Weavers

The package must then be added to the Fody `FodyWeavers.xml` document, such as below:

```xml
<?xml version="1.0" encoding="utf-8"?>
<Weavers>
  <Mimick />
</Weavers>
```

### Getting Started

The Mimick Framework requires a little configuration before it can be immediately used within an application. For more information, please refer to the [Getting Started](https://github.com/Epoque1/Mimick.Fody/wiki/Getting-Started) page.

## Building

The framework is designed to run on both .NET Framework (4.6.1+) and .NET Standard (2.0+). Running the `dotnet build` command, as described below, will result in a `net461` and `netstandard2.0` build being created.

### Build

The library can be built locally by running the following against the solution directory:

```powershell
C:\Path> dotnet build -c Release
```

### Test

The library can be tested locally by running the following againstthe solution directory:

```powershell
C:\Path> dotnet test -c Release Mimick.Tests\Mimick.Tests.csproj
```
