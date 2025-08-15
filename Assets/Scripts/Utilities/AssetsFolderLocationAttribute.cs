using UnityEngine;

public class AssetsFolderLocationAttribute : PropertyAttribute
{
    public string label;

    public AssetsFolderLocationAttribute(string label = "Assets Folder")
    {
        this.label = label;
    }
}
