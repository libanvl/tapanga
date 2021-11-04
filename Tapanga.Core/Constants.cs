namespace Tapanga.Core;

public class Constants
{
    public const string ExtensionName = "Tapanga";

    public static readonly string FragmentsRootPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "Microsoft",
        "Windows Terminal",
        "Fragments");

}
