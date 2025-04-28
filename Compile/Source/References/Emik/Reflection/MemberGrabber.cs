// SPDX-License-Identifier: MPL-2.0
#if !NETSTANDARD || NETSTANDARD2_0_OR_GREATER
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Contains functions to create other functions that get or set fields and properties.</summary>
static partial class MemberGrabber
{
    /// <summary>The function containing the value to set.</summary>
    /// <typeparam name="T">The instance to mutate an inner value of.</typeparam>
    /// <typeparam name="TValue">The value to insert.</typeparam>
    public delegate void Setting<T, in TValue>(ref T obj, TValue value);

    /// <summary>Creates the getter function for the field.</summary>
    /// <param name="x">The field to generate the function for.</param>
    /// <returns>
    /// The function that get <paramref name="x"/>. The return type is <see cref="Converter{TInput, TOutput}"/>.
    /// </returns>
    // ReSharper disable NullableWarningSuppressionIsUsed
    public static Delegate Getter(this FieldInfo x)
    {
        DynamicMethod ret = new(x.Name, x.DeclaringType, [x.FieldType]);
        var il = ret.GetILGenerator();
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldfld, x);
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ret);
        return ret.CreateDelegate(typeof(Converter<,>).MakeGenericType(x.DeclaringType!, x.FieldType));
    }

    /// <summary>Creates the setter function for the field.</summary>
    /// <param name="x">The field to generate the function for.</param>
    /// <returns>
    /// The function that sets <paramref name="x"/>. The return type is <see cref="Setting{T, Value}"/>.
    /// </returns>
    public static Delegate Setter(this FieldInfo x)
    {
        DynamicMethod ret = new(x.Name, typeof(void), [x.DeclaringType!.MakeByRefType(), x.FieldType]);
        var il = ret.GetILGenerator();
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldarg_1);
        il.Emit(OpCodes.Stfld, x);
        il.Emit(OpCodes.Ret);
        return ret.CreateDelegate(typeof(Setting<,>).MakeGenericType(x.DeclaringType!, x.FieldType));
    }

    /// <summary>Creates the getter function for the field.</summary>
    /// <param name="x">The property to generate the function for.</param>
    /// <returns>
    /// The function that get <paramref name="x"/>. The return type is <see cref="Converter{TInput, TOutput}"/>.
    /// </returns>
    public static Delegate Getter(this PropertyInfo x)
    {
        DynamicMethod ret = new(x.Name, x.DeclaringType, [x.PropertyType]);
        var il = ret.GetILGenerator();
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Call, x.GetMethod!);
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ret);
        return ret.CreateDelegate(typeof(Converter<,>).MakeGenericType(x.DeclaringType!, x.PropertyType));
    }

    /// <summary>Creates the setter function for the field.</summary>
    /// <param name="x">The property to generate the function for.</param>
    /// <returns>
    /// The function that sets <paramref name="x"/>. The return type is <see cref="Setting{T, TValue}"/>.
    /// </returns>
    public static Delegate Setter(this PropertyInfo x)
    {
        DynamicMethod ret = new(x.Name, typeof(void), [x.DeclaringType!.MakeByRefType(), x.PropertyType]);
        var il = ret.GetILGenerator();
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldarg_1);
        il.Emit(OpCodes.Call, x.SetMethod!);
        il.Emit(OpCodes.Ret);
        return ret.CreateDelegate(typeof(Setting<,>).MakeGenericType(x.DeclaringType!, x.PropertyType));
    }
}
#endif
