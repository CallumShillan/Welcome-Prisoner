using UnityEngine;
using TMPro;

public class TickerTape : TextMeshProUGUI
{
    public float scrollSpeed = 0.00001f;
    private float scrollOffset = 0f;

    protected override void Awake()
    {
        base.Awake();
    }

    void Update()
    {
        scrollOffset += scrollSpeed * Time.deltaTime;
        UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
    }

    public override void UpdateVertexData(TMP_VertexDataUpdateFlags flags)
    {
        base.UpdateVertexData(flags);

        if (textInfo == null || textInfo.characterCount == 0)
            return;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible)
                continue;

            int vertexIndex = charInfo.vertexIndex;
            Vector3[] vertices = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;

            float offset = scrollOffset % (rectTransform.rect.width + charInfo.textElement.glyph.metrics.horizontalAdvance);

            vertices[vertexIndex + 0].x -= offset;
            vertices[vertexIndex + 1].x -= offset;
            vertices[vertexIndex + 2].x -= offset;
            vertices[vertexIndex + 3].x -= offset;
        }

        // Update the mesh with the new vertex positions
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            textInfo.meshInfo[i].mesh.uv = textInfo.meshInfo[i].uvs0;
            textInfo.meshInfo[i].mesh.uv2 = textInfo.meshInfo[i].uvs2;
            textInfo.meshInfo[i].mesh.colors32 = textInfo.meshInfo[i].colors32;
            canvasRenderer.SetMesh(textInfo.meshInfo[i].mesh);
        }
    }
}
