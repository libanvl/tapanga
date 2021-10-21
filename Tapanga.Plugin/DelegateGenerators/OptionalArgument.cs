namespace Tapanga.Plugin
{
    public abstract class OptionalArgument
    {
        public abstract object? GetValue();
    }

    public class OptionalArgument<T> : OptionalArgument
    {
        private static readonly Dictionary<Type, Func<string?, object?>> Converters = new()
        {
            { typeof(bool), x => !string.IsNullOrWhiteSpace(x) && System.Convert.ToBoolean(x) },
            { typeof(string), x => string.IsNullOrWhiteSpace(x) ? null : x },
            { typeof(FileInfo), x => string.IsNullOrWhiteSpace(x) ? null : new FileInfo(x) },
            { typeof(DirectoryInfo), x => string.IsNullOrWhiteSpace(x) ? null : new DirectoryInfo(x) }
        };

        private readonly Opt<T> _value;

        public OptionalArgument(string? result) => _value = Convert(result).WrapOpt();

        private OptionalArgument(T value) => _value = value.WrapOpt();

        public static OptionalArgument<T> Make(T value) => new(value);

        public static OptionalArgument<T> Empty { get; } = new OptionalArgument<T>(null);

        private static T? Convert(string? result)
        {
            return Converters.TryGetValue(typeof(T), out var converter)
                ? (T?)converter(result)
                : (T?)System.Convert.ChangeType(result, typeof(T?));
        }

        public Opt<T> AsOpt() => _value;

        public override object? GetValue() => this._value.SomeOrDefault(default!);

        public static implicit operator Opt<T>(OptionalArgument<T> arg) => arg.AsOpt();

        public static explicit operator T?(OptionalArgument<T> arg) => arg._value.SomeOrDefault(default!);
    }
}
