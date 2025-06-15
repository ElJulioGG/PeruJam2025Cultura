using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class CanvasRitual : MonoBehaviour
{
    [SerializeField] private Image image; // Reference to the Image component
    
    [SerializeField] private TMP_Text text;
    [SerializeField] private int index;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    //[SerializeField] public bool LatigoPickup = false;
    //[SerializeField] public bool MascaraPickup = false;
    //[SerializeField] public bool CampanitasPickup = false;
    //[SerializeField] public bool BolsaPikcup = false;

    //[SerializeField] public bool ChumpiPickup = false;
    //[SerializeField] public bool PututuPickup = false;
    //[SerializeField] public bool MullyPickup = false;

    //[SerializeField] public bool ChichaPickup = false;
    //[SerializeField] public bool ConopasPickup = false;
    //[SerializeField] public bool CuchilloPickup = false;
    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
    }
    void Update()
    {
        // Check if the ritual is complete and update the image and text accordingly
            switch (index)
            {
                case 0: // Ritual 1
                    if (GameManager.instance.LatigoPickup)
                    {
                        image.color = Color.green;
                        text.text = "1/1";
                    }
                    break;
                case 1: // Ritual 2
                    if (GameManager.instance.MascaraPickup)
                    {
                        image.color = Color.green;
                        text.text = "1/1";
                    }
                    break;
                case 2: // Ritual 3
                    if (GameManager.instance.CampanitasPickup)
                    {
                        image.color = Color.green;
                        text.text = "1/1";
                    }
                    break;
                case 3: // Ritual 4
                    if (GameManager.instance.BolsaPikcup)
                    {
                        image.color = Color.green;
                        text.text = "1/1";
                    }
                    break;
                case 4: // Ritual 5
                    if (GameManager.instance.ChumpiPickup)
                    {
                        image.color = Color.green;
                        text.text = "1/1";
                    }
                    break;
                case 5: // Ritual 6
                    if (GameManager.instance.PututuPickup)
                    {
                        image.color = Color.green;
                        text.text = "1/1";
                    }
                    break;
                case 6: // Ritual 7
                    if (GameManager.instance.MullyPickup)
                    {
                        image.color = Color.green;
                        text.text = "1/1";
                    }
                    break;
                case 7: // Ritual 8
                    if (GameManager.instance.ChichaPickup)
                    {
                        image.color = Color.green;
                        text.text = "1/1";
                    }
                    break;
                case 8: // Ritual 9
                    if (GameManager.instance.ConopasPickup)
                    {
                        image.color = Color.green;
                        text.text = "1/1";
                    }
                    break;
                case 9: // Ritual 10
                    if (GameManager.instance.CuchilloPickup)
                    {
                        image.color = Color.green;
                        text.text = "1/1";
                    }
                    break;
                case 10: // Ritual 11   
                    if (GameManager.instance.CocaPikcup)
                    {
                        image.color = Color.green;
                        text.text = "1/1";
                    }
                    break;
            }
        }
    
}
