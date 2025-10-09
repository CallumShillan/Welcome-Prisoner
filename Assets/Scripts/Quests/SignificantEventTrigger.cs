using UnityEngine;

/// <summary>
/// Triggers quest, task, and/or significant event activation when the player enters the trigger.
/// </summary>
public class SignificantEventTrigger : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The Significant Event to raise, if needed")]
    [SignificantEventDropdown("GetSignificantEvents")]
    private string significantEvent = string.Empty;

    /// <summary>
    /// When the player trips the trigger, set the Quest, Task, and/or Significant Event as needed.
    /// </summary>
    /// <param name="other">The collider that entered the trigger.</param>
    private void OnTriggerEnter(Collider other)
    {
        // Only trigger for the player
        if (!other.CompareTag("Player"))
        {
            return;
        }

        // Always handle the significant event first
        if (!string.IsNullOrWhiteSpace(significantEvent))
        {
            QuestManager.HandleSignificantEvent(significantEvent);
        }
    }
}
