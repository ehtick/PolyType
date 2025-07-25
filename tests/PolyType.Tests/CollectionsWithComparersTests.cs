﻿using System.Collections;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using PolyType.Examples.Utilities;

namespace PolyType.Tests;

[Trait("CustomComparers", "true")]
public abstract partial class CollectionsWithComparersTests(ProviderUnderTest providerUnderTest)
{
    private static readonly KeyValuePair<int, bool>[] NonEmptyDictionary = [new KeyValuePair<int, bool>(3, true)];
    private static readonly int[] NonEmptyEnumerable = [3, 6];

    [Fact]
    public void Dictionary() => this.AssertDefaultDictionary<Dictionary<int, bool>, int, bool>(new EvenOddEqualityComparer(), d => d.Comparer);

    [Fact]
    public void IDictionary() => this.AssertDefaultDictionary<IDictionary<int, bool>, int, bool>(new EvenOddEqualityComparer(), d => ((Dictionary<int, bool>)d).Comparer);

    [Fact]
    public void SortedDictionary() => this.AssertDefaultDictionary<SortedDictionary<int, bool>, int, bool>(new ReverseComparer(), d => d.Comparer);

    [Fact]
    public void ReadOnlyDictionary()
    {
        KeyValuePair<int, bool>[] values = [new(3, true), new(6, true), new(5, true)];
        ReadOnlyDictionary<int, bool> dict = this.CreateParameterizedDictionary<ReadOnlyDictionary<int, bool>, int, bool>(values, new EvenOddEqualityComparer());

        // We have to get creative when testing the comparer backing a ReadOnlyDictionary because that type doesn't expose it.
        // We provided 3 key=value pairs, but with our custom EqualityComparer, only 2 unique keys are present.
        Assert.Equal(2, dict.Count);

        dict = this.CreateParameterizedDictionary<ReadOnlyDictionary<int, bool>, int, bool>(values, equalityComparer: null);
        Assert.Equal(values.ToArray(), dict);
    }

    [Fact]
    public void ImmutableDictionary() => this.AssertParameterizedDictionary<ImmutableDictionary<int, bool>, int, bool>(NonEmptyDictionary, new EvenOddEqualityComparer(), d => d.KeyComparer);

    [Fact]
    public void ImmutableSortedDictionary() => this.AssertParameterizedDictionary<ImmutableSortedDictionary<int, bool>, int, bool>(NonEmptyDictionary, new ReverseComparer(), d => d.KeyComparer);

    [Fact]
    public void DictionaryBySpan() => this.AssertParameterizedDictionary<DictionarySpan, int, bool>(NonEmptyDictionary, new EvenOddEqualityComparer(), d => d.Comparer);

    [Fact]
    public void SortedDictionaryBySpan() => this.AssertParameterizedDictionary<SortedDictionarySpan, int, bool>(NonEmptyDictionary, new ReverseComparer(), d => d.Comparer);

    [Fact]
    public void DictionaryValuesThenComparer() => this.AssertParameterizedDictionary<DictionaryValuesEC, int, bool>(NonEmptyDictionary, new EvenOddEqualityComparer(), d => d.Comparer);

    [Fact]
    public void SortedDictionaryValuesThenComparer() => this.AssertParameterizedDictionary<SortedDictionaryValuesC, int, bool>(NonEmptyDictionary, new ReverseComparer(), d => d.Comparer);

    [Fact]
    public void HashSet() => this.AssertDefaultEnumerable<HashSet<int>, int>(new EvenOddEqualityComparer(), s => s.Comparer);

    [Fact]
    public void HashSetOfEqualityComparers()
    {
        HashSet<IEqualityComparer<int>> set = this.CreateDefaultEnumerable<HashSet<IEqualityComparer<int>>, IEqualityComparer<int>>(new EqualityComparerOfEqualityComparers());
        Assert.IsType<EqualityComparerOfEqualityComparers>(set.Comparer);
    }

    [Fact]
    public void DictionaryOfEqualityComparers()
    {
        Dictionary<IEqualityComparer<int>, int> dict = this.CreateDefaultDictionary<Dictionary<IEqualityComparer<int>, int>, IEqualityComparer<int>, int>(new EqualityComparerOfEqualityComparers());
        Assert.IsType<EqualityComparerOfEqualityComparers>(dict.Comparer);
    }

    [Fact]
    public void SortedSet() => this.AssertDefaultEnumerable<SortedSet<int>, int>(new ReverseComparer(), s => s.Comparer);

    // REVISIT: This test is skipped for no-emit Reflection because it uses Span, which isn't supported by that provider.
    //          Consider adding an array/list construction strategy for better support.
    [Fact]
    public void ImmutableHashSet()
    {
        this.AssertParameterizedEnumerable<ImmutableHashSet<int>, int>(NonEmptyEnumerable, new EvenOddEqualityComparer(), s => s.KeyComparer);
    }

    // REVISIT: This test is skipped for no-emit Reflection because it uses Span, which isn't supported by that provider.
    //          Consider adding an array/list construction strategy for better support.
    [Fact]
    public void IImmutableSet()
    {
        var enumerable = this.CreateParameterizedEnumerable<IImmutableSet<int>, int>([3, 6, 7], new EvenOddEqualityComparer());
        AssertEquivalentContent([3, 6], enumerable);
    }

    // REVISIT: This test is skipped for no-emit Reflection because it uses Span, which isn't supported by that provider.
    //          Consider adding an array/list construction strategy for better support.
    [Fact]
    public void ImmutableSortedSet()
    {
        this.AssertParameterizedEnumerable<ImmutableSortedSet<int>, int>(NonEmptyEnumerable, new ReverseComparer(), s => s.KeyComparer);
    }

    [Fact]
    public void EnumerableByEnumerableEC() => this.AssertParameterizedEnumerable<SpanEnumerableEC, int>(NonEmptyEnumerable, new EvenOddEqualityComparer(), n => n.Comparer);

    [Fact]
    public void EnumerableByECEnumerable() => this.AssertParameterizedEnumerable<EnumerableECEnumerable, int>(NonEmptyEnumerable, new EvenOddEqualityComparer(), n => n.Comparer);

    [Fact]
    public void EnumerableByEnumerableC() => this.AssertParameterizedEnumerable<SpanEnumerableC, int>(NonEmptyEnumerable, new ReverseComparer(), n => n.Comparer);

    [Fact]
    public void EnumerableByCEnumerable() => this.AssertParameterizedEnumerable<EnumerableCEnumerable, int>(NonEmptyEnumerable, new ReverseComparer(), n => n.Comparer);

    // REVISIT: This test is skipped for no-emit Reflection because it uses Span, which isn't supported by that provider.
    //          Consider adding an array/list construction strategy for better support.
    [Fact]
    public void EnumerableByECList() => this.AssertParameterizedEnumerable<EnumerableECList, int>(NonEmptyEnumerable, new EvenOddEqualityComparer(), l => l.Comparer);

    // REVISIT: This test is skipped for no-emit Reflection because it uses Span, which isn't supported by that provider.
    //          Consider adding an array/list construction strategy for better support.
    [Fact]
    public void EnumerableByCList() => this.AssertParameterizedEnumerable<EnumerableCList, int>(NonEmptyEnumerable, new ReverseComparer(), l => l.Comparer);

    // REVISIT: This test is skipped for no-emit Reflection because it uses Span, which isn't supported by that provider.
    //          Consider adding an array/list construction strategy for better support.
    [Fact]
    public void EnumerableByListEC() => this.AssertParameterizedEnumerable<EnumerableListEC, int>(NonEmptyEnumerable, new EvenOddEqualityComparer(), l => l.Comparer);

    // REVISIT: This test is skipped for no-emit Reflection because it uses Span, which isn't supported by that provider.
    //          Consider adding an array/list construction strategy for better support.
    [Fact]
    public void EnumerableByListC() => this.AssertParameterizedEnumerable<EnumerableListC, int>(NonEmptyEnumerable, new ReverseComparer(), l => l.Comparer);

    [Fact]
    public void EnumerableByECSpan() => this.AssertParameterizedEnumerable<EnumerableECSpan, int>(NonEmptyEnumerable, new EvenOddEqualityComparer(), l => l.Comparer);

    [Fact]
    public void EnumerableByCSpan() => this.AssertParameterizedEnumerable<EnumerableCSpan, int>(NonEmptyEnumerable, new ReverseComparer(), l => l.Comparer);

