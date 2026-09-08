namespace Orbyss.Localization;

/// <summary>Selects the catalog whose latest non-retired release serves runtime requests.</summary>
public sealed record LocalizationRuntimeOptions(string CatalogId = "application")
{
    /// <summary>Names the shell configuration section.</summary>
    public const string SectionName = "Orbyss:Localization:Runtime";
}
