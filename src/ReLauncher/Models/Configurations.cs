using Relauncher.Core.Configs;
using System.Reflection;

namespace Relauncher.Models;

[Obfuscation]
public static class Configurations
{
    public static ConfigurationDefinition<GenshinConfigurations> Genshin { get; } = new(nameof(GenshinConfigurations), new());
}
