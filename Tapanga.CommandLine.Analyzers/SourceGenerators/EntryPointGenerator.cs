using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using SourceGeneratorsKit;

namespace Tapanga.CommandLine.SourceGenerators
{

    /// <summary>
    /// Generates the entry point for a tapanga plugin tool.
    /// </summary>
    [Generator]
    public class EntryPointGenerator : ISourceGenerator
    {
        private readonly ClassesWithInterfacesReceiver interfacesReceiver = new ClassesWithInterfacesReceiver("IGeneratorProvider");

        /// <inheritdoc />
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => interfacesReceiver);
        }

        /// <inheritdoc />
        public void Execute(GeneratorExecutionContext context)
        {
            Debugger.Launch();
            if (this.interfacesReceiver.Classes.SingleOrDefault() is INamedTypeSymbol providerSymbol)
            {
                string description = providerSymbol.ToDisplayString();

                if (providerSymbol.FindAttribute("Description") is AttributeData attribute)
                {
                    description = (string)attribute.ConstructorArguments[0].Value;
                }

                string source = $@"

namespace {providerSymbol.ToDisplayString()};

public class Program
{{
    public static async Task<int> Main(string[] args)
    {{
        var provider = new {providerSymbol.ContainingNamespace.ToDisplayString()}.{providerSymbol.Name}();
        var factories = provider.GetGeneratorFactories)();
        await new Tapanga.CommandLine.Runner(""{description}"", factories).InvokeAsync(args);
    }}
}}
";

                context.AddSource("Program", source);
            }
        }
    }
}
