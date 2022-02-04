namespace Tapanga.Plugin;

public delegate void ContextFileWriter(IProfileGenerator generator, string relativePath, StringCollection content);

public delegate string ContextPathCombine(IProfileGenerator generator, string relativePath);

/// <summary>
/// The context for instantiating generators.
/// </summary>
/// <param name="Logger">A logger instance.</param>
/// <param name="ContextFileWriter">A delegate to write files to the generator context.</param>
/// <param name="ContextPathCombine">A delegate to get the root path of the generator context.</param>
/// <param name="DryRun"></param>
public record GeneratorContext(ILogger Logger, ContextFileWriter ContextFileWriter, ContextPathCombine ContextPathCombine, bool DryRun);
