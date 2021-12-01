namespace Tapanga.Plugin;

/// <summary>
/// The context for instantiating generators.
/// </summary>
/// <param name="Logger">A logger instance.</param>
/// <param name="DryRun"></param>
public record GeneratorContext(ILogger Logger, bool DryRun);
