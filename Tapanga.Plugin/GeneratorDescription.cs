using System.Collections;

namespace Tapanga.Plugin;

public class GeneratorDescription : IReadOnlyList<string>, IEnumerable<string>
{
    private readonly IList<string> _inner;

    public GeneratorDescription(IList<string> text) => _inner = text;

    public GeneratorDescription() : this(new List<string>())
    {
    }

    public static GeneratorDescription Empty { get; } = new GeneratorDescription(Enumerable.Empty<string>().ToList());

    public string this[int index] => ((IReadOnlyList<string>)_inner)[index];

    public int Count => ((IReadOnlyCollection<string>)_inner).Count;

    public void Add(string line) => _inner.Add(line);

    public IEnumerator<string> GetEnumerator() => _inner.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_inner).GetEnumerator();

    public override string ToString() => String.Join("\n", _inner);
}
