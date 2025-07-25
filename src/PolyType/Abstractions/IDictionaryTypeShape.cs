﻿using System.Collections;

namespace PolyType.Abstractions;

/// <summary>
/// Provides a strongly typed shape model for a .NET type that is a dictionary.
/// </summary>
/// <remarks>
/// Typically covers types implementing interfaces such as <see cref="IDictionary{TKey, TValue}"/>,
/// <see cref="IReadOnlyDictionary{TKey, TValue}"/> or <see cref="IDictionary"/>.
/// </remarks>
[InternalImplementationsOnly]
public interface IDictionaryTypeShape : ITypeShape
{
    /// <summary>
    /// Gets the shape of the underlying key type.
    /// </summary>
    /// <remarks>
    /// For non-generic dictionaries this returns the shape for <see cref="object"/>.
    /// </remarks>
    ITypeShape KeyType { get; }

    /// <summary>
    /// Gets the shape of the underlying value type.
    /// </summary>
    /// <remarks>
    /// For non-generic dictionaries this returns the shape for <see cref="object"/>.
    /// </remarks>
    ITypeShape ValueType { get; }

    /// <summary>
    /// Gets the construction strategy for the given collection.
    /// </summary>
    CollectionConstructionStrategy ConstructionStrategy { get; }

    /// <summary>
    /// Gets the kind of custom comparer (if any) that this collection may be initialized with.
    /// </summary>
    CollectionComparerOptions SupportedComparer { get; }

    /// <summary>
    /// Gets the available insertion modes supported by the dictionary type.
    /// </summary>
    DictionaryInsertionMode AvailableInsertionModes { get; }
}

/// <summary>
/// Provides a strongly typed shape model for a .NET type that is a dictionary.
/// </summary>
/// <typeparam name="TDictionary">The type of the underlying dictionary.</typeparam>
/// <typeparam name="TKey">The type of the underlying key.</typeparam>
/// <typeparam name="TValue">The type of the underlying value.</typeparam>
/// <remarks>
/// Typically covers types implementing interfaces such as <see cref="IDictionary{TKey, TValue}"/>,
/// <see cref="IReadOnlyDictionary{TKey, TValue}"/> or <see cref="IDictionary"/>.
/// </remarks>
[InternalImplementationsOnly]
public interface IDictionaryTypeShape<TDictionary, TKey, TValue> : ITypeShape<TDictionary>, IDictionaryTypeShape
    where TKey : notnull
{
    /// <summary>
    /// Gets the shape of the underlying key type.
    /// </summary>
    /// <remarks>
    /// For non-generic dictionaries this returns the shape for <see cref="object"/>.
    /// </remarks>
    new ITypeShape<TKey> KeyType { get; }

    /// <summary>
    /// Gets the shape of the underlying value type.
    /// </summary>
    /// <remarks>
    /// For non-generic dictionaries this returns the shape for <see cref="object"/>.
    /// </remarks>
    new ITypeShape<TValue> ValueType { get; }

    /// <summary>
    /// Creates a delegate used for getting a <see cref="IReadOnlyDictionary{TKey, TValue}"/> view of the dictionary.
    /// </summary>
    /// <returns>
    /// A delegate accepting a <typeparamref name="TDictionary"/> and
    /// returning an <see cref="IReadOnlyDictionary{TKey, TValue}"/> view of the instance.
    /// </returns>
    Func<TDictionary, IReadOnlyDictionary<TKey, TValue>> GetGetDictionary();

    /// <summary>
    /// Creates a delegate for creating an empty, mutable collection.
    /// </summary>
    /// <exception cref="InvalidOperationException">The collection is not <see cref="CollectionConstructionStrategy.Mutable"/>.</exception>
    /// <returns>A delegate for creating an empty mutable collection.</returns>
    MutableCollectionConstructor<TKey, TDictionary> GetMutableConstructor();

    /// <summary>
    /// Creates a delegate used for inserting a new key/value pair to a mutable dictionary.
    /// </summary>
    /// <param name="insertionMode">Specifies the duplicate key handling strategy used by the delegate.</param>
    /// <exception cref="InvalidOperationException">The collection is not <see cref="CollectionConstructionStrategy.Mutable"/>.</exception>
    /// <returns>A delegate used for inserting entries into a mutable dictionary.</returns>
    DictionaryInserter<TDictionary, TKey, TValue> GetInserter(DictionaryInsertionMode insertionMode = DictionaryInsertionMode.None);

    /// <summary>
    /// Creates a delegate for creating a collection from a span.
    /// </summary>
    /// <exception cref="InvalidOperationException">The collection is not <see cref="CollectionConstructionStrategy.Parameterized"/>.</exception>
    /// <returns>A delegate constructing a collection from a span of values.</returns>
    ParameterizedCollectionConstructor<TKey, KeyValuePair<TKey, TValue>, TDictionary> GetParameterizedConstructor();
}
