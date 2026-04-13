using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Resources;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
        context.RegisterSyntaxNodeAction(AnalyzeSymbol,
            SyntaxKind.ClassDeclaration,
            SyntaxKind.StructDeclaration,
            SyntaxKind.FieldDeclaration,
            SyntaxKind.PropertyDeclaration,
            SyntaxKind.MethodDeclaration,
            SyntaxKind.LocalFunctionStatement,
            SyntaxKind.Parameter);
    }

    private void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
    {
        var attrs = GetEnvironmentValidate(context, context.CancellationToken);
        foreach (var fs in attrs)
        {
            HashSet<string> formats = [];
            HashSet<int> priorities = [];
            foreach (var (parent, attr) in fs.Formats)
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
    }

    private static IEnumerable<EnvironmentValidateSyntax> GetEnvironmentValidate(
        SyntaxNodeAnalysisContext ctx,
        CancellationToken ct)
    {
        var semanticModel = ctx.SemanticModel;
        if (ctx.Node is FieldDeclarationSyntax fields)
        {
            foreach (var field in fields.Declaration.Variables)
            {
                var symbol = semanticModel.GetDeclaredSymbol(field, ct);
                if (symbol is null) continue;
                List<(bool parent, AttributeData attr)> formats = [];
                var currentSymbol = symbol.ContainingSymbol;
                while (currentSymbol is not null)
                {
                    formats.AddRange(
                        GetFromSymbol(currentSymbol, semanticModel, ct).Select(static attr => (true, attr)));
                    currentSymbol = currentSymbol.ContainingSymbol;
                }

                formats.AddRange(GetFromSymbol(symbol, semanticModel, ct).Select(static attr => (false, attr)));
                yield return new EnvironmentValidateSyntax(formats);
            }
        }
        else
        {
            var symbol = semanticModel.GetDeclaredSymbol(ctx.Node, ct);
            if (symbol is null) yield break;
            List<(bool parent, AttributeData attr)> formats = [];
            var currentSymbol = symbol.ContainingSymbol;
            while (currentSymbol is not null)
            {
                formats.AddRange(GetFromSymbol(currentSymbol, semanticModel, ct).Select(static attr => (true, attr)));
                currentSymbol = currentSymbol.ContainingSymbol;
            }

            formats.AddRange(GetFromSymbol(symbol, semanticModel, ct).Select(static attr => (false, attr)));
            yield return new EnvironmentValidateSyntax(formats);
        }

        yield break;

        static List<AttributeData> GetFromSymbol(ISymbol symbol, SemanticModel model,
            CancellationToken ct)
        {
            const string envNameFormatName = "EnvironmentVariableNameFormatAttribute";
            return symbol.GetAttributes()
                .Where(static attr => attr.AttributeClass?.ToDisplayString().Contains(envNameFormatName) ?? false)
                .Where(a => a is not null)
                .Select(a => a!)
                .ToList();
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

    private readonly record struct EnvironmentValidateSyntax(
        List<(bool parent, AttributeData attr)> Formats)
    {
        public List<(bool parent, AttributeData attr)> Formats { get; } = Formats;
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