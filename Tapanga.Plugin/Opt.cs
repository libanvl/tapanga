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

        public static Opt<string> WrapOpt(this string value, bool whitespaceIsNone = false)
        {
            return whitespaceIsNone
                ? string.IsNullOrWhiteSpace(value) ? Tapanga.None.String : Some(value)
                : string.IsNullOrEmpty(value) ? Tapanga.None.String : Some(value);
        }

        public static Opt<T>.Some Some<T>(T value) => new(value);

        public static Opt<T>.None None<T>() => Opt<T>.NoneInstance;
    }

    public static class None
    {
        public static Opt<string>.None String => Opt<string>.NoneInstance;
    }
}
