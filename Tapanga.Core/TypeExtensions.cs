using System.Reflection;

namespace Tapanga.Core;

public static class TypeExtensions
{
    public static Opt<ConstructorInfo> GetGenericWrapperConstructor(this Type wrapperType, Type boundType)
    {
        return GetGenericConstructor(wrapperType, boundType, boundType);
    }

    public static Opt<ConstructorInfo> GetGenericConstructor(this Type genericType, Type boundType, params Type[] constructorTypes)
    {
        if (!genericType.IsGenericType)
        {
            return Opt.None<ConstructorInfo>();
        }

        var boundGenericType = genericType.MakeGenericType(boundType);
        if (boundGenericType is null)
        {
            return Opt.None<ConstructorInfo>();
        }

        return boundGenericType.GetConstructor(constructorTypes).WrapOpt();
    }
}
