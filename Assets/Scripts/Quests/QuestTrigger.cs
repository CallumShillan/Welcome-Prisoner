using UnityEngine;

public class QuestTrigger : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The Significant Event to raise, if needed")]
    private string significantEvent = string.Empty;

    [SerializeField]
    [Tooltip("The Quest to initiate, if needed")]
    private string questToInititate = string.Empty;

    [SerializeField]
    [Tooltip("The Task to initiate, if needed")]
    private string taskToInititate = string.Empty;

    /// <summary>
    /// When the player trips the trigger set the Quest, Task and/or Significant Event, as needed
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        // Handle the Significant Event, if needed
        if (!string.IsNullOrWhiteSpace(significantEvent))
        {
            QuestManager.HandleSignificantEvent(significantEvent);
        }

        // Set current quest and task, if needed
        if (!string.IsNullOrWhiteSpace(questToInititate))
        {
            // Assign the quest to be made active
            QuestManager.CurrentQuestName = questToInititate;

            if (!string.IsNullOrWhiteSpace(taskToInititate))
            {
                // Assign the task to be made active
                QuestManager.CurrentTaskName = taskToInititate;
            }
        }

    }
}
