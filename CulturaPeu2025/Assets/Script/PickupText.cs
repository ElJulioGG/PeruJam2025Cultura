using TMPro;
using UnityEngine;
using UnityEngine.UI; // Required for Image

public class PickupText : MonoBehaviour
{
    [SerializeField] private int index = 0; // 0 = food1, 1 = food2, 2 = food3
    [SerializeField] private Sprite activeSprite; // Sprite to show when quantity > 0
    [SerializeField] private Image uiImage; // Reference to UI Image component

    private TextMeshProUGUI textComponent;

    void Start()
    {
        textComponent = GetComponent<TextMeshProUGUI>();

        if (uiImage != null)
            uiImage.enabled = true; // Make sure it's visible

        UpdateUI();
    }

    void Update()
    {
        UpdateUI(); // Can optimize with events later
    }

    void UpdateUI()
    {
        if (GameManager.instance == null) return;

        int quantity = 0;

        switch (index)
        {
            case 0:
                quantity = GameManager.instance.food1Cuantity;
                break;
            case 1:
                quantity = GameManager.instance.food2Cuantity;
                break;
            case 2:
                quantity = GameManager.instance.food3Cuantity;
                break;
        }

        if (textComponent != null)
            textComponent.text = "x" + quantity;

        // Only update the sprite if quantity > 0
        if (uiImage != null && quantity > 0 && activeSprite != null)
        {
            uiImage.sprite = activeSprite;
        }
    }
}
