﻿// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace EmptyNamespace
namespace System;
#if NET20 || NET30 // ReSharper disable TypeParameterCanBeVariant
/// <summary>Encapsulates a method that has no parameters and does not return a value.</summary>
delegate void Action();

/// <summary>Encapsulates a method that has two parameters and does not return a value.</summary>
/// <typeparam name="T1">The type of the first parameter of the method that this delegate encapsulates.</typeparam>
/// <typeparam name="T2">The type of the second parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg1">The first parameter of the method that this delegate encapsulates.</param>
/// <param name="arg2">The second parameter of the method that this delegate encapsulates.</param>
delegate void Action<T1, T2>(T1 arg1, T2 arg2);

/// <summary>Encapsulates a method that has three parameters and does not return a value.</summary>
/// <typeparam name="T1">The type of the first parameter of the method that this delegate encapsulates.</typeparam>
/// <typeparam name="T2">The type of the second parameter of the method that this delegate encapsulates.</typeparam>
/// <typeparam name="T3">The type of the third parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg1">The first parameter of the method that this delegate encapsulates.</param>
/// <param name="arg2">The second parameter of the method that this delegate encapsulates.</param>
/// <param name="arg3">The third parameter of the method that this delegate encapsulates.</param>
delegate void Action<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3);

/// <summary>Encapsulates a method that has four parameters and does not return a value.</summary>
/// <typeparam name="T1">The type of the first parameter of the method that this delegate encapsulates.</typeparam>
/// <typeparam name="T2">The type of the second parameter of the method that this delegate encapsulates.</typeparam>
/// <typeparam name="T3">The type of the third parameter of the method that this delegate encapsulates.</typeparam>
/// <typeparam name="T4">The type of the fourth parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg1">The first parameter of the method that this delegate encapsulates.</param>
/// <param name="arg2">The second parameter of the method that this delegate encapsulates.</param>
/// <param name="arg3">The third parameter of the method that this delegate encapsulates.</param>
/// <param name="arg4">The fourth parameter of the method that this delegate encapsulates.</param>
delegate void Action<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);

/// <summary>
/// Encapsulates a method that has no parameters and returns a value of
/// the type specified by the <typeparamref name="TResult"/> parameter.
/// </summary>
/// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates.</typeparam>
/// <returns>The return value of the method that this delegate encapsulates.</returns>
delegate TResult Func<TResult>();

/// <summary>
/// Encapsulates a method that has no parameters and returns a value of
/// the type specified by the <typeparamref name="TResult"/> parameter.
/// </summary>
/// <typeparam name="T">The type of the parameter of the method that this delegate encapsulates.</typeparam>
/// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates.</typeparam>
/// <param name="arg">The parameter of the method that this delegate encapsulates.</param>
/// <returns>The return value of the method that this delegate encapsulates.</returns>
delegate TResult Func<T, TResult>(T arg);

/// <summary>
/// Encapsulates a method that has no parameters and returns a value of
/// the type specified by the <typeparamref name="TResult"/> parameter.
/// </summary>
/// <typeparam name="T1">The type of the first parameter of the method that this delegate encapsulates.</typeparam>
/// <typeparam name="T2">The type of the second parameter of the method that this delegate encapsulates.</typeparam>
/// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates.</typeparam>
/// <param name="arg1">The first parameter of the method that this delegate encapsulates.</param>
/// <param name="arg2">The second parameter of the method that this delegate encapsulates.</param>
/// <returns>The return value of the method that this delegate encapsulates.</returns>
delegate TResult Func<T1, T2, TResult>(T1 arg1, T2 arg2);

/// <summary>
/// Encapsulates a method that has no parameters and returns a value of
/// the type specified by the <typeparamref name="TResult"/> parameter.
/// </summary>
/// <typeparam name="T1">The type of the first parameter of the method that this delegate encapsulates.</typeparam>
/// <typeparam name="T2">The type of the second parameter of the method that this delegate encapsulates.</typeparam>
/// <typeparam name="T3">The type of the third parameter of the method that this delegate encapsulates.</typeparam>
/// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates.</typeparam>
/// <param name="arg1">The first parameter of the method that this delegate encapsulates.</param>
/// <param name="arg2">The second parameter of the method that this delegate encapsulates.</param>
/// <param name="arg3">The third parameter of the method that this delegate encapsulates.</param>
/// <returns>The return value of the method that this delegate encapsulates.</returns>
delegate TResult Func<T1, T2, T3, TResult>(T1 arg1, T2 arg2, T3 arg3);

/// <summary>
/// Encapsulates a method that has no parameters and returns a value of
/// the type specified by the <typeparamref name="TResult"/> parameter.
/// </summary>
/// <typeparam name="T1">The type of the first parameter of the method that this delegate encapsulates.</typeparam>
/// <typeparam name="T2">The type of the second parameter of the method that this delegate encapsulates.</typeparam>
/// <typeparam name="T3">The type of the third parameter of the method that this delegate encapsulates.</typeparam>
/// <typeparam name="T4">The type of the fourth parameter of the method that this delegate encapsulates.</typeparam>
/// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates.</typeparam>
/// <param name="arg1">The first parameter of the method that this delegate encapsulates.</param>
/// <param name="arg2">The second parameter of the method that this delegate encapsulates.</param>
/// <param name="arg3">The third parameter of the method that this delegate encapsulates.</param>
/// <param name="arg4">The fourth parameter of the method that this delegate encapsulates.</param>
/// <returns>The return value of the method that this delegate encapsulates.</returns>
delegate TResult Func<T1, T2, T3, T4, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
#elif NET35
/// <summary>Encapsulates a method that has four parameters and does not return a value.</summary>
/// <typeparam name="T1">The type of the first parameter of the method that this delegate encapsulates.</typeparam>
/// <typeparam name="T2">The type of the second parameter of the method that this delegate encapsulates.</typeparam>
/// <typeparam name="T3">The type of the third parameter of the method that this delegate encapsulates.</typeparam>
/// <typeparam name="T4">The type of the fourth parameter of the method that this delegate encapsulates.</typeparam>
/// <typeparam name="T5">The type of the fifth parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg1">The first parameter of the method that this delegate encapsulates.</param>
/// <param name="arg2">The second parameter of the method that this delegate encapsulates.</param>
/// <param name="arg3">The third parameter of the method that this delegate encapsulates.</param>
/// <param name="arg4">The fourth parameter of the method that this delegate encapsulates.</param>
/// <param name="arg5">The fifth parameter of the method that this delegate encapsulates.</param>
delegate void Action<T1, T2, T3, T4, T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);