    [Fact]
    public void NoComparerCollections()
    {
        Assert.Equal(CollectionComparerOptions.None, this.GetEnumerableShape<ReadOnlyMemory<int>, int>().SupportedComparer);
        Assert.Equal(CollectionComparerOptions.None, this.GetEnumerableShape<Memory<int>, int>().SupportedComparer);
        Assert.Equal(CollectionComparerOptions.None, this.GetEnumerableShape<int[], int>().SupportedComparer);
        Assert.Equal(CollectionComparerOptions.None, this.GetEnumerableShape<int[,], int>().SupportedComparer);
    }

    private void AssertDefaultDictionary<T, K, V>(IComparer<K> comparer, Func<T, IComparer<K>> getComparer)
        where K : notnull
    {
        T dict = this.CreateDefaultDictionary<T, K, V>(comparer);
        Assert.Same(comparer, getComparer(dict));

        dict = this.CreateDefaultDictionary<T, K, V>(comparer: null);
        Assert.NotNull(getComparer(dict));
    }

    private void AssertDefaultDictionary<T, K, V>(IEqualityComparer<K> equalityComparer, Func<T, IEqualityComparer<K>> getComparer)
        where K : notnull
    {
        T dict = this.CreateDefaultDictionary<T, K, V>(equalityComparer);
        Assert.Same(equalityComparer, getComparer(dict));

        dict = this.CreateDefaultDictionary<T, K, V>(equalityComparer: null);
        Assert.NotNull(getComparer(dict));
    }

    private void AssertParameterizedDictionary<T, K, V>(ReadOnlySpan<KeyValuePair<K, V>> values, IComparer<K> comparer, Func<T, IComparer<K>> getComparer)
        where K : notnull
        where T : IEnumerable<KeyValuePair<K, V>>
    {
        T dict = this.CreateParameterizedDictionary<T, K, V>(values, comparer);
        Assert.Same(comparer, getComparer(dict));
        Assert.Equal(values.ToArray(), dict);

        dict = this.CreateParameterizedDictionary<T, K, V>(values, comparer: null);
        Assert.NotNull(getComparer(dict));
        Assert.Equal(values.ToArray(), dict);
    }

    private void AssertParameterizedDictionary<T, K, V>(ReadOnlySpan<KeyValuePair<K, V>> values, IEqualityComparer<K> equalityComparer, Func<T, IEqualityComparer<K>> getComparer)
        where K : notnull
        where T : IEnumerable<KeyValuePair<K, V>>
    {
        T dict = this.CreateParameterizedDictionary<T, K, V>(values, equalityComparer);
        Assert.Same(equalityComparer, getComparer(dict));
        Assert.Equal(values.ToArray(), dict);

        dict = this.CreateParameterizedDictionary<T, K, V>(values, equalityComparer: null);
        Assert.NotNull(getComparer(dict));
        Assert.Equal(values.ToArray(), dict);
    }

    private T CreateDefaultDictionary<T, K, V>(IComparer<K>? comparer)
        where K : notnull
    {
        IDictionaryTypeShape<T, K, V> shape = this.GetDictionaryShape<T, K, V>();
        Assert.Equal(CollectionComparerOptions.Comparer, shape.SupportedComparer);
        return shape.GetMutableConstructor()(new() { Comparer = comparer });
    }

    private T CreateDefaultDictionary<T, K, V>(IEqualityComparer<K>? equalityComparer)
        where K : notnull
    {
        IDictionaryTypeShape<T, K, V> shape = this.GetDictionaryShape<T, K, V>();
        Assert.Equal(CollectionComparerOptions.EqualityComparer, shape.SupportedComparer);
        return shape.GetMutableConstructor()(new() { EqualityComparer = equalityComparer });
    }

    private T CreateParameterizedDictionary<T, K, V>(ReadOnlySpan<KeyValuePair<K, V>> values, IComparer<K>? comparer)
        where K : notnull
    {
        Assert.SkipWhen(providerUnderTest is ReflectionProviderUnderTest { Kind: ProviderKind.ReflectionNoEmit }, "Reflection (no-emit) does not support span collections.");
        IDictionaryTypeShape<T, K, V> shape = this.GetDictionaryShape<T, K, V>();
        Assert.Equal(CollectionComparerOptions.Comparer, shape.SupportedComparer);
        Assert.Equal(CollectionConstructionStrategy.Parameterized, shape.ConstructionStrategy);
        return shape.GetParameterizedConstructor()(values, new() { Comparer = comparer });
    }

