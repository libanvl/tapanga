# Tapanga (Terminal Profile N-Generator)

## What is Tapanga?

* Tapanga is a pluggable framework that makes it easy to write “generator” tools for Windows Terminal profiles. It uses the Windows Terminal JSON fragment extension feature.
* Tapanga Plugins are assemblies that provide one or more generators that take user input and turn it into structured profile data such as command-line, profile name, tab title, starting directory, etc…
* Tapanga Core manages the JSON files, updating them when a generator tool is run.

## What is the Tapanga.CommandLine package?

* This package wraps a Plugin to create a dotnet tool package with default subcommands.
* Generator tools can be run as full cli tools, requiring arguments to be passed on the command line. This is the `run` command. Example:
  * `yourtool.exe core.ssh run --destination user@server.tld`
* Generator tools can also be run in interactive mode. In this mode, the user is prompted to provide values for each argument. This is the `go` command. Example:
  * `yourtool.exe core.ssh go`
* Profiles created by Tapanga generator tool can be managed using the `profile` command. It has several subcommands which can be seen using `--help`. Example:
  * `yourtool.exe profile --help`

## Getting Started

* Create a plugin library project using the `Tapanga.Plugin` package
* Create a console project. Add a reference to your plugin project.
* Add the `Tapanga.CommandLine` package to the console project.
* Implement the IGeneratorProvider interface

```csharp
public class GeneratorProvider : IGeneratorProvider
{
    public IEnumerable<GeneratorFactoryAsync> GetGeneratorFactories()
    {
        yield return (_) => Task.FromResult<IProfileGenerator>(new SecureShellGenerator());
        yield return (_) => Task.FromResult<IProfileGenerator>(new TestGenerator());
    }
}
```

* In the console project's entry point, invoke the Tapanga.CommandLine.Runner

```csharp
public static Task<int> Main(string[] args)
{
    return new Runner("Tapanga Demonstration Generators", new GeneratorProvider())
        .InvokeAsync(args);
}
```

_Icons made by [Freepik](https://www.freepik.com) from [www.flaticon.com](https://www.flaticon.com/)_
