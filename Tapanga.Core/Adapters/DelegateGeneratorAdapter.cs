using libanvl;
using System.ComponentModel;
using System.Reflection;
using Tapanga.Plugin;

namespace Tapanga.Core;

internal class DelegateGeneratorAdapter : ProfileGeneratorAdapter, IDelegateProfileGenerator
{
    private IReadOnlyList<UserArgument>? _cachedArguments = null;

    private readonly IDelegateProfileGenerator _inner;

    public DelegateGeneratorAdapter(IDelegateProfileGenerator delegateGenerator)
        : base(delegateGenerator)
    {
        _inner = delegateGenerator;
    }

    public bool IsAsyncDelegate => _inner.IsAsyncDelegate;

    public Type ParameterType => _inner.ParameterType;

    public override IReadOnlyList<UserArgument> GetUserArguments()
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
            var uaAttr = p.GetCustomAttribute<UserArgumentAttribute>(inherit: true);
            if (uaAttr is null)
            {
                continue;
            }

            Type boundType = p.PropertyType;

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
                object? defaultOptValue = defaultOptType.GetProperty(nameof(Opt<object>.None), BindingFlags.Static | BindingFlags.Public)?.GetValue(null);
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
                    uaAttr.ShortName.WrapOpt(whitespaceIsNone: true),
                    uaAttr.Description,
                    defaultOptValue,
                    uaAttr.IsRequired,
                    uaAttr.Sort
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
