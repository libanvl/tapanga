using libanvl;
using System.CommandLine.Rendering;
using System.CommandLine.Rendering.Views;
using Tapanga.Core;
using Tapanga.Plugin;

namespace Tapanga.CommandLine;

internal class ProfileGeneratorView : StackLayoutView
{
    public ProfileGeneratorView(IProfileGeneratorAdapter generator)
    {
        var uaTableView = new TableView<UserArgument>();
        uaTableView.Items = generator.GetUserArguments();

        uaTableView.AddColumn(
            cellValue: ua => ua.LongName.LightGreen(),
            header: new ContentView("Long Name".Underline()));

        uaTableView.AddColumn(
            cellValue: ua => ua.ShortName.SomeOrDefault("None").White(),
            header: new ContentView("Short Name".Underline()));

        uaTableView.AddColumn(
            cellValue: ua => ua.Required ? "*".White() : "".White(),
            header: new ContentView("Required".Underline()));

        uaTableView.AddColumn(
            cellValue: ua => ua.Description.White(),
            header: new ContentView("Description".Underline()));

        uaTableView.AddColumn(
            cellValue: ua => Span(ua.DefaultObject ?? "None".White()),
            header: new ContentView("Default".Underline()));

        Add(new GeneratorIdView(generator.GeneratorId));
        Add(Span(Environment.NewLine).AsView());

        Add(generator.Description.ToString().Yellow().AsView());
        Add(Span(Environment.NewLine).AsView());

        Add("Arguments".Green().Reverse().AsView());
        Add(uaTableView);
    }

    private TextSpan Span(object obj)
    {
        return Formatter.Format(obj);
    }

    protected TextSpanFormatter Formatter { get; } = new TextSpanFormatter();
}