/// <summary>Encapsulates a method that has four parameters and does not return a value.</summary>
/// <typeparam name="T1">The type of the first parameter of the method that this delegate encapsulates.</typeparam>
/// <typeparam name="T2">The type of the second parameter of the method that this delegate encapsulates.</typeparam>
/// <typeparam name="T3">The type of the third parameter of the method that this delegate encapsulates.</typeparam>
/// <typeparam name="T4">The type of the fourth parameter of the method that this delegate encapsulates.</typeparam>
/// <typeparam name="T5">The type of the fifth parameter of the method that this delegate encapsulates.</typeparam>
/// <typeparam name="T6">The type of the sixth parameter of the method that this delegate encapsulates.</typeparam>
/// <param name="arg1">The first parameter of the method that this delegate encapsulates.</param>
/// <param name="arg2">The second parameter of the method that this delegate encapsulates.</param>
/// <param name="arg3">The third parameter of the method that this delegate encapsulates.</param>
/// <param name="arg4">The fourth parameter of the method that this delegate encapsulates.</param>
/// <param name="arg5">The fifth parameter of the method that this delegate encapsulates.</param>
/// <param name="arg6">The sixth parameter of the method that this delegate encapsulates.</param>
delegate void Action<T1, T2, T3, T4, T5, T6>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);

/// <summary>
/// Encapsulates a method that has no parameters and returns a value of
/// the type specified by the <typeparamref name="TResult"/> parameter.
/// </summary>
/// <typeparam name="T1">The type of the first parameter of the method that this delegate encapsulates.</typeparam>
/// <typeparam name="T2">The type of the second parameter of the method that this delegate encapsulates.</typeparam>
/// <typeparam name="T3">The type of the third parameter of the method that this delegate encapsulates.</typeparam>
/// <typeparam name="T4">The type of the fourth parameter of the method that this delegate encapsulates.</typeparam>
/// <typeparam name="T5">The type of the fifth parameter of the method that this delegate encapsulates.</typeparam>
/// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates.</typeparam>
/// <param name="arg1">The first parameter of the method that this delegate encapsulates.</param>
/// <param name="arg2">The second parameter of the method that this delegate encapsulates.</param>
/// <param name="arg3">The third parameter of the method that this delegate encapsulates.</param>
/// <param name="arg4">The fourth parameter of the method that this delegate encapsulates.</param>
/// <param name="arg5">The fifth parameter of the method that this delegate encapsulates.</param>
/// <returns>The return value of the method that this delegate encapsulates.</returns>
delegate TResult Func<T1, T2, T3, T4, T5, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);

/// <summary>
/// Encapsulates a method that has no parameters and returns a value of
/// the type specified by the <typeparamref name="TResult"/> parameter.
/// </summary>
/// <typeparam name="T1">The type of the first parameter of the method that this delegate encapsulates.</typeparam>
/// <typeparam name="T2">The type of the second parameter of the method that this delegate encapsulates.</typeparam>
/// <typeparam name="T3">The type of the third parameter of the method that this delegate encapsulates.</typeparam>
/// <typeparam name="T4">The type of the fourth parameter of the method that this delegate encapsulates.</typeparam>
/// <typeparam name="T5">The type of the fifth parameter of the method that this delegate encapsulates.</typeparam>
/// <typeparam name="T6">The type of the sixth parameter of the method that this delegate encapsulates.</typeparam>
/// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates.</typeparam>
/// <param name="arg1">The first parameter of the method that this delegate encapsulates.</param>
/// <param name="arg2">The second parameter of the method that this delegate encapsulates.</param>
/// <param name="arg3">The third parameter of the method that this delegate encapsulates.</param>
/// <param name="arg4">The fourth parameter of the method that this delegate encapsulates.</param>
/// <param name="arg5">The fifth parameter of the method that this delegate encapsulates.</param>
/// <param name="arg6">The sixth parameter of the method that this delegate encapsulates.</param>
/// <returns>The return value of the method that this delegate encapsulates.</returns>
delegate TResult Func<T1, T2, T3, T4, T5, T6, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
#elif NETSTANDARD && !NETSTANDARD2_0_OR_GREATER // ReSharper disable once RedundantBlankLines
/// <summary>Represents a method that converts an object from one type to another type.</summary>
/// <typeparam name="TInput">The type of object that is to be converted.</typeparam>
/// <typeparam name="TOutput">The type the input object is to be converted to.</typeparam>
/// <param name="input">The object to convert.</param>
/// <returns>The <typeparamref name="TOutput"/> that represents the converted <typeparamref name="TInput"/>.</returns>
delegate TOutput Converter<in TInput, out TOutput>(TInput input);
#endif
