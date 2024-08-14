using Fischless.Relauncher.Core.Configs;
using System.Reflection;

namespace Fischless.Relauncher.Models;

[Obfuscation]
public static class Configurations
{
    public static ConfigurationDefinition<GenshinConfigurations> Genshin { get; } = new(nameof(GenshinConfigurations), new());
}