    private T CreateParameterizedDictionary<T, K, V>(ReadOnlySpan<KeyValuePair<K, V>> values, IEqualityComparer<K>? equalityComparer)
        where K : notnull
    {
        Assert.SkipWhen(providerUnderTest is ReflectionProviderUnderTest { Kind: ProviderKind.ReflectionNoEmit }, "Reflection (no-emit) does not support span collections.");
        IDictionaryTypeShape<T, K, V> shape = this.GetDictionaryShape<T, K, V>();
        Assert.Equal(CollectionComparerOptions.EqualityComparer, shape.SupportedComparer);
        Assert.Equal(CollectionConstructionStrategy.Parameterized, shape.ConstructionStrategy);
        return shape.GetParameterizedConstructor()(values, new() { EqualityComparer = equalityComparer });
    }

    private IDictionaryTypeShape<T, K, V> GetDictionaryShape<T, K, V>()
        where K : notnull
    {
        var shape = (IDictionaryTypeShape<T, K, V>?)providerUnderTest.Provider.GetShape(typeof(T));
        Assert.NotNull(shape);
        return shape;
    }

    private void AssertDefaultEnumerable<T, K>(IComparer<K> comparer, Func<T, IComparer<K>> getComparer)
        where K : notnull
    {
        T dict = this.CreateDefaultEnumerable<T, K>(comparer);
        Assert.Same(comparer, getComparer(dict));

        dict = this.CreateDefaultEnumerable<T, K>(comparer: null);
        Assert.NotNull(getComparer(dict));
    }

    private void AssertDefaultEnumerable<T, K>(IEqualityComparer<K> equalityComparer, Func<T, IEqualityComparer<K>> getComparer)
        where K : notnull
    {
        T dict = this.CreateDefaultEnumerable<T, K>(equalityComparer);
        Assert.Same(equalityComparer, getComparer(dict));

        dict = this.CreateDefaultEnumerable<T, K>(equalityComparer: null);
        Assert.NotNull(getComparer(dict));
    }

    private void AssertParameterizedEnumerable<T, K>(ReadOnlySpan<K> values, IComparer<K> comparer, Func<T, IComparer<K>> getComparer)
        where K : notnull
        where T : IEnumerable<K>
    {
        T enumerable = this.CreateParameterizedEnumerable<T, K>(values, comparer);
        Assert.Same(comparer, getComparer(enumerable));
        AssertEquivalentContent(values, enumerable);

        enumerable = this.CreateParameterizedEnumerable<T, K>(values, comparer: null);
        Assert.NotNull(getComparer(enumerable));
        AssertEquivalentContent(values, enumerable);
    }

    private void AssertParameterizedEnumerable<T, K>(ReadOnlySpan<K> values, IEqualityComparer<K> equalityComparer, Func<T, IEqualityComparer<K>> getComparer)
        where K : notnull
        where T : IEnumerable<K>
    {
        T enumerable = this.CreateParameterizedEnumerable<T, K>(values, equalityComparer);
        Assert.Same(equalityComparer, getComparer(enumerable));
        AssertEquivalentContent(values, enumerable);

        enumerable = this.CreateParameterizedEnumerable<T, K>(values, equalityComparer: null);
        Assert.NotNull(getComparer(enumerable));
        AssertEquivalentContent(values, enumerable);
    }

    private static void AssertEquivalentContent<T>(ReadOnlySpan<T> expected, IEnumerable<T> actual) => Assert.Equal(expected.ToArray().OrderBy(v => v), actual.OrderBy(v => v));
    private static void AssertEquivalentContent<T>(IEnumerable<T> expected, IEnumerable<T> actual) => Assert.Equal(expected.OrderBy(v => v), actual.OrderBy(v => v));

    private T CreateDefaultEnumerable<T, K>(IComparer<K>? comparer)
    {
        IEnumerableTypeShape<T, K> shape = this.GetEnumerableShape<T, K>();
        Assert.Equal(CollectionComparerOptions.Comparer, shape.SupportedComparer);
        Assert.Equal(CollectionConstructionStrategy.Mutable, shape.ConstructionStrategy);
        return shape.GetMutableConstructor()(new() { Comparer = comparer });
    }

