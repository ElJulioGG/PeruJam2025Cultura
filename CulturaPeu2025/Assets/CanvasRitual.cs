using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasRitual : MonoBehaviour
{
    [Header("Visual Elements")]
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text textIzq;
    [SerializeField] private TMP_Text textCent;
    [SerializeField] private TMP_Text textDer;

    [Header("Particle Effects")]
    public GameObject textoPresionar;
    public GameObject particulitas;
    public GameObject lightParticle;

    [Header("Ritual Settings")]
    [SerializeField] private int index;

    private bool isInsideTrigger = false;
    public bool ritualComplete = false;

    void Start()
    {
        textoPresionar.SetActive(false);
        particulitas?.SetActive(false);
        lightParticle?.SetActive(false);
    }

    void Update()
    {
        UpdateTexts();

        if (isInsideTrigger && Input.GetKeyDown(KeyCode.E) && ritualComplete)
        {
            EntregarRitual();
        }
    }

    void UpdateTexts()
    {
        bool izq = false, cent = false, der = false;

        // Evaluar recolección según index de ritual
        switch (index)
        {
            case 0: // Ritual 1
                izq = GameManager.instance.LatigoPickup;
                cent = GameManager.instance.MascaraPickup;
                der = GameManager.instance.CampanitasPickup;
                break;
            case 1: // Ritual 2
                izq = GameManager.instance.BolsaPikcup;
                cent = GameManager.instance.ChumpiPickup;
                der = GameManager.instance.PututuPickup;
                break;
            case 2: // Ritual 3
                izq = GameManager.instance.MullyPickup;
                cent = GameManager.instance.ChichaPickup;
                der = GameManager.instance.ConopasPickup;
                break;
            case 3: // Ritual 4
                izq = GameManager.instance.CuchilloPickup;
                cent = GameManager.instance.CocaPikcup;
                der = GameManager.instance.LatigoPickup; // ejemplo repetido, cámbialo si deseas otro objeto
                break;
                // Puedes continuar agregando más rituales aquí
        }

        textIzq.text = izq ? "1/1" : "0/1";
        textCent.text = cent ? "1/1" : "0/1";
        textDer.text = der ? "1/1" : "0/1";

        ritualComplete = izq && cent && der;
    }

    void EntregarRitual()
    {
        Debug.Log("Ritual entregado!");
        textoPresionar.SetActive(false);
        particulitas?.SetActive(true);
        lightParticle?.SetActive(true);
        // Aquí puedes poner una animación, cambiar sprite, sonido, etc.
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInsideTrigger = true;
            textoPresionar.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInsideTrigger = false;
            textoPresionar.SetActive(false);
        }
    }
}
