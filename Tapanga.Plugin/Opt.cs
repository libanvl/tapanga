namespace Tapanga
{
    public abstract record BaseOpt(Type ValueType);

    public abstract record Opt<T>() : BaseOpt(typeof(T))
    {
        /// <inheritdoc />
        public sealed record Some(T Value) : Opt<T>
        {
            public override T Unwrap() => Value;

            public override T SomeOrDefault(T defaultValue) => Value;
        }

        /// <inheritdoc />
        public abstract record None : Opt<T>
        {
            public override T Unwrap() => throw new InvalidOperationException(typeof(T).AssemblyQualifiedName);

            public override T SomeOrDefault(T defaultValue) => defaultValue;
        }

        private sealed record NoneImpl : None;

        /// <summary>
        /// The single instance of None per type T.
        /// </summary>
        public static None NoneInstance { get; } = new NoneImpl();

        /// <summary>
        /// Returns the value if Some, or throws if None.
        /// </summary>
        /// <exception cref="InvalidOperationException" />
        public abstract T Unwrap();

        /// <summary>
        /// Returns the value if Some, or the <paramref name="defaultValue"/> if None.
        /// </summary>
        /// <param name="defaultValue"></param>
        public abstract T SomeOrDefault(T defaultValue);

        public static implicit operator Opt<T>(T value) => value.WrapOpt();
    }

    public static class Opt
    {
        /// <summary>
        /// Wrap <paramref name="value"/> in an appropriate <see cref="Opt{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the <paramref name="value"/> to wrap</typeparam>
        /// <param name="value">The value to wrap</param>
        /// <returns>An <see cref="Opt{T}.None"/> if the value is <c>null</c>, <see cref="Opt{T}.Some"/> otherwise.</returns>
        public static Opt<T> WrapOpt<T>(this T? value) => value is null ? None<T>() : Some(value);

        /// <summary>
        /// Wrap a string in an appropriate <see cref="Opt{string}"/>.
        /// </summary>
        /// <param name="value">The string to wrap</param>
        /// <param name="whitespaceIsNone">Whether to treat whitespace strings the same as null and empty string</param>
        /// <returns>An <see cref="Opt{string}.None"/> if the string is null or empty (or whitespace if <paramref name="whitespaceIsNone"/> is <c>true</c>), <see cref="Opt{string}.Some"/> otherwise.</returns>
        public static Opt<string> WrapOpt(this string? value, bool whitespaceIsNone = false)
        {
            return whitespaceIsNone
                ? string.IsNullOrWhiteSpace(value) ? Tapanga.None.String : Some(value)
                : string.IsNullOrEmpty(value) ? Tapanga.None.String : Some(value);
        }

        /// <summary>
        /// Wrap an IEnumerable in an appropirate <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T">The collection type</typeparam>
        /// <param name="value">The enumerable to warp</param>
        /// <param name="emptyIsNone">Whether to treat empty enumerables the same as a null one</param>
        /// <returns>An <see cref="Opt{IEnumerable{T}}"/> if the value is null (or empty if <paramref name="emptyIsNone"/> is <c>true</c>, an <see cref="Opt{IEnumerable{T}}.Some"/> otherwise.</returns>
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

        /// <summary>
        /// Create an <see cref="Opt{T}.Some"/> for the given value.
        /// </summary>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <param name="value">The value to wrap</param>
        public static Opt<T>.Some Some<T>(T value) => new(value);

        /// <summary>
        /// Get the <see cref="Opt{T}.NoneInstance"/> for a the given type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the Opt</typeparam>
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
