using System.Collections;
using UnityEngine;

public class PrisonerItems : MonoBehaviour, IActionInterface
{
    private const string NamePlaceholder = "{NAME}";
    private const string ActionPlaceholder = "{ACTION}";

    [SerializeField, Tooltip("The significant event to raise")]
    [SignificantEventDropdown("GetSignificantEvents")]
    private string significantEvent = string.Empty;

    [SerializeField, Tooltip("The action hint message")]
    private string actionHintMessage = string.Empty;

    [SerializeField, Tooltip("The item to display")]
    private GameObject itemToDisplay = null;

    [SerializeField, Tooltip("The duration for the item to fade in")]
    private float fadeInDuration = 2.0f;

    [SerializeField, Tooltip("Should a Game Message be shown after the interaction?")]
    private InteractionMessage postInteractionMessage = null;

    public void Awake()
    {
        itemToDisplay.SetActive(false);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool AdvertiseInteraction()
    {
        PlayerInteraction playerInteraction = Globals.Instance?.PlayerInteraction;

        if (playerInteraction.ActionIcon != null)
        {
            playerInteraction.ActionIcon.enabled = true;
            playerInteraction.ActionHintTextMesh.enabled = true;
            playerInteraction.ActionHintTextMesh.text = GameUtils.ActionNameHint("Drop off", name, actionHintMessage);
        }
        return false;
    }

    public bool PerformInteraction()
    {
        Material materialToFadeIn = itemToDisplay.GetComponent<Renderer>().material;

        // Color is a struct, so modify a copy and then reassign
        Color color = materialToFadeIn.color;
        color.a = 0.0f;
        materialToFadeIn.color = color;

        // Handle the significant event, if needed
        if (!string.IsNullOrEmpty(significantEvent))
        {
            QuestManager.HandleSignificantEvent(significantEvent);
        }

        itemToDisplay.SetActive(true);

        StartCoroutine(FadeIn(materialToFadeIn, fadeInDuration));

        GameUtils.SetLayerRecursively(itemToDisplay, "Default");

        // Show the game message
        GameUtils.DisplayInteractionMessage(postInteractionMessage);

        return false; // No further interactions needed
    }

    IEnumerator FadeIn(Material mat, float duration)
    {
        // Color is a struct, so modify a copy and then reassign
        Color color = mat.color;
        color.a = 0f;
        mat.color = color;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsed / duration);
            Debug.Log($"Alpha: {color.a}");
            mat.color = color;
            yield return null;
        }
    }

    //public void SetLayerRecursively(GameObject root, string newLayer)
    //{
    //    int layer = LayerMask.NameToLayer(newLayer);
    //    if (layer == -1)
    //    {
    //        Debug.LogWarning($"Layer \"{newLayer}\" does not exist.");
    //        return;
    //    }

    //    foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
    //    {
    //        t.gameObject.layer = layer;
    //    }
    //}

}