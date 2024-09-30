using System;

/// <summary>
/// An action that can be stored in a cache
/// </summary>
public class CachedAction
{
    // The number of ticks that represent the date and time for when the action was inserted into the cache
    long insertionTick;

    // The action itself
    IActionInterface action;

    /// <summary>
    /// Constructor for the cached action
    /// </summary>
    public CachedAction()
    {
    }

    /// <summary>
    /// Constructor for the cached action with initial values
    /// </summary>
    /// <param name="insertionTick">The number of ticks that represent the date and time for when the action was inserted into the cache</param>
    /// <param name="action">The action itself</param>
    public CachedAction(long insertionTick, IActionInterface action)
    {
        this.insertionTick = insertionTick;
        this.action = action;
    }

    /// <summary>
    /// The action that has been cached
    /// </summary>
    public IActionInterface Action { get => action; set => action = value; }

    /// <summary>
    /// The number of ticks that represent the date and time for when the action was inserted into the cache
    /// </summary>
    public long InsertionTick { get => insertionTick; set => insertionTick = value; }
}
