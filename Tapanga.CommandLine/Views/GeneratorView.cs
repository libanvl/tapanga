using libanvl;
using System.CommandLine.Rendering;
using System.CommandLine.Rendering.Views;
using Tapanga.Core;

namespace Tapanga.CommandLine;

internal class GeneratorView : StackLayoutView
{
    public GeneratorView(IProfileGeneratorAdapter generator)
    {
        var oaTableView = new TableView<OptionAdapter>
        {
            Items = generator.GetUserArguments()
                .Select(ua => new OptionAdapter(ua))
                .ToList()
        };

        oaTableView.AddColumn(
            cellValue: oa => oa.LongName.LightGreen(),
            header: new ContentView("Long Name".Underline()));

        oaTableView.AddColumn(
            cellValue: oa => oa.ShortName.SomeOrDefault(string.Empty).White(),
            header: new ContentView("Alias".Underline()));

        oaTableView.AddColumn(
            cellValue: oa => oa.IsRequired ? "*".White() : "".White(),
            header: new ContentView("*".Underline()));

        oaTableView.AddColumn(
            cellValue: oa => (oa.Arity.MaximumNumberOfValues > 1 ? "+" : string.Empty).White(),
            header: new ContentView("+".Underline()));

        oaTableView.AddColumn(
            cellValue: oa => oa.Description?.White() ?? "None".White(),
            header: new ContentView("Description".Underline()));

        oaTableView.AddColumn(
            cellValue: oa => oa.GetDefaultValuesString().White(),
            header: new ContentView("Default".Underline()));

        oaTableView.AddColumn(
            cellValue: oa => oa.BoundType.Name.White(),
            header: new ContentView("Type".Underline()));

        oaTableView.AddColumn(
            cellValue: oa => GetEnumValuesSpan(oa.BoundType),
            header: new ContentView("Enum".Underline()));

        Add(new GeneratorIdView(generator.GeneratorId));
        Add(SpanNewLine);

        Add(generator.Description.ToString().Yellow().AsView());
        Add(SpanNewLine);

        Add(generator.GeneratorInfo.ToString().White().AsView());
        Add(SpanNewLine);

        Add("Arguments".Green().Reverse().AsView());
        Add(oaTableView);
        Add(SpanNewLine);
        Add("*=Required  +=Accepts Multiple".White().AsView());
    }

    private TextSpan Span(object obj) => Formatter.Format(obj);

    private View SpanNewLine => Span(Environment.NewLine).AsView();

    private TextSpan GetEnumValuesSpan(Type type)
    {
        if (type.IsEnum)
        {
            return string.Join(' ', Enum.GetNames(type)).White();
        }

        return Span(string.Empty);
    }

    protected TextSpanFormatter Formatter { get; } = new TextSpanFormatter();
}
