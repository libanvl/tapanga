# Tapanga

An extensible tool for generating profiles for Windows Terminal

## Requirements

[.NET 6 Runtime](https://dotnet.microsoft.com/download/dotnet/6.0)

## Install

Tapanaga can be installed as a dotnet tool:

```
dotnet tool install --tool-path c:\dotnet-tools --prerelease tapanga.commandline
```

Tapanga uses plugins to generate the profiles. It comes with one generator by
default, called `core.ssh`.

```
c:\dotnet-tools\tapanga.exe gen core.ssh go
```

Install other plugin assemblies in the same path as tapanga.commandline.dll, or provide the path to the
directory with the plugin assemblies:

```
c:\dotnet-tools\tapanga.exe -pp \path\to\plugins\ gen <plugin.generator> go
```

```
PS C:\dotnet-tools> .\tapanga.exe --help
Tapanga.CommandLine
  TaPaN-Ga: Terminal Profile N-Generator

Usage:
  Tapanga.CommandLine [options] [command]

Options:
  -pp, --plugin-path <plugin-path>  Directory to search for plugin assemblies. Accepts mutiple options. [default:
                                    C:\dotnet-tools\.store\tapanga.commandline\1.2.4-beta\tapanga.commandline\1.2.4-beta
                                    \tools\net6.0\any\]
  --version                         Show version information
  -?, -h, --help                    Show help and usage information

Commands:
  gen, generator  Interact with the available generators
  pro, profile    Manage the generated profiles
```
