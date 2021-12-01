using System.CommandLine.Rendering.Views;
using Tapanga.Core;

namespace Tapanga.CommandLine;

internal class ProfileDataExItemsView : BaseStackLayoutView
{
    public ProfileDataExItemsView(IEnumerable<ProfileDataEx> profiles)
        : base(Orientation.Vertical)
    {
        var profileLookup = profiles.ToLookup(p => p.GeneratorId);

        foreach (var x in profileLookup)
        {
            Add(new GeneratorIdView(x.Key));
            Add(SpanNewLine);
            Add(new ProfileTableView()
            {
                Items = x.ToList()
            });
            Add(SpanNewLine);
            Add(SpanNewLine);
        }
    }
}
