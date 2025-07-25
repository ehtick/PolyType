﻿using System.Formats.Cbor;
using PolyType.Abstractions;
using PolyType.Examples.Utilities;

namespace PolyType.Examples.CborSerializer.Converters;

internal class CborDictionaryConverter<TDictionary, TKey, TValue>(
    CborConverter<TKey> keyConverter,
    CborConverter<TValue> valueConverter,
    Func<TDictionary, IReadOnlyDictionary<TKey, TValue>> getDictionary) : CborConverter<TDictionary>
{
    private protected readonly CborConverter<TKey> _keyConverter = keyConverter;
    private protected readonly CborConverter<TValue> _valueConverter = valueConverter;

    public override TDictionary? Read(CborReader reader)
    {
        if (default(TDictionary) is null && reader.PeekState() == CborReaderState.Null)
        {
            reader.ReadNull();
            return default;
        }

        throw new NotSupportedException($"Type {typeof(TDictionary)} does not support deserialization.");
    }

    public sealed override void Write(CborWriter writer, TDictionary? value)
    {
        if (value is null)
        {
            writer.WriteNull();
            return;
        }

        IReadOnlyDictionary<TKey, TValue> dictionary = getDictionary(value);
        CborConverter<TKey> keyConverter = _keyConverter;
        CborConverter<TValue> valueConverter = _valueConverter;

        writer.WriteStartMap(dictionary.Count);
        foreach (KeyValuePair<TKey, TValue> kvp in dictionary)
        {
            keyConverter.Write(writer, kvp.Key);
            valueConverter.Write(writer, kvp.Value);
        }

        writer.WriteEndMap();
    }
}

internal sealed class CborMutableDictionaryConverter<TDictionary, TKey, TValue>(
    CborConverter<TKey> keyConverter,
    CborConverter<TValue> valueConverter,
    Func<TDictionary, IReadOnlyDictionary<TKey, TValue>> getDictionary,
    MutableCollectionConstructor<TKey, TDictionary> createObject,
    DictionaryInserter<TDictionary, TKey, TValue> inserter) : CborDictionaryConverter<TDictionary, TKey, TValue>(keyConverter, valueConverter, getDictionary)
{
    private readonly DictionaryInserter<TDictionary, TKey, TValue> _inserter = inserter;

    public override TDictionary? Read(CborReader reader)
    {
        if (default(TDictionary) is null && reader.PeekState() is CborReaderState.Null)
        {
            reader.ReadNull();
            return default;
        }

        int? definiteLength = reader.ReadStartMap();
        TDictionary result = createObject(new() { Capacity = definiteLength });

        CborConverter<TKey> keyConverter = _keyConverter;
        CborConverter<TValue> valueConverter = _valueConverter;
        DictionaryInserter<TDictionary, TKey, TValue> inserter = _inserter;

        while (reader.PeekState() != CborReaderState.EndMap)
        {
            TKey key = keyConverter.Read(reader)!;
            TValue value = valueConverter.Read(reader)!;
            if (!inserter(ref result, key, value))
            {
                Throw(key);
                static void Throw(TKey key) => throw new ArgumentException($"Found duplicate key '{key}.'");
            }
        }

        reader.ReadEndMap();
        return result;
    }
}

internal sealed class CborParameterizedDictionaryConverter<TDictionary, TKey, TValue>(
    CborConverter<TKey> keyConverter,
    CborConverter<TValue> valueConverter,
    Func<TDictionary, IReadOnlyDictionary<TKey, TValue>> getDictionary,
    ParameterizedCollectionConstructor<TKey, KeyValuePair<TKey, TValue>, TDictionary> constructor)
    : CborDictionaryConverter<TDictionary, TKey, TValue>(keyConverter, valueConverter, getDictionary)
{
    public sealed override TDictionary? Read(CborReader reader)
    {
        if (default(TDictionary) is null && reader.PeekState() is CborReaderState.Null)
        {
            reader.ReadNull();
            return default;
        }

        int? definiteLength = reader.ReadStartMap();
        using PooledList<KeyValuePair<TKey, TValue>> buffer = new(definiteLength ?? 4);
        CborConverter<TKey> keyConverter = _keyConverter;
        CborConverter<TValue> valueConverter = _valueConverter;

        while (reader.PeekState() != CborReaderState.EndMap)
        {
            TKey key = keyConverter.Read(reader)!;
            TValue value = valueConverter.Read(reader)!;
            buffer.Add(new(key, value));
        }

        reader.ReadEndMap();
        return constructor(buffer.AsSpan());
    }
}