    private T CreateDefaultEnumerable<T, K>(IEqualityComparer<K>? equalityComparer)
    {
        IEnumerableTypeShape<T, K> shape = this.GetEnumerableShape<T, K>();
        Assert.Equal(CollectionComparerOptions.EqualityComparer, shape.SupportedComparer);
        Assert.Equal(CollectionConstructionStrategy.Mutable, shape.ConstructionStrategy);
        return shape.GetMutableConstructor()(new() { EqualityComparer = equalityComparer });
    }

    private T CreateParameterizedEnumerable<T, K>(ReadOnlySpan<K> values, IComparer<K>? comparer)
    {
        Assert.SkipWhen(providerUnderTest is ReflectionProviderUnderTest { Kind: ProviderKind.ReflectionNoEmit }, "Reflection (no-emit) does not support span collections.");
        IEnumerableTypeShape<T, K> shape = this.GetEnumerableShape<T, K>();
        Assert.Equal(CollectionConstructionStrategy.Parameterized, shape.ConstructionStrategy);
        Assert.Equal(CollectionComparerOptions.Comparer, shape.SupportedComparer);
        return shape.GetParameterizedConstructor()(values, new() { Comparer = comparer });
    }

    private T CreateParameterizedEnumerable<T, K>(ReadOnlySpan<K> values, IEqualityComparer<K>? equalityComparer)
    {
        Assert.SkipWhen(providerUnderTest is ReflectionProviderUnderTest { Kind: ProviderKind.ReflectionNoEmit }, "Reflection (no-emit) does not support span collections.");
        IEnumerableTypeShape<T, K> shape = this.GetEnumerableShape<T, K>();
        Assert.Equal(CollectionConstructionStrategy.Parameterized, shape.ConstructionStrategy);
        Assert.Equal(CollectionComparerOptions.EqualityComparer, shape.SupportedComparer);
        return shape.GetParameterizedConstructor()(values, new() { EqualityComparer = equalityComparer });
    }

    private IEnumerableTypeShape<T, K> GetEnumerableShape<T, K>()
    {
        var shape = (IEnumerableTypeShape<T, K>?)providerUnderTest.Provider.GetShape(typeof(T));
        Assert.NotNull(shape);
        return shape;
    }

    [GenerateShapeFor<Dictionary<int, bool>>]
    [GenerateShapeFor<IDictionary<int, bool>>]
    [GenerateShapeFor<ReadOnlyDictionary<int, bool>>]
    [GenerateShapeFor<SortedDictionary<int, bool>>]
    [GenerateShapeFor<ImmutableDictionary<int, bool>>]
    [GenerateShapeFor<ImmutableSortedDictionary<int, bool>>]
    [GenerateShapeFor<HashSet<int>>]
    [GenerateShapeFor<SortedSet<int>>]
    [GenerateShapeFor<ImmutableHashSet<int>>]
    [GenerateShapeFor<ImmutableSortedSet<int>>]
    [GenerateShapeFor<IImmutableSet<int>>]
    [GenerateShapeFor<HashSet<IEqualityComparer<int>>>]
    [GenerateShapeFor<Dictionary<IEqualityComparer<int>, int>>]
    [GenerateShapeFor<ReadOnlyMemory<int>>]
    [GenerateShapeFor<Memory<int>>]
    [GenerateShapeFor<int[]>]
    [GenerateShapeFor<int[,]>]
    partial class Witness;

    private class ReverseComparer : IComparer<int>
    {
        public int Compare(int x, int y) => -Comparer<int>.Default.Compare(x, y);
    }

    [ExcludeFromCodeCoverage]
    private class EvenOddEqualityComparer : IEqualityComparer<int>
    {
        public bool Equals(int x, int y) => x % 2 == y % 2;

        public int GetHashCode([DisallowNull] int obj) => obj % 2;
    }

    [ExcludeFromCodeCoverage]
    private class EqualityComparerOfEqualityComparers : IEqualityComparer<IEqualityComparer<int>>
    {
        public bool Equals(IEqualityComparer<int>? x, IEqualityComparer<int>? y) => object.ReferenceEquals(x, y);

