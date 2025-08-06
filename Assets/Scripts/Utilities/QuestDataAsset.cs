using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "QuestDataAsset", menuName = "Quests/Quest Data")]
public class QuestDataAsset : ScriptableObject
{
    [SerializeField] private List<string> significantEvents = new();
    [SerializeField] private List<string> questNames = new();
    [SerializeField] private List<string> taskNames = new();

    public List<string> GetSignificantEvents() => significantEvents;
    public List<string> GetQuestNames() => questNames;
    public List<string> GetTaskNames() => taskNames;
}
