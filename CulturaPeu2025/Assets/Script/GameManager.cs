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

    private void Awake()
    {
        // AudioManager.instance.PlayMusic("StealthTheme");//Only for now 

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
    public void setPlayerCanMove(bool newState)
    {
        playerCanMove = newState;
    }
    private void Start()
    {
        //EventManager.OnTimerStart();
    }
}
