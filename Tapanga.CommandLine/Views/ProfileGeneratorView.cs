using libanvl;
using System.CommandLine.Rendering;
using System.CommandLine.Rendering.Views;
using Tapanga.Core;

namespace Tapanga.CommandLine;

internal class ProfileGeneratorView : StackLayoutView
{
    public ProfileGeneratorView(IProfileGeneratorAdapter generator)
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
            cellValue: oa => oa.ShortName.SomeOrDefault("None").White(),
            header: new ContentView("Short Name".Underline()));

        oaTableView.AddColumn(
            cellValue: oa => oa.IsRequired ? "*".White() : "".White(),
            header: new ContentView("Required".Underline()));

        oaTableView.AddColumn(
            cellValue: oa => oa.Description?.White() ?? "None".White(),
            header: new ContentView("Description".Underline()));

        oaTableView.AddColumn(
            cellValue: oa => oa.GetDefaultValuesString().White(),
            header: new ContentView("Default".Underline()));

        Add(new GeneratorIdView(generator.GeneratorId));
        Add(Span(Environment.NewLine).AsView());

        Add(generator.Description.ToString().Yellow().AsView());
        Add(Span(Environment.NewLine).AsView());

        Add(generator.GeneratorInfo.ToString().White().AsView());
        Add(Span(Environment.NewLine).AsView());

        Add("Arguments".Green().Reverse().AsView());
        Add(oaTableView);
    }

    private TextSpan Span(object obj) => Formatter.Format(obj);

    protected TextSpanFormatter Formatter { get; } = new TextSpanFormatter();
}
