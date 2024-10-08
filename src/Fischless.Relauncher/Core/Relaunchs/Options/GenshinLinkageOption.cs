﻿namespace Fischless.Relauncher.Core.Relaunchs;

public sealed class GenshinLinkageOption
{
    public string? ReShadePath { get; set; } = null;

    public bool IsUseReShade { get; set; } = false;

    public bool IsUseReShadeSilent { get; set; } = false;

    public bool IsUseBetterGI { get; set; } = false;
}
