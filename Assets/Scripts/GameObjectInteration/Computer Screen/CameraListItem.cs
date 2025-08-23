using UnityEngine.UIElements;

[UxmlElement]
public partial class CameraListItem : VisualElement
{
    public CameraListItem()
    {
        var toggle = new Toggle { name = "itemToggle" };
        var label = new Label { name = "itemLabel" };
        Add(toggle);
        Add(label);
    }
}
