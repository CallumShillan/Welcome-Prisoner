using UnityEngine;

public class StringDropdownAttribute : PropertyAttribute
{
    public string methodName;

    public StringDropdownAttribute(string methodName)
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