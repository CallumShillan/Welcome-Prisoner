using UnityEngine;

/// <summary>
/// Triggers quest, task, and/or significant event activation when the player enters the trigger.
/// </summary>
public class QuestTrigger : MonoBehaviour
{
    private enum RepeatableQuestType
    {
        Repeatable,
        NonRepeatable
    }

    [SerializeField]
    [Tooltip("The Significant Event to raise, if needed")]
    [StringDropdown("GetSignificantEvents")]
    private string significantEvent = string.Empty;

    [SerializeField]
    [Tooltip("Is the quest repeatable or not?")]
    private RepeatableQuestType questType = RepeatableQuestType.NonRepeatable;

    [SerializeField]
    [Tooltip("The Quest to initiate, if needed")]
    [QuestDropdown("GetQuestNames")]
    private string questToInitiate = string.Empty;

    [SerializeField]
    [Tooltip("The Task to initiate, if needed")]
    [TaskDropdown("GetTaskNames")]
    private string taskToInitiate = string.Empty;

    private bool questAlreadyTriggered = false;

    private void Awake()
    {
        // Gatekeeper check: Ensure at least one triggerable item is set
        bool noQuest = string.IsNullOrWhiteSpace(questToInitiate);
        bool noTask = string.IsNullOrWhiteSpace(taskToInitiate);
        bool hasEvent = !string.IsNullOrWhiteSpace(significantEvent);

        if (noQuest || noTask)
        {
            Debug.LogWarning($"QuestTrigger on '{gameObject.name}' both Quest and Task need to be set.");
            return;
        }

    }

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

        // If the quest is non-repeatable and already triggered, do nothing
        if (questType == RepeatableQuestType.NonRepeatable && questAlreadyTriggered)
        {
            return;
        }

        // To be here means quest is either repeatable or not yet triggered, either way, we need to set it up
        questAlreadyTriggered = true;

        // Determine if we have a quest and task to initiate
        bool hasQuest = !string.IsNullOrWhiteSpace(questToInitiate);
        bool hasTask = !string.IsNullOrWhiteSpace(taskToInitiate);

        if (hasQuest && hasTask)
        {
            QuestManager.CurrentQuestName = questToInitiate;
            QuestManager.CurrentTaskName = taskToInitiate;
        }
        else
        {
            GameLog.ErrorMessage($"QuestTrigger on '{gameObject.name}' has no Quest and Task set to trigger. Quest: {questToInitiate}. Task: {taskToInitiate}");
        }
    }
}
