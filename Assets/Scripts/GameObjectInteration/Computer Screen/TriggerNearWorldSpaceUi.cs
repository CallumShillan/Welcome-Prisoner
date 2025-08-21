using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TriggerNearWorldSpaceUi : MonoBehaviour
{
    private RawImage cursorIconRawImage;
    void Start()
    {
        if (Globals.Instance.CursorIcon.TryGetComponent<RawImage>(out cursorIconRawImage))
        {
            cursorIconRawImage.color = Color.white; // Set the initial color to white
        }
        else
        {
            Debug.LogError("ActionIcon does not have a RawImage component.");
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Globals.Instance.PlayerInteraction.NearWorldSpaceUi = true;
            cursorIconRawImage.color = Color.red; // Change the action icon color to red when near world space UI

        }

    }

    private void OnTriggerExit(Collider other)  
    {
        if (other.CompareTag("Player"))
        {
            //Globals.Instance.PlayerInteraction.NearWorldSpaceUi = false;
            cursorIconRawImage.color = Color.white;
        }
    }
}