        public int GetHashCode([DisallowNull] IEqualityComparer<int> obj) => obj.GetHashCode();
    }

    [GenerateShape, ExcludeFromCodeCoverage]
    internal partial class EnumerableECEnumerable(IEqualityComparer<int>? eq, IEnumerable<int> values) : IEnumerable<int>
    {
        public EnumerableECEnumerable(IEnumerable<int> values) : this(null, values) { }

        public IEqualityComparer<int> Comparer => eq ?? EqualityComparer<int>.Default;

        public IEnumerator<int> GetEnumerator() => values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }

    [GenerateShape, ExcludeFromCodeCoverage]
    internal partial class SpanEnumerableEC(IEnumerable<int> values, IEqualityComparer<int>? eq) : IEnumerable<int>
    {
        public SpanEnumerableEC(IEnumerable<int> values) : this(values, null) { }

        public IEqualityComparer<int> Comparer => eq ?? EqualityComparer<int>.Default;

        public IEnumerator<int> GetEnumerator() => values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }

    [GenerateShape, ExcludeFromCodeCoverage]
    internal partial class SpanEnumerableC(IEnumerable<int> values, IComparer<int>? comparer) : IEnumerable<int>
    {
        public SpanEnumerableC(IEnumerable<int> values) : this(values, null) { }

        public IComparer<int> Comparer => comparer ?? Comparer<int>.Default;

        public IEnumerator<int> GetEnumerator() => values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }

    [GenerateShape, ExcludeFromCodeCoverage]
    internal partial class EnumerableCEnumerable(IComparer<int>? comparer, IEnumerable<int> values) : IEnumerable<int>
    {
        public EnumerableCEnumerable(IEnumerable<int> values) : this(null, values) { }

        public IComparer<int> Comparer => comparer ?? Comparer<int>.Default;

        public IEnumerator<int> GetEnumerator() => values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }

    [GenerateShape, ExcludeFromCodeCoverage]
    internal partial class EnumerableECList(IEqualityComparer<int>? comparer, IList<int> values) : IEnumerable<int>
    {
        public EnumerableECList(IList<int> values) : this(null, values) { }

        public IEqualityComparer<int> Comparer => comparer ?? EqualityComparer<int>.Default;

        public IEnumerator<int> GetEnumerator() => values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [GenerateShape, ExcludeFromCodeCoverage]
    internal partial class EnumerableCList(IComparer<int>? comparer, IList<int> values) : IEnumerable<int>
    {
        public EnumerableCList(IList<int> values) : this(null, values) { }

        public IComparer<int> Comparer => comparer ?? Comparer<int>.Default;

        public IEnumerator<int> GetEnumerator() => values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [GenerateShape, ExcludeFromCodeCoverage]
    internal partial class EnumerableListEC(IList<int> values, IEqualityComparer<int>? comparer) : IEnumerable<int>
    {
        public EnumerableListEC(IList<int> values) : this(values, null) { }

        public IEqualityComparer<int> Comparer => comparer ?? EqualityComparer<int>.Default;

        public IEnumerator<int> GetEnumerator() => values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [GenerateShape, ExcludeFromCodeCoverage]
    internal partial class EnumerableListC(IList<int> values, IComparer<int>? comparer) : IEnumerable<int>
    {
        public EnumerableListC(IList<int> values) : this(values, null) { }

        public IComparer<int> Comparer => comparer ?? Comparer<int>.Default;

        public IEnumerator<int> GetEnumerator() => values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [GenerateShape, ExcludeFromCodeCoverage]
    internal partial class EnumerableECSpan : IEnumerable<int>
    {
        private readonly IEqualityComparer<int> comparer;
        private readonly List<int> values;

        public EnumerableECSpan(IEqualityComparer<int>? comparer, ReadOnlySpan<int> values)
        {
            this.comparer = comparer ?? EqualityComparer<int>.Default;
            this.values = [.. values];
        }

        public EnumerableECSpan(ReadOnlySpan<int> values) : this(null, values) { }

        public IEqualityComparer<int> Comparer => comparer;

        public IEnumerator<int> GetEnumerator() => values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [GenerateShape, ExcludeFromCodeCoverage]
    internal partial class EnumerableCSpan : IEnumerable<int>
    {
        private readonly IComparer<int> comparer;
        private readonly List<int> values;

