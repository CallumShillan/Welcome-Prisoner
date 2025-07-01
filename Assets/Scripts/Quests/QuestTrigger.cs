using UnityEngine;

/// <summary>
/// Triggers quest, task, and/or significant event activation when the player enters the trigger.
/// </summary>
public class QuestTrigger : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The Quest to initiate, if needed")]
    private string questToInitiate = string.Empty;

    [SerializeField]
    [Tooltip("The Task to initiate, if needed")]
    private string taskToInitiate = string.Empty;

    [SerializeField]
    [Tooltip("The Significant Event to raise, if needed")]
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

        // Determine if we have a quest, task, or significant event to initiate
        bool hasQuest = !string.IsNullOrWhiteSpace(questToInitiate);
        bool hasTask = !string.IsNullOrWhiteSpace(taskToInitiate);
        bool hasEvent = !string.IsNullOrWhiteSpace(significantEvent);

        // Return if we don't have any of them
        if (!hasQuest && !hasTask && !hasEvent)
        {
            return;
        }

        if (hasQuest)
        {
            QuestManager.CurrentQuestName = questToInitiate;

            if (hasTask)
            {
                QuestManager.CurrentTaskName = taskToInitiate;
            }
        }

        if (hasEvent)
        {
            QuestManager.HandleSignificantEvent(significantEvent);
        }
    }
}
