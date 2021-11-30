using System.CommandLine.Rendering.Views;
using Tapanga.Core;

namespace Tapanga.CommandLine;

internal class GeneratorIdView : BaseStackLayoutView
{
    public GeneratorIdView(GeneratorId id)
        : base(Orientation.Horizontal)
    {
        Add($" Generator {id.Key} {id.Version} ".Cyan().Reverse().AsView());
        Add(Span(" ").AsView());
        Add($"{id.AssemblyName} {id.AssemblyVersion}".Cyan().AsView());
    }
}
