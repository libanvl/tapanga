# Tapanga

An framework for generating profiles for Windows Terminal

## Requirements

[.NET 6 Runtime](https://dotnet.microsoft.com/download/dotnet/6.0)

## Use

Start with a Tapanga Plugin assembly.

Add a new executable project, and reference the plugin assembly.

Override the build property ToolCommandName

> Plugin.CommandLine.csproj

```xml
<PropertyGroup>
    <ToolCommandName>my-awesome-tool</ToolCommandName>
</PropertyGroup>
```

Add an entry point to register the generators from your plugin with the tapanga runner:

> Program.cs

```csharp
using Tapanga.CommandLine;
using Tapanga.Core;
using Tapanga.Core.Generators;
using Tapanga.Plugin;

await new Runner("Tapanga Demonstration Generators", new[]
{
    new GeneratorFactoryAsync(() => Task.FromResult<IProfileGenerator>(new SecureShellGenerator()))
}).InvokeAsync(args);
```

`dotnet pack` will pack the executable project as a tool. Publish to a NuGet registry and use `dotnet tool` to install it.
