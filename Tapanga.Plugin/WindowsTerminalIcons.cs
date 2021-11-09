using Tapanga.Plugin;

namespace Tapanga;

/// <summary>
/// Icons available in the Windows Terminal package.
/// </summary>
public static class WindowsTerminalIcons
{
    /// <summary>
    /// Command Prompt Icon
    /// </summary>
    public static Icon CommandPrompt => new PathIcon("CommandPrompt", "ms-appx:///ProfileIcons/{0caa0dad-35be-5f56-a8ff-afceeeaa6101}.png");

    /// <summary>
    /// Powershell Icon
    /// </summary>
    public static Icon Pwsh => new PathIcon("Pwsh", "ms-appx:///ProfileIcons/pwsh.png");

    /// <summary>
    /// Powershell Preview Icon
    /// </summary>
    public static Icon PwshPreview => new PathIcon("Pwsh Preview", "ms-appx:///ProfileIcons/pwsh-preview.png");

    /// <summary>
    /// Windows PowerShell Icon
    /// </summary>
    public static Icon WindowsPowerShell => new PathIcon("Windows Powershell", "ms-appx:///ProfileIcons/{61c54bbd-c2c6-5271-96e7-009a87ff44bf}.png");

    /// <summary>
    /// Telnet Icon
    /// </summary>
    public static Icon Telnet => new PathIcon("Telnet", "ms-appx:///ProfileIcons/{550ce7b8-d500-50ad-8a1a-c400c3262db3}.png");

    /// <summary>
    /// Linux Tux Icon
    /// </summary>
    public static Icon LinuxTux => new PathIcon("Linux Tux", "ms-appx:///ProfileIcons/{9acb9455-ca41-5af7-950f-6bca1bc9722f}.png");

    /// <summary>
    /// Azure Cloud Icon
    /// </summary>
    public static Icon AzureCloud => new PathIcon("Azure Cloud", "ms-appx:///ProfileIcons/{b453ae62-4e3d-5e58-b989-0a998ec441b8}.png");
}
