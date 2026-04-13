using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Resources;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Nilarea.Generator;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class EnvironmentFormatAnalyzer : DiagnosticAnalyzer
{
    private const string Category = "Nilarea.Generator";
    private static readonly ResourceManager ResourceManager = new(typeof(Resources));

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        [UniquePriorityRule, DuplicateFormatRule, InvalidFormatRule];


    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze |
                                               GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.RegisterSymbolAction(AnalyzeSymbol,
            SymbolKind.NamedType,
            SymbolKind.Field,
            SymbolKind.Property,
            SymbolKind.Method,
            SymbolKind.Parameter);
    }

    private static void AnalyzeSymbol(SymbolAnalysisContext context)
    {
        var attrs = GetEnvNameFormatAttributes(context.Symbol, context.CancellationToken);
        HashSet<string> formats = [];
        HashSet<int> priorities = [];
        foreach (var (parent, attr) in attrs)
        {
            var location = attr.ApplicationSyntaxReference?.GetSyntax().GetLocation();
            if (!GetAttributeValue(attr, out var priority, out var prefix, out var suffix) && !parent)
                context.ReportDiagnostic(Diagnostic.Create(InvalidFormatRule, location));
            if (!priorities.Add(priority) && !parent)
                context.ReportDiagnostic(Diagnostic.Create(UniquePriorityRule, location, priority));
            if (formats.Add($"{prefix}${suffix}") || parent) continue;
            if (string.IsNullOrWhiteSpace(prefix))
                context.ReportDiagnostic(Diagnostic.Create(DuplicateFormatRule, location, $"Suffix: {suffix}"));
            else if (string.IsNullOrWhiteSpace(suffix))
                context.ReportDiagnostic(Diagnostic.Create(DuplicateFormatRule, location, $"Prefix: {prefix}"));
            else
                context.ReportDiagnostic(Diagnostic.Create(DuplicateFormatRule, location,
                    $"Prefix: {prefix}, Suffix: {suffix}"));
        }
    }

    private static bool GetAttributeValue(AttributeData attr, out int priority, out string? prefix, out string? suffix)
    {
        priority = 0;
        prefix = null;
        suffix = null;
        if (attr.NamedArguments.IsDefaultOrEmpty)
            return false;
        foreach (var named in attr.NamedArguments)
        {
            if (named is { Key: "Priority", Value.Value: int priorityValue })
                priority = priorityValue;
            if (named is { Key: "Prefix", Value.Value: string prefixValue })
                prefix = prefixValue;
            if (named is { Key: "Suffix", Value.Value: string suffixValue })
                suffix = suffixValue;
        }

        return !string.IsNullOrWhiteSpace(prefix) || !string.IsNullOrWhiteSpace(suffix);
    }


    private static List<(bool Parent, AttributeData AttributeData)> GetEnvNameFormatAttributes(ISymbol symbol,
        CancellationToken ct)
    {
        List<(bool Parent, AttributeData AttributeData)> attrs = [];
        var current = symbol.ContainingSymbol;
        while (current is not null)
        {
            attrs.AddRange(GetFromSymbol(current).Select(attr => (true, attr)));
            current = current.ContainingSymbol;
        }

        attrs.AddRange(GetFromSymbol(symbol).Select(attr => (false, attr)));
        return attrs;

        static IEnumerable<AttributeData> GetFromSymbol(ISymbol symbol)
        {
            const string envNameFormatName = "EnvironmentVariableNameFormatAttribute";
            return symbol.GetAttributes()
                .Where(static attr => attr.AttributeClass?.ToDisplayString().Contains(envNameFormatName) ?? false)
                .Where(a => a is not null)
                .Select(a => a!);
        }
    }


    #region NG001

    private const string UniquePriorityId = "NG001";

    private static readonly LocalizableString TitleUniquePriority =
        new LocalizableResourceString(nameof(TitleUniquePriority), ResourceManager, typeof(Resources));

    private static readonly LocalizableString MessageFormatUniquePriority =
        new LocalizableResourceString(nameof(MessageFormatUniquePriority), ResourceManager, typeof(Resources));

    private static readonly LocalizableString DescriptionUniquePriority =
        new LocalizableResourceString(nameof(DescriptionUniquePriority), ResourceManager, typeof(Resources));

    private static readonly DiagnosticDescriptor UniquePriorityRule = new(
        UniquePriorityId, TitleUniquePriority, MessageFormatUniquePriority, Category,
        DiagnosticSeverity.Error,
        true,
        DescriptionUniquePriority);

    #endregion

    #region NG002

    private const string DuplicateFormatId = "NG002";

    private static readonly LocalizableString TitleDuplicateFormat =
        new LocalizableResourceString(nameof(TitleDuplicateFormat), ResourceManager, typeof(Resources));

    private static readonly LocalizableString MessageFormatDuplicateFormat =
        new LocalizableResourceString(nameof(MessageFormatDuplicateFormat), ResourceManager, typeof(Resources));

    private static readonly LocalizableString DescriptionDuplicateFormat =
        new LocalizableResourceString(nameof(DescriptionDuplicateFormat), ResourceManager, typeof(Resources));

    private static readonly DiagnosticDescriptor DuplicateFormatRule = new(
        DuplicateFormatId, TitleDuplicateFormat, MessageFormatDuplicateFormat, Category,
        DiagnosticSeverity.Error,
        true,
        DescriptionDuplicateFormat);

    #endregion

    #region NG003

    private const string InvalidFormatId = "NG003";

    private static readonly LocalizableString TitleInvalidFormat =
        new LocalizableResourceString(nameof(TitleInvalidFormat), ResourceManager, typeof(Resources));

    private static readonly LocalizableString MessageFormatInvalidFormat =
        new LocalizableResourceString(nameof(MessageFormatInvalidFormat), ResourceManager, typeof(Resources));

    private static readonly LocalizableString DescriptionInvalidFormat =
        new LocalizableResourceString(nameof(DescriptionInvalidFormat), ResourceManager, typeof(Resources));

    private static readonly DiagnosticDescriptor InvalidFormatRule = new(
        InvalidFormatId, TitleInvalidFormat, MessageFormatInvalidFormat, Category,
        DiagnosticSeverity.Error,
        true,
        DescriptionInvalidFormat);

    #endregion
}