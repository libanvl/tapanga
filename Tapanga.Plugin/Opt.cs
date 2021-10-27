namespace Tapanga
{
    public abstract record BaseOpt(Type ValueType);

    public abstract record Opt<T>() : BaseOpt(typeof(T))
    {
        public sealed record Some(T Value) : Opt<T>
        {
            public override T Unwrap() => Value;

            public override T SomeOrDefault(T defaultValue) => Value;
        }

        public abstract record None : Opt<T>
        {
            public override T Unwrap() => throw new ArgumentNullException(typeof(T).AssemblyQualifiedName);

            public override T SomeOrDefault(T defaultValue) => defaultValue;
        }

        private sealed record NoneImpl : None;

        public static None NoneInstance { get; } = new NoneImpl();

        /// <summary>
        /// Returns the value if Some, or throws if None.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException" />
        public abstract T Unwrap();

        public abstract T SomeOrDefault(T defaultValue);

        public static implicit operator Opt<T>(T value) => value.WrapOpt();
    }

    public static class Opt
    {
        public static Opt<T> WrapOpt<T>(this T? value) => value is null ? None<T>() : Some(value);

        public static Opt<string> WrapOpt(this string? value, bool whitespaceIsNone = false)
        {
            return whitespaceIsNone
                ? string.IsNullOrWhiteSpace(value) ? Tapanga.None.String : Some(value)
                : string.IsNullOrEmpty(value) ? Tapanga.None.String : Some(value);
        }

        public static Opt<IEnumerable<T>> WrapOpt<T>(this IEnumerable<T>? value, bool emptyIsNone = false)
        {
            if (value is not null)
            {
                return emptyIsNone
                    ? value.Any() ? Some(value) : None<IEnumerable<T>>()
                    : Some(value);
            }

            return None<IEnumerable<T>>();
        }

        public static Opt<T>.Some Some<T>(T value) => new(value);

        public static Opt<T>.None None<T>() => Opt<T>.NoneInstance;
    }

    public static class None
    {
        public static Opt<string>.None String => Opt<string>.NoneInstance;

        public static Opt<int>.None Int => Opt<int>.NoneInstance;

        public static Opt<bool>.None Bool => Opt<bool>.NoneInstance;

        public static Opt<double>.None Double => Opt<double>.NoneInstance;
    }

    public static class Some
    {
        public static Opt<string>.Some String(string v) => new(v);
    }

    public record GenericEnum<T, U>(T EnumValue, U ObjectValue) where T : Enum
    {
        public static implicit operator T(GenericEnum<T, U> arg) => arg.EnumValue;
        public static implicit operator U(GenericEnum<T, U> arg) => arg.ObjectValue;
    }
}
