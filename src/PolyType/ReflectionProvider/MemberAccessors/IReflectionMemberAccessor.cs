﻿using PolyType.Abstractions;
using System.Reflection;

namespace PolyType.ReflectionProvider.MemberAccessors;

internal interface IReflectionMemberAccessor
{
    Getter<TDeclaringType, TPropertyType> CreateGetter<TDeclaringType, TPropertyType>(MemberInfo memberInfo, MemberInfo[]? parentMembers);
    Setter<TDeclaringType, TPropertyType> CreateSetter<TDeclaringType, TPropertyType>(MemberInfo memberInfo, MemberInfo[]? parentMembers);

    EnumerableAppender<TEnumerable, TElement> CreateEnumerableAppender<TEnumerable, TElement>(MethodInfo methodInfo);
    DictionaryInserter<TDictionary, TKey, TValue> CreateDictionaryInserter<TDictionary, TKey, TValue>(MutableCollectionConstructorInfo ctorInfo, DictionaryInsertionMode insertionMode);

    Func<TDeclaringType> CreateDefaultConstructor<TDeclaringType>(IConstructorShapeInfo ctorInfo);

    Type CreateConstructorArgumentStateType(IConstructorShapeInfo ctorInfo);
    Func<TArgumentState> CreateConstructorArgumentStateCtor<TArgumentState>(IConstructorShapeInfo ctorInfo);
    Setter<TArgumentState, TParameter> CreateConstructorArgumentStateSetter<TArgumentState, TParameter>(IConstructorShapeInfo ctorInfo, int parameterIndex);
    Constructor<TArgumentState, TDeclaringType> CreateParameterizedConstructor<TArgumentState, TDeclaringType>(IConstructorShapeInfo ctorInfo);

    TDelegate CreateFuncDelegate<TDelegate>(ConstructorInfo ctorInfo) where TDelegate : Delegate;
    Func<T, TResult> CreateFuncDelegate<T, TResult>(ConstructorInfo ctorInfo);
    Func<T1, T2, TResult> CreateFuncDelegate<T1, T2, TResult>(ConstructorInfo ctorInfo);

    bool IsCollectionConstructorSupported(MethodBase method, CollectionConstructorParameter[] signature);
    MutableCollectionConstructor<TKey, TCollection> CreateMutableCollectionConstructor<TKey, TElement, TCollection>(MutableCollectionConstructorInfo constructorInfo);
    ParameterizedCollectionConstructor<TKey, TElement, TCollection> CreateParameterizedCollectionConstructor<TKey, TElement, TCollection>(ParameterizedCollectionConstructorInfo constructorInfo);

    Getter<TUnion, int> CreateGetUnionCaseIndex<TUnion>(DerivedTypeInfo[] derivedTypeInfos);
}