        public EnumerableCSpan(IComparer<int>? comparer, ReadOnlySpan<int> values)
        {
            this.comparer = comparer ?? Comparer<int>.Default;
            this.values = [.. values];
        }

        public EnumerableCSpan(ReadOnlySpan<int> values) : this(null, values) { }

        public IComparer<int> Comparer => comparer;

        public IEnumerator<int> GetEnumerator() => values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [GenerateShape, ExcludeFromCodeCoverage]
    internal partial class DictionarySpan : IReadOnlyDictionary<int, bool>
    {
        private Dictionary<int, bool> inner;

        public DictionarySpan(ReadOnlySpan<KeyValuePair<int, bool>> span)
            : this(span, null)
        {
        }

        public DictionarySpan(ReadOnlySpan<KeyValuePair<int, bool>> span, IEqualityComparer<int>? ec)
        {
            this.inner = new(ec);
            foreach (var item in span)
            {
                this.inner.Add(item.Key, item.Value);
            }
        }

        public IEqualityComparer<int> Comparer => inner.Comparer;

        public bool this[int key] => throw new NotImplementedException();

        public IEnumerable<int> Keys => throw new NotImplementedException();

        public IEnumerable<bool> Values => throw new NotImplementedException();

        public int Count => this.inner.Count;

        public bool ContainsKey(int key) => throw new NotImplementedException();

        public IEnumerator<KeyValuePair<int, bool>> GetEnumerator() => this.inner.GetEnumerator();

        public bool TryGetValue(int key, [MaybeNullWhen(false)] out bool value) => throw new NotImplementedException();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [GenerateShape, ExcludeFromCodeCoverage]
    internal partial class DictionaryValuesEC : Dictionary<int, bool>
    {
        public DictionaryValuesEC(IEnumerable<KeyValuePair<int, bool>> enumerable)
            : this(enumerable, null)
        {
        }

        public DictionaryValuesEC(IEnumerable<KeyValuePair<int, bool>> enumerable, IEqualityComparer<int>? ec)
            : base(enumerable.ToDictionary(kv => kv.Key, kv => kv.Value), ec)
        {
        }
    }

    [GenerateShape, ExcludeFromCodeCoverage]
    internal partial class SortedDictionaryValuesC : SortedDictionary<int, bool>
    {
        public SortedDictionaryValuesC(IEnumerable<KeyValuePair<int, bool>> enumerable)
            : this(enumerable, null)
        {
        }

        public SortedDictionaryValuesC(IEnumerable<KeyValuePair<int, bool>> enumerable, IComparer<int>? comparer)
            : base(enumerable.ToDictionary(kv => kv.Key, kv => kv.Value), comparer)
        {
        }
    }

    [GenerateShape, ExcludeFromCodeCoverage]
    internal partial class SortedDictionarySpan : IReadOnlyDictionary<int, bool>
    {
        private SortedDictionary<int, bool> inner;

        public SortedDictionarySpan(ReadOnlySpan<KeyValuePair<int, bool>> span)
            : this(span, null)
        {
        }

        public SortedDictionarySpan(ReadOnlySpan<KeyValuePair<int, bool>> span, IComparer<int>? comparer)
        {
            this.inner = new(comparer);
            foreach (var item in span)
            {
                this.inner.Add(item.Key, item.Value);
            }
        }

        public IComparer<int> Comparer => inner.Comparer;

        public bool this[int key] => throw new NotImplementedException();

        public IEnumerable<int> Keys => throw new NotImplementedException();

        public IEnumerable<bool> Values => throw new NotImplementedException();

        public int Count => this.inner.Count;

        public bool ContainsKey(int key) => throw new NotImplementedException();

        public IEnumerator<KeyValuePair<int, bool>> GetEnumerator() => this.inner.GetEnumerator();

        public bool TryGetValue(int key, [MaybeNullWhen(false)] out bool value) => throw new NotImplementedException();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public sealed class Reflection() : CollectionsWithComparersTests(ReflectionProviderUnderTest.NoEmit);
    public sealed class ReflectionEmit() : CollectionsWithComparersTests(ReflectionProviderUnderTest.Emit);
    public sealed class SourceGen() : CollectionsWithComparersTests(new SourceGenProviderUnderTest(Witness.ShapeProvider));
}
