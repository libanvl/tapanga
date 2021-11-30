using System.CommandLine.Rendering;
using System.CommandLine.Rendering.Views;

namespace Tapanga.CommandLine;

internal abstract class BaseStackLayoutView : StackLayoutView
{
    public BaseStackLayoutView(Orientation orientation) 
        : base(orientation)
    {
    }

    protected virtual TextSpanFormatter Formatter { get; } = new TextSpanFormatter();

    protected virtual View SpanNewLine => Span(Environment.NewLine).AsView();

    protected virtual TextSpan Span(object obj) => Formatter.Format(obj);
}