using System.ComponentModel;
using System.Reflection;
using Tapanga.Plugin;

namespace Tapanga.CommandLine
{
    internal class DelegateGeneratorAdapter : IProvideUserArguments, IProfileGenerator
    {
        private static IReadOnlyList<UserArgument>? _cachedArguments = null;

        private readonly IDelegateProfileGenerator _inner;

        public DelegateGeneratorAdapter(IDelegateProfileGenerator delegateGenerator)
        {
            _inner = delegateGenerator;
        }

        public GeneratorInfo GeneratorInfo => _inner.GeneratorInfo;

        public Delegate GetGeneratorDelegate(IProfileCollection profiles) => _inner.GetGeneratorDelegate(profiles);

        public IReadOnlyList<UserArgument> GetUserArguments()
        {
            if (_cachedArguments is null)
            {
                var argProps = _inner.ParameterType.GetProperties();
                _cachedArguments = argProps.Any()
                    ? GetUserArgumentsImpl(argProps)
                    : new List<UserArgument>();
            }

            return _cachedArguments;
        }

        private static List<UserArgument> GetUserArgumentsImpl(PropertyInfo[] argProps)
        {
            var userArguments = new List<UserArgument>();

            foreach (var p in argProps)
            {
                var attr = p.GetCustomAttribute<UserArgumentAttribute>(inherit: true);
                if (attr is null)
                {
                    continue;
                }

                bool isRequired = true;
                Type boundType = p.PropertyType;
                if (boundType.IsGenericType)
                {
                    if (boundType.IsAssignableTo(typeof(OptionalArgument<>).MakeGenericType(boundType.GetGenericArguments())))
                    {
                        isRequired = false;
                        boundType = typeof(OptionalArgument<>).MakeGenericType(boundType.GetGenericArguments());
                    }
                }

                var defaultOptType = typeof(Opt<>).MakeGenericType(boundType);
                var userArgumentType = typeof(UserArgument<>).MakeGenericType(boundType);
                var userArgumentConstructor = userArgumentType.GetConstructor(new[]
                {
                    typeof(string), /* LongName */
                    typeof(Opt<string>), /* ShortName */
                    typeof(string), /* Description */
                    defaultOptType, /* DefaultValue */
                    typeof(bool), /* IsRequired */
                    typeof(int) /* Sort */
                });

                if (userArgumentConstructor is null)
                {
                    // Log an error somehwere
                    continue;
                }

                object? defaultOptValue = defaultOptType.GetProperty("NoneInstance", BindingFlags.Static | BindingFlags.Public)?.GetValue(null);
                var valfac = p.GetCustomAttribute<DefaultValueFactoryAttribute>();
                if (valfac is not null)
                {
                    var defaultValueFactoryMethod = p.DeclaringType?.GetMethod(valfac.MethodName, BindingFlags.Static | BindingFlags.Public);
                    var defaultValue = defaultValueFactoryMethod?.Invoke(null, null);
                    if (GetDefaultOptValue(boundType, defaultValue) is object value)
                    {
                        defaultOptValue = value;
                    }
                }
                else if (p.GetCustomAttribute<DefaultValueAttribute>(inherit: true)?.Value is object defaultValue)
                {
                    if (GetDefaultOptValue(boundType, defaultValue) is object value)
                    {
                        defaultOptValue = value;
                    }
                }

                var userArgument = (UserArgument)userArgumentConstructor.Invoke(new object?[]
                {
                    p.Name.ToLowerInvariant(),
                    attr.ShortName.WrapOpt(whitespaceIsNone: true),
                    attr.Description,
                    defaultOptValue,
                    isRequired,
                    attr.Sort
                });

                userArguments.Add(userArgument);
            }

            return userArguments;
        }

        private static object? GetDefaultOptValue(Type p, object? defaultValue)
        {
            if (defaultValue != null)
            {
                if (p.IsGenericType)
                {
                    Type optionalType = typeof(OptionalArgument<>).MakeGenericType(p.GenericTypeArguments);

                    if (p.IsAssignableTo(optionalType))
                    {
                        var optionalMaker = optionalType.GetMethod("Make", BindingFlags.Public | BindingFlags.Static);
                        defaultValue = optionalMaker?.Invoke(null, new[] { defaultValue });
                    }
                }

                var someType = typeof(Opt<>.Some).MakeGenericType(p);
                var someTypeConstructor = someType.GetConstructor(new[]
                {
                    p
                });

                if (someTypeConstructor != null)
                {
                    return someTypeConstructor.Invoke(new[]
                    {
                        defaultValue
                    });
                }
            }

            return null;
        }
    }
}
