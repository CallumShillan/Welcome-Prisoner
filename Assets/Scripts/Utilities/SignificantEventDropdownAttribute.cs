using UnityEngine;

public class SignificantEventDropdownAttribute : PropertyAttribute
{
    public string methodName;

    public SignificantEventDropdownAttribute(string methodName)
    {
        this.methodName = methodName;
    }
}

public class QuestDropdownAttribute : PropertyAttribute
{
    public string methodName;

    public QuestDropdownAttribute(string methodName)
    {
        this.methodName = methodName;
    }
}

public class TaskDropdownAttribute : PropertyAttribute
{
    public string methodName;
    public TaskDropdownAttribute(string methodName)
    {
        this.methodName = methodName;
    }
}