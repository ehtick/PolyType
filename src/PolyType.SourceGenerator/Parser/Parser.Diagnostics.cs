﻿using Microsoft.CodeAnalysis;

namespace PolyType.SourceGenerator;

public sealed partial class Parser
{
    private static DiagnosticDescriptor TypeNotSupported { get; } = new DiagnosticDescriptor(
        id: "PT0001",
        title: "Type shape generation not supported for type.",
        messageFormat: "The type '{0}' is not supported for PolyType generation.",
        category: "PolyType.SourceGenerator",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static DiagnosticDescriptor GeneratedTypeNotPartial { get; } = new DiagnosticDescriptor(
        id: "PT0002",
        title: "Type annotated with GenerateShapeAttribute is not partial.",
        messageFormat: "The type '{0}' has been annotated with GenerateShapeAttribute but it or one of its parent types are not partial.",
        category: "PolyType.SourceGenerator",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    private static DiagnosticDescriptor TypeNameConflict { get; } = new DiagnosticDescriptor(
        id: "PT0003",
        title: "Transitive type graph contains types with conflicting fully qualified names.",
        messageFormat: "The transitive type graph contains multiple types named '{0}'.",
        category: "PolyType.SourceGenerator",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    private static DiagnosticDescriptor GenericTypeDefinitionsNotSupported { get; } = new DiagnosticDescriptor(
        id: "PT0004",
        title: "PolyType generation not supported for generic types.",
        messageFormat: "The type '{0}' is a generic type which is not supported for PolyType generation.",
        category: "PolyType.SourceGenerator",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static DiagnosticDescriptor TypeNotAccessible { get; } = new DiagnosticDescriptor(
        id: "PT0005",
        title: "Type not accessible for generation.",
        messageFormat: "The type '{0}' is not accessible for PolyType generation.",
        category: "PolyType.SourceGenerator",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static DiagnosticDescriptor DuplicateConstructorShape { get; } = new DiagnosticDescriptor(
        id: "PT0006",
        title: "Duplicate ConstructorShapeAttribute annotation.",
        messageFormat: "The type '{0}' contains multiple constructors with a ConstructorShapeAttribute.",
        category: "PolyType.SourceGenerator",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static DiagnosticDescriptor GeneratedTypeIsStatic { get; } = new DiagnosticDescriptor(
        id: "PT0007",
        title: "Types annotated with GenerateShapeAttribute cannot be static.",
        messageFormat: "The type '{0}' that has been annotated with GenerateShapeAttribute cannot be static.",
        category: "PolyType.SourceGenerator",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    private static DiagnosticDescriptor UnsupportedLanguageVersion { get; } = new DiagnosticDescriptor(
        id: "PT0008",
        title: "PolyType requires C# version 9 or newer.",
        messageFormat: "The PolyType source generator requires C# version 9 or newer.",
        category: "PolyType.SourceGenerator",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    private static DiagnosticDescriptor InvalidTypeShapeKind { get; } = new DiagnosticDescriptor(
        id: "PT0009",
        title: "The specified TypeShapeKind is not supported for the type.",
        messageFormat: "The TypeShapeKind '{0}' is not supported for type '{1}'.",
        category: "PolyType.SourceGenerator",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    private static DiagnosticDescriptor InvalidMarshaller { get; } = new DiagnosticDescriptor(
        id: "PT0010",
        title: "Type contains invalid marshaller configuration.",
        messageFormat:
            "The type '{0}' contains invalid marshaller configuration. " +
            "A valid marshaller must be an accessible type with a default constructor and exactly one IMarshaller<,> implementation for the current type.",

        category: "PolyType.SourceGenerator",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    private static DiagnosticDescriptor DerivedTypeNotAssignableToBase { get; } = new DiagnosticDescriptor(
        id: "PT0011",
        title: "Derived type is not a valid subtype.",
        messageFormat: "The declared derived type '{0}' is not a valid subtype of '{1}'.",
        category: "PolyType.SourceGenerator",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    private static DiagnosticDescriptor DerivedTypeDuplicateMetadata { get; } = new DiagnosticDescriptor(
        id: "PT0012",
        title: "Derived type contains conflicting metadata.",
        messageFormat: "Polymorphic type '{0}' uses duplicate assignments for {1} '{2}'.",
        category: "PolyType.SourceGenerator",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    private static DiagnosticDescriptor DerivedTypeUnsupportedGenerics { get; } = new DiagnosticDescriptor(
        id: "PT0013",
        title: "Derived type uses unsupported generics.",
        messageFormat: "The declared derived type '{0}' introduces unsupported type parameters over '{1}'.",
        category: "PolyType.SourceGenerator",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    private static DiagnosticDescriptor AssociatedTypeArityMismatch { get; } = new DiagnosticDescriptor(
        id: "PT0016",
        title: "Associated type arity mismatch.",
        messageFormat: "The associated type '{0}' has arity {1} which must be 0 or match the target type, which has arity {2}.",
        category: "PolyType.SourceGenerator",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    private static DiagnosticDescriptor AssociatedTypeInExternalAssembly { get; } = new DiagnosticDescriptor(
        id: "PT0017",
        title: "Associated type must be declared in same assembly.",
        messageFormat: "The associated type '{0}' is not declared in the same assembly as the attribute annotation specifying it.",
        category: "PolyType.SourceGenerator",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    internal static DiagnosticDescriptor ConflictingMarshallers { get; } = new DiagnosticDescriptor(
        id: "PT0018",
        title: "Multiple marshallers specified.",
        messageFormat: "Multiple TypeShapeExtensionAttribute attributes specified for target type '{0}' with conflicting marshallers specified. At most one marshaller can be specified.",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    internal static DiagnosticDescriptor UnsanctionedShape { get; } = new DiagnosticDescriptor(
        id: "PT0019",
        title: "No external shape implementations.",
        messageFormat: "The type \"{0}\" implements \"{1}\", which is a reserved interface that should only be implemented by PolyType itself.",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);
}
