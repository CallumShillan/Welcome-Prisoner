using UnityEngine.UI;
using TMPro;

public enum InteractionStatus { Completed, Continuing, ShowPdaHomeScreen }

/// <summary>
/// Defines the actions that can be used on some Game Objects
/// </summary>
public interface IActionInterface
{
    /// <summary>
    /// Advertises an action
    /// </summary>
    /// <returns>TRUE if the object needs to advertise further action, FALSE if not</returns>
    bool AdvertiseInteraction();

    /// <summary>
    /// Perform an Action
    /// </summary>
    /// <returns>TRUE if the object performs further interactions, FALSE otherwise (e.g., when turning a light on/off or opening/closing a door)</returns>
    bool PerformInteraction();

    /// <summary>
    /// Continue a user interface dialogue with the user
    /// </summary>
    /// <returns>InteractionStatus</returns>
    InteractionStatus ContinueInteraction()
    {
        return (InteractionStatus.Completed);
    }

    void CompleteInteraction()
    {
    }

}