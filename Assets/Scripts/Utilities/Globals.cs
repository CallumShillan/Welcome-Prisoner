using System;
using System.Collections.Generic;

// Thanks to https://csharpindepth.com/articles/singleton
public sealed class Globals
{
    private static readonly Lazy<Globals> lazy = new Lazy<Globals>(() => new Globals());

    public static Globals Instance { get { return lazy.Value; } }

    public bool FocussedInteraction { get => focussedInteraction; set => focussedInteraction = value; }

    private bool focussedInteraction = false;

    private Globals()
    {
    }
}