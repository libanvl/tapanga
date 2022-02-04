using System.Collections;

namespace Tapanga.Plugin;

/// <summary>
/// A collection of strings.
/// </summary>
public class StringCollection : IReadOnlyList<string>, IEnumerable<string>
{
    private readonly IList<string> _inner;

    /// <summary>
    /// Creates a new instance of <see cref="StringCollection"/> initialized by <paramref name="text"/>.
    /// </summary>
    /// <param name="text"></param>
    public StringCollection(IList<string> text) => _inner = text;

    /// <summary>
    /// Creates a new empty instance of <see cref="StringCollection"/>
    /// </summary>
    public StringCollection() : this(new List<string>())
    {
    }

    /// <summary>
    /// Creates a <see cref="StringCollection"/> from the <paramref name="arg"/>.
    /// </summary>
    /// <param name="arg"></param>
    public static implicit operator StringCollection(string arg) => new(arg.Split(Environment.NewLine).ToList());

    /// <summary>
    /// Creates a string from <paramref name="arg"/> by joining the strings in the collection with <see cref="Environment.NewLine"/>.
    /// </summary>
    /// <param name="arg"></param>
    public static implicit operator string(StringCollection arg) => arg.ToString();

    /// <summary>
    /// Gets an empty instance of <see cref="StringCollection"/>.
    /// </summary>
    public static StringCollection Empty { get; } = new StringCollection(Enumerable.Empty<string>().ToList());

    /// <inheritdoc />
    public string this[int index] => ((IReadOnlyList<string>)_inner)[index];

    /// <inheritdoc />
    public int Count => ((IReadOnlyCollection<string>)_inner).Count;

    /// <inheritdoc />
    public void Add(string line) => _inner.Add(line);

    /// <inheritdoc />
    public IEnumerator<string> GetEnumerator() => _inner.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_inner).GetEnumerator();

    /// <inheritdoc />
    public override string ToString() => String.Join(Environment.NewLine, _inner);
}
