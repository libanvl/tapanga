using System.CommandLine.Rendering.Views;
using Tapanga.Core;

namespace Tapanga.CommandLine;

internal class ProfileDataExItemsView : StackLayoutView
{
    public ProfileDataExItemsView(IEnumerable<ProfileDataEx> profiles)
    {
        var profileLookup = profiles.ToLookup(p => p.GeneratorId);

        foreach (var x in profileLookup)
        {
            Add(new GeneratorIdView(x.Key));
            Add(new ContentView(Environment.NewLine));
            Add(new ProfileTableView()
            {
                Items = x.ToList()
            });
            Add(new ContentView(Environment.NewLine));
            Add(new ContentView(Environment.NewLine));
        }
    }
}
