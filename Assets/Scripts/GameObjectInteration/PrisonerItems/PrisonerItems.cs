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

    private bool hasInteracted = false;

    public void Awake()
    {
        itemToDisplay.SetActive(false);
        hasInteracted = false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool AdvertiseInteraction()
    {
        if (hasInteracted)
        {
            return false; // No further interactions needed
        }

        PlayerInteraction playerInteraction = Globals.Instance?.PlayerInteraction;

        if (playerInteraction.ActionIcon != null)
        {
            //playerInteraction.ActionIcon.enabled = true;
            //playerInteraction.ActionHintTextMesh.enabled = true;
            playerInteraction.ActionHintTextMesh.text = GameUtils.ActionNameHint("Drop off", name, actionHintMessage);
        }
        return false;
    }

    public bool PerformInteraction()
    {
        if (hasInteracted)
        {
            return false; // No further interactions needed
        }

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

        hasInteracted = true;

        // Show the game message
        GameUtils.DisplayInteractionMessage(postInteractionMessage);

        GameUtils.SetLayerRecursively(this.gameObject, "Default");

    }
}