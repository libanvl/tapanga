using System.CommandLine.Rendering;
using System.CommandLine.Rendering.Views;
using Tapanga.Core;

namespace Tapanga.CommandLine;

internal class GeneratorIdView : StackLayoutView
{
    public GeneratorIdView(GeneratorId id)
        : base(Orientation.Horizontal)
    {
        Add($" Generator {id.Key} {id.Version} ".Cyan().Reverse().AsView());
        Add(Span(" ").AsView());
        Add($"{id.AssemblyName} {id.AssemblyVersion}".Cyan().AsView());
    }

    private TextSpan Span(object obj)
    {
        return Formatter.Format(obj);
    }

    protected TextSpanFormatter Formatter { get; } = new TextSpanFormatter();
}
