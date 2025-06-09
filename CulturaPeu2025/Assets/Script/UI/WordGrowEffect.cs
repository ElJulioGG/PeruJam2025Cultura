using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class WordGrowFadeEffect : MonoBehaviour
{
    [Header("Timing Settings")]
    public float delayBetweenWords = 0.3f;
    public float growDuration = 0.5f;
    public float waitBeforeFade = 3f; 
    public float fadeDuration = 0.2f;

    [Header("Object Switching")]
    public GameObject objectToActivateAfterFade; 

    private TMP_Text textMesh;
    private Mesh mesh;
    private Vector3[] vertices;
    private Color32[] colors;

    private List<int> wordIndexes = new List<int>();
    private List<int> wordLengths = new List<int>();

    private float startTime;
    private bool faded = false;

    void Start()
    {
        textMesh = GetComponent<TMP_Text>();
        textMesh.ForceMeshUpdate();

        wordIndexes.Add(0);
        string s = textMesh.text;

        for (int index = s.IndexOf(' '); index > -1; index = s.IndexOf(' ', index + 1))
        {
            wordLengths.Add(index - wordIndexes[wordIndexes.Count - 1]);
            wordIndexes.Add(index + 1);
        }
        wordLengths.Add(s.Length - wordIndexes[wordIndexes.Count - 1]);

        startTime = Time.time;
    }

    void Update()
    {
        textMesh.ForceMeshUpdate();
        mesh = textMesh.mesh;
        vertices = mesh.vertices;
        colors = mesh.colors32;

        bool allVisible = true;

        for (int w = 0; w < wordIndexes.Count; w++)
        {
            float wordAppearTime = startTime + w * delayBetweenWords;
            float t = (Time.time - wordAppearTime) / growDuration;
            float scale = Mathf.Clamp01(t);
            scale = Mathf.SmoothStep(0, 1, scale);

            if (scale < 1f) allVisible = false;

            int startCharIndex = wordIndexes[w];
            int wordLength = wordLengths[w];

            for (int i = 0; i < wordLength; i++)
            {
                TMP_CharacterInfo c = textMesh.textInfo.characterInfo[startCharIndex + i];
                if (!c.isVisible) continue;

                int index = c.vertexIndex;

                Vector3[] charVerts = new Vector3[4];
                for (int j = 0; j < 4; j++)
                    charVerts[j] = vertices[index + j];

                Vector3 center = (charVerts[0] + charVerts[2]) * 0.5f;

                for (int j = 0; j < 4; j++)
                    vertices[index + j] = center + (charVerts[j] - center) * scale;
            }
        }

        float fadeTriggerTime = startTime + (wordIndexes.Count * delayBetweenWords) + waitBeforeFade;

        if (Time.time >= fadeTriggerTime && !faded)
        {
            StartCoroutine(FadeOutAndSwitch());
            faded = true;
        }

        mesh.vertices = vertices;
        mesh.colors32 = colors;
        textMesh.canvasRenderer.SetMesh(mesh);
    }

    IEnumerator FadeOutAndSwitch()
    {
        float elapsed = 0f;
        textMesh.ForceMeshUpdate();
        mesh = textMesh.mesh;
        colors = mesh.colors32;

        Color32[] startColors = (Color32[])colors.Clone();

        while (elapsed < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = new Color32(
                    startColors[i].r,
                    startColors[i].g,
                    startColors[i].b,
                    (byte)(255 * alpha)
                );
            }

            mesh.colors32 = colors;
            textMesh.canvasRenderer.SetMesh(mesh);

            elapsed += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < colors.Length; i++)
            colors[i].a = 0;

        mesh.colors32 = colors;
        textMesh.canvasRenderer.SetMesh(mesh);

        gameObject.SetActive(false); 
        if (objectToActivateAfterFade != null)
            objectToActivateAfterFade.SetActive(true); 
    }
}
