namespace NilArea.Contracts.Annotation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field |
                AttributeTargets.Class | AttributeTargets.Struct |
                AttributeTargets.Method | AttributeTargets.Parameter,
    AllowMultiple = true)]
public sealed class EnvironmentVariableNameFormatAttribute : Attribute
{
    public string? Prefix { get; set; }
    public string? Suffix { get; set; }
    public int Priority { get; set; }
}