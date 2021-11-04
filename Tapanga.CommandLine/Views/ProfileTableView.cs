using libanvl;
using System.CommandLine.Rendering.Views;
using Tapanga.Core;

namespace Tapanga.CommandLine;

internal class ProfileTableView : TableView<ProfileDataEx>
{
    public ProfileTableView()
    {
        AddColumn(
            header: "Id".Underline().AsView(),
            cellValue: p => p.ProfileId.ToString()[0..6].Green().Reverse());

        AddColumn(
            header: "Name".Underline().AsView(),
            cellValue: p => p.Name.White());

        AddColumn(
            header: "Command Line".Underline().AsView(),
            cellValue: p => p.CommandLine.Yellow());

        AddColumn(
            header: "Tab Title".Underline().AsView(),
            cellValue: p => p.TabTitle.SomeOrEmpty().White());

        AddColumn(
            header: "Starting Directory".Underline().AsView(),
            cellValue: p => p.StartingDirectory.Select(sd => sd.FullName).SomeOrEmpty().White());

        AddColumn(
            header: "Icon".Underline().AsView(),
            cellValue: p => p.Icon.Select(i => i.Name).SomeOrEmpty().White());
    }
}
