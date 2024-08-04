﻿using System.Windows;

namespace Relauncher.Views.Controls;

/// <summary>
/// Inherited from the <see cref="System.Windows.Controls.Primitives.ButtonBase"/> control which displays an additional control on the right side of the card.
/// </summary>
public class CardControl : System.Windows.Controls.Primitives.ButtonBase
{
    /// <summary>
    /// Property for <see cref="Title"/>.
    /// </summary>
    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title),
        typeof(string), typeof(CardControl), new PropertyMetadata(""));

    /// <summary>
    /// Property for <see cref="Subtitle"/>.
    /// </summary>
    public static readonly DependencyProperty SubtitleProperty = DependencyProperty.Register(nameof(Subtitle),
        typeof(string), typeof(CardControl), new PropertyMetadata(""));

    /// <summary>
    /// Property for <see cref="Icon"/>.
    /// </summary>
    public static readonly DependencyProperty IconProperty = DependencyProperty.Register(nameof(Icon),
        typeof(string), typeof(CardControl),
        new PropertyMetadata(""));

    /// <summary>
    /// Property for <see cref="IconFilled"/>.
    /// </summary>
    public static readonly DependencyProperty IconFilledProperty = DependencyProperty.Register(nameof(IconFilled),
        typeof(bool), typeof(CardControl), new PropertyMetadata(false));

    /// <summary>
    /// Gets or sets text displayed on the left side of the card.
    /// </summary>
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>
    /// Gets or sets text displayed under main <see cref="Title"/>.
    /// </summary>
    public string Subtitle
    {
        get => (string)GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    /// <inheritdoc />
    public string Icon
    {
        get => (string)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <inheritdoc />
    public bool IconFilled
    {
        get => (bool)GetValue(IconFilledProperty);
        set => SetValue(IconFilledProperty, value);
    }
}
