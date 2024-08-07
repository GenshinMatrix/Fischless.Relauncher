using Relauncher.Core.Configs;
using System.Reflection;

namespace Relauncher.Models;

[Obfuscation]
public static class Configurations
{
    public static ConfigurationDefinition<string> Language { get; } = new(nameof(Language), string.Empty);
}
