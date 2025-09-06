using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Player Variables")]
    [SerializeField] public int lastSelectedNote = 0;
    [SerializeField] public float securityLevel = 0f;

    [Header("Player Stats")]
    [SerializeField] public int playerHealth = 3;

    [Header("Player Status")]
    [SerializeField] public bool playerCanDialog = true;
    [SerializeField] public bool playerIsInDialog = true;
    [SerializeField] public bool playerCanMove = true;
    [SerializeField] public bool playerIsHit = false;
    [SerializeField] public bool playerDied = false;
    [SerializeField] public bool playerCanAction = true;
    [SerializeField] public bool playerInvincibility = false;
    [SerializeField] public bool playerHasWeapon = false;

    [SerializeField] public int floorType = 0;

    [Header("Items")]
    [SerializeField] public bool LatigoPickup = false;
    [SerializeField] public bool MascaraPickup = false;
    [SerializeField] public bool CampanitasPickup = false;
    [SerializeField] public bool BolsaPikcup = false;

    [SerializeField] public bool ChumpiPickup = false;
    [SerializeField] public bool PututuPickup = false;
    [SerializeField] public bool MullyPickup = false;

    [SerializeField] public bool ChichaPickup = false;
    [SerializeField] public bool ConopasPickup = false;
    [SerializeField] public bool CuchilloPickup = false;

    [SerializeField] public bool CocaPikcup = false;

    [SerializeField] public int food1Cuantity = 0;
    [SerializeField] public int food2Cuantity = 0;
    [SerializeField] public int food3Cuantity = 0;

    private Vector3 lastCheckpoint;

    private bool savedLatigoPickup, savedMascaraPickup, savedCampanitasPickup, savedBolsaPikcup;
    private bool savedChumpiPickup, savedPututuPickup, savedMullyPickup;
    private bool savedChichaPickup, savedConopasPickup, savedCuchilloPickup;
    private bool savedCocaPikcup;

    private int savedFood1Cuantity, savedFood2Cuantity, savedFood3Cuantity;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            lastCheckpoint = player.transform.position;

        SaveState();
    }

    public void setPlayerCanMove(bool newState)
    {
        playerCanMove = newState;
    }

    public void SetCheckpoint(Vector3 checkpointPos)
    {
        lastCheckpoint = checkpointPos;
        SaveState();
    }

    private void SaveState()
    {
        savedLatigoPickup = LatigoPickup;
        savedMascaraPickup = MascaraPickup;
        savedCampanitasPickup = CampanitasPickup;
        savedBolsaPikcup = BolsaPikcup;

        savedChumpiPickup = ChumpiPickup;
        savedPututuPickup = PututuPickup;
        savedMullyPickup = MullyPickup;

        savedChichaPickup = ChichaPickup;
        savedConopasPickup = ConopasPickup;
        savedCuchilloPickup = CuchilloPickup;

        savedCocaPikcup = CocaPikcup;

        savedFood1Cuantity = food1Cuantity;
        savedFood2Cuantity = food2Cuantity;
        savedFood3Cuantity = food3Cuantity;
    }

    private void LoadState()
    {
        LatigoPickup = savedLatigoPickup;
        MascaraPickup = savedMascaraPickup;
        CampanitasPickup = savedCampanitasPickup;
        BolsaPikcup = savedBolsaPikcup;

        ChumpiPickup = savedChumpiPickup;
        PututuPickup = savedPututuPickup;
        MullyPickup = savedMullyPickup;

        ChichaPickup = savedChichaPickup;
        ConopasPickup = savedConopasPickup;
        CuchilloPickup = savedCuchilloPickup;

        CocaPikcup = savedCocaPikcup;

        food1Cuantity = savedFood1Cuantity;
        food2Cuantity = savedFood2Cuantity;
        food3Cuantity = savedFood3Cuantity;
    }

    public void RespawnPlayer(GameObject player)
    {
        player.transform.position = lastCheckpoint;

        LoadState();

        playerHealth = 3;
        playerDied = false;
        playerCanMove = true;
    }

}
