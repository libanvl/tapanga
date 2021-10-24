using System.ComponentModel;
using System.Reflection;
using Tapanga.Plugin;

namespace Tapanga.Core;

internal class DelegateGeneratorAdapter : IProvideUserArguments, IProfileGenerator
{
    private static IReadOnlyList<UserArgument>? _cachedArguments = null;

    private readonly IDelegateProfileGenerator _inner;

    public DelegateGeneratorAdapter(IDelegateProfileGenerator delegateGenerator) => _inner = delegateGenerator;

    public GeneratorInfo GeneratorInfo => _inner.GeneratorInfo;

    public Delegate GetGeneratorDelegate(IProfileCollection profiles) => _inner.GetGeneratorDelegate(profiles);

    public IReadOnlyList<UserArgument> GetUserArguments()
    {
        if (_cachedArguments is null)
        {
            var argProps = _inner.ParameterType.GetProperties();
            _cachedArguments = argProps.Any()
                ? GetUserArgumentsImpl(argProps).OrderBy(ua => ua.Sort).ToList()
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
            if (typeof(UserArgument<>).GetGenericConstructor(boundType, new[]
            {
                    typeof(string), /* LongName */
                    typeof(Opt<string>), /* ShortName */
                    typeof(string), /* Description */
                    defaultOptType, /* DefaultValue */
                    typeof(bool), /* IsRequired */
                    typeof(int) /* Sort */
                }) is Opt<ConstructorInfo>.Some someCtor)
            {
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

                var userArgument = (UserArgument)someCtor.Value.Invoke(new object?[]
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
            else
            {
                // log an error somewhere
            }
        }

        return userArguments;
    }

    private static object? GetDefaultOptValue(Type p, object? defaultValue)
    {
        if (defaultValue == null)
        {
            return null;
        }

        if (p.IsGenericType)
        {
            Type optionalType = typeof(OptionalArgument<>).MakeGenericType(p.GenericTypeArguments);

            if (p.IsAssignableTo(optionalType))
            {
                var optionalMaker = optionalType.GetMethod("Make", BindingFlags.Public | BindingFlags.Static);
                defaultValue = optionalMaker?.Invoke(null, new[] { defaultValue });
            }
        }

        if (typeof(Opt<>.Some).GetGenericWrapperConstructor(p) is Opt<ConstructorInfo>.Some someCtor)
        {
            return someCtor.Value.Invoke(new[]
            {
                    defaultValue
                });
        }

        return null;
    }
}
