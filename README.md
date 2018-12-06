[![AppVeyor](https://ci.appveyor.com/api/projects/status/ifjrmddnbmedidb0?svg=true&style=flat)](https://ci.appveyor.com/project/Epoque1/mimick-fody) 

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

### Attributes

The Mimick framework provides a collection of standard attributes which can be applied at class, field, property, constructor and method levels. For more information, please see the [Attributes wiki page](https://github.com/Epoque1/Mimick.Fody/wiki/Attributes) for a complete list of all built-in attributes.
