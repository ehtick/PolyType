using System.Collections;
using System.Runtime.CompilerServices;
using PolyType.Abstractions;
using PolyType.ReflectionProvider;

#pragma warning disable IDE0060 // Remove unused parameter

namespace PolyType.Tests;

/// <summary>
/// Defines a set of factory methods for building test cases.
/// </summary>
public static class TestCase
{
#if NET
    /// <summary>
    /// Creates a new test case instance.
    /// </summary>
    /// <typeparam name="T">The type of test case.</typeparam>
    /// <param name="value">The value of the test case.</param>
    /// <param name="additionalValues">Any additional values to be tested.</param>
    /// <param name="hasRefConstructorParameters">Whether the shape constructor accepts any ref parameters.</param>
    /// <param name="hasOutConstructorParameters">Whether the shape constructor accepts any out parameters.</param>
    /// <param name="usesSpanConstructor">Whether the shape defines a collection constructor that takes a span of elements.</param>
    /// <param name="isSet">Whether the type is a set.</param>
    /// <param name="isStack">Whether the type is a stack.</param>
    /// <param name="isUnion">Whether the type is a union.</param>
    /// <returns>A test case instance using the specified parameters.</returns>
    public static TestCase<T> Create<T>(
        T? value,
        T?[]? additionalValues = null,
        bool hasRefConstructorParameters = false,
        bool hasOutConstructorParameters = false,
        bool usesSpanConstructor = false,
        bool isSet = false,
        bool isStack = false,
        bool isUnion = false)
        where T : IShapeable<T>
    {
        return new TestCase<T, T>(value)
        {
            AdditionalValues = additionalValues,
            HasRefConstructorParameters = hasRefConstructorParameters,
            HasOutConstructorParameters = hasOutConstructorParameters,
            UsesSpanConstructor = usesSpanConstructor,
            IsSet = isSet,
            IsStack = isStack,
            IsUnion = isUnion,
        };
    }

    /// <summary>
    /// Creates a new test case instance.
    /// </summary>
    /// <typeparam name="T">The type of test case.</typeparam>
    /// <typeparam name="TProvider">The type of the shape provider for the <typeparamref name="T"/>.</typeparam>
    /// <param name="value">The value of the test case.</param>
    /// <param name="provider">An instance of <typeparamref name="TProvider"/> to aid type inference.</param>
    /// <param name="additionalValues">Any additional values to be tested.</param>
    /// <param name="hasRefConstructorParameters">Whether the shape constructor accepts any ref parameters.</param>
    /// <param name="hasOutConstructorParameters">Whether the shape constructor accepts any out parameters.</param>
    /// <param name="usesSpanConstructor">Whether the shape defines a collection constructor that takes a span of elements.</param>
    /// <param name="isSet">Whether the type is a set.</param>
    /// <param name="isStack">Whether the type is a stack.</param>
    /// <param name="isUnion">Whether the type is a union.</param>
    /// <returns>A test case instance using the specified parameters.</returns>
    public static TestCase<T> Create<T, TProvider>(
        T? value,
        TProvider? provider,
        T?[]? additionalValues = null,
        bool hasRefConstructorParameters = false,
        bool hasOutConstructorParameters = false,
        bool usesSpanConstructor = false,
        bool isSet = false,
        bool isStack = false,
        bool isUnion = false)
        where TProvider : IShapeable<T>
    {
        return new TestCase<T, TProvider>(value)
        {
            AdditionalValues = additionalValues,
            HasRefConstructorParameters = hasRefConstructorParameters,
            HasOutConstructorParameters = hasOutConstructorParameters,
            UsesSpanConstructor = usesSpanConstructor,
            IsSet = isSet,
            IsStack = isStack,
            IsUnion = isUnion,
        };
    }
#endif

    /// <summary>
    /// Creates a new test case instance.
    /// </summary>
    /// <typeparam name="TProvider">The type of the shape provider for the <typeparamref name="T"/>.</typeparam>
    /// <typeparam name="T">The type of test case.</typeparam>
    /// <param name="value">The value of the test case.</param>
    /// <param name="provider">The shape provider used to resolve the shape of <typeparamref name="T"/>.</param>
    /// <param name="additionalValues">Any additional values to be tested.</param>
    /// <param name="hasRefConstructorParameters">Whether the shape constructor accepts any ref parameters.</param>
    /// <param name="hasOutConstructorParameters">Whether the shape constructor accepts any out parameters.</param>
    /// <param name="usesSpanConstructor">Whether the shape defines a collection constructor that takes a span of elements.</param>
    /// <param name="isSet">Whether the type is a set.</param>
    /// <param name="isStack">Whether the type is a stack.</param>
    /// <param name="isUnion">Whether the type is a union.</param>
    /// <returns>A test case instance using the specified parameters.</returns>
    public static TestCase<T> Create<T>(
        T? value,
        ITypeShapeProvider provider,
        T?[]? additionalValues = null,
        bool hasRefConstructorParameters = false,
        bool hasOutConstructorParameters = false,
        bool usesSpanConstructor = false,
        bool isSet = false,
        bool isStack = false,
        bool isUnion = false)
    {
        return new TestCase<T>(value, provider)
        {
            AdditionalValues = additionalValues,
            HasRefConstructorParameters = hasRefConstructorParameters,
            HasOutConstructorParameters = hasOutConstructorParameters,
            UsesSpanConstructor = usesSpanConstructor,
            IsSet = isSet,
            IsStack = isStack,
            IsUnion = isUnion,
        };
    }

#if !NET
    // Defines a Netfx polyfill for the internal `T : IShapeable<T>`
    // calls that hardcodes the built-in `Witness` shape provider.
    internal static TestCase<T> Create<T>(
        T? value,
        T?[]? additionalValues = null,
        bool hasRefConstructorParameters = false,
        bool hasOutConstructorParameters = false,
        bool usesSpanConstructor = false,
        bool isSet = false,
        bool isStack = false,
        bool isUnion = false)
    {
        return new TestCase<T>(value, Witness.ShapeProvider)
        {
            AdditionalValues = additionalValues,
            HasRefConstructorParameters = hasRefConstructorParameters,
            HasOutConstructorParameters = hasOutConstructorParameters,
            UsesSpanConstructor = usesSpanConstructor,
            IsSet = isSet,
            IsStack = isStack,
            IsUnion = isUnion,
        };
    }
#endif
}