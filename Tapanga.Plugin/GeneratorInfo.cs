using System.Collections;

namespace Tapanga.Plugin;

public class GeneratorInfo : IReadOnlyList<string>, IEnumerable<string>
{
    private readonly IList<string> _inner;

    public GeneratorInfo(IList<string> text) => _inner = text;

    public GeneratorInfo() : this(new List<string>())
    {
    }

    public static GeneratorInfo Empty { get; } = new GeneratorInfo(Enumerable.Empty<string>().ToList());

    public string this[int index] => ((IReadOnlyList<string>)_inner)[index];

    public int Count => ((IReadOnlyCollection<string>)_inner).Count;

    public void Add(string line) => _inner.Add(line);

    public IEnumerator<string> GetEnumerator() => _inner.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_inner).GetEnumerator();

    public override string ToString() => String.Join(Environment.NewLine, _inner);

    public static implicit operator GeneratorInfo(string arg) => new(arg.Split(Environment.NewLine).ToList());
}
