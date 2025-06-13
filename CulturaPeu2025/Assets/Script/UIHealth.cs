using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

public class HeartUIManager : MonoBehaviour
{
    [SerializeField] private List<Image> heartImages;
    [SerializeField] private Sprite emptyHeartSprite;
    [SerializeField] private Sprite halfHeartSprite;
    [SerializeField] private Sprite fullHeartSprite;

    [Header("Shake Settings")]
    [SerializeField] private float shakeDuration = 0.2f;
    [SerializeField] private float shakeStrength = 0.3f;
    [SerializeField] private int shakeVibrato = 10;

    private int lastHP = -1;

    void Update()
    {
        if (GameManager.instance == null) return;

        int currentHP = Mathf.Clamp(GameManager.instance.playerHealth, 0, 6);

        if (currentHP != lastHP)
        {
            UpdateHeartsUI(currentHP);
            lastHP = currentHP;
        }
    }

    void UpdateHeartsUI(int currentHP)
    {
        for (int i = 0; i < heartImages.Count; i++)
        {
            int heartValue = Mathf.Clamp(currentHP - (i * 2), 0, 2);

            Sprite newSprite = heartValue switch
            {
                2 => fullHeartSprite,
                1 => halfHeartSprite,
                _ => emptyHeartSprite
            };

            // Only shake if the heart visually changed
            if (heartImages[i].sprite != newSprite)
            {
                ShakeHeart(heartImages[i].transform);
                heartImages[i].sprite = newSprite;
            }
        }
    }

    void ShakeHeart(Transform heartTransform)
    {
        heartTransform.DOComplete(); // Stop any ongoing tween
        heartTransform.localScale = Vector3.one; // Reset scale
        heartTransform
            .DOShakeScale(shakeDuration, shakeStrength, shakeVibrato)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => heartTransform.localScale = Vector3.one); // Optional reset
    }
}
