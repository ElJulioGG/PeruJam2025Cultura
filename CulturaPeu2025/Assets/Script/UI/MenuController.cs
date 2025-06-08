using UnityEngine;
using System.Collections;
using Unity.Collections;
using DG.Tweening;
using TMPro;

public class MenuController : MonoBehaviour
{
    // === Elementos principales del menú ===
    [Header("Elementos principales del menú")]
    public Transform object1;
    public Transform object2;
    public Transform object3;
    public Transform Copiryght;

    [Space]
    public float moveDistance = 5f;
    public float moveDistanceCopiryght = 5f;
    public float moveDuration = 1f;
    public float delayBetweenMoves = 0.5f;

    private bool isMoving = false;
    private bool moved = false;

    // === Créditos del Team ===
    [Header("Créditos del Team")]
    public Transform creditosTeamBloque1;
    public Transform creditosTeamBloque2;

    public Transform creditosTeamBloqueRefuerzo1;
    public Transform creditosTeamBloqueRefuerzo2;
    public GameObject creditosTeamBloqueR1;
    public GameObject creditosTeamBloqueR2;


    public float moveDurationCreditos = 1f;
    public float moveDistancecreditosTeam1;
    public float moveDistancecreditosTeam2;

    [Space]
    public GameObject CreditoInstruccions;
    public TextMeshProUGUI creditosTeamText1;
    public float CreditsDistanceInstruccions = -300f;

    // === Roles y Títulos ===
    [Header("Roles y Títulos")]
    public Transform CTitulo;

    [Space]
    public float CreditsDistance = 700f;
    public Transform CRoleGDD;
    public Transform CRoleGDDIntegrants;

    [Space]
    public Transform CRoleProgramador;
    public Transform CRoleProgramadorIntegrants;

    [Space]
    public Transform CRoleArtista;
    public Transform CRoleArtistaIntegrants;

    [Space]
    public Transform CRoleMusica;
    public Transform CRoleMusicaIntegrants;

    [Space]
    public Transform CRoleExterno;
    public Transform CRoleExternoIntegrants;

    // === Integrantes ===
    [Header("Integrantes")]
    public GameObject Integrant1;
    public GameObject Integrant2;
    public GameObject Integrant3;
    public GameObject Integrant4;

    // === Otros ===
    [Header("Otros")]
    public Transform Piso;
    public AudioSource soundeffect;

    void Start()
    {
        StartCoroutine(MoveObjectsStart());
        soundeffect = GetComponent<AudioSource>();
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape) && !isMoving && !moved)
        {
            StartCoroutine(MoveObjects());
        }
    }

    public void StartMoveObjects()
{
    if (!isMoving && !moved)
    {
        StartCoroutine(MoveObjects());
     }
}

public void StartReturnObjects()
{
    if (!isMoving && moved)
    {
        StartCoroutine(ReturnObjects());
            moved = false;
        }
}


    public IEnumerator MoveObjectsStart()
    {
        isMoving = true;
        {
            soundeffect.Play();
            Tween t1 = object1.DOMoveX(object1.position.x + moveDistance, moveDuration).SetEase(Ease.OutExpo);
            yield return t1.WaitForCompletion();
            yield return new WaitForSeconds(delayBetweenMoves);
            soundeffect.Play();

            Tween t2 = object2.DOMoveX(object2.position.x + moveDistance, moveDuration).SetEase(Ease.OutExpo);
            yield return t2.WaitForCompletion();
            yield return new WaitForSeconds(delayBetweenMoves);
            soundeffect.Play();

            Tween t3 = object3.DOMoveX(object3.position.x + moveDistance, moveDuration).SetEase(Ease.OutExpo);
            yield return t3.WaitForCompletion();
            yield return new WaitForSeconds(delayBetweenMoves);
            soundeffect.Play();

            Tween t4 = Copiryght.DOMoveX(Copiryght.position.x + moveDistanceCopiryght, moveDuration).SetEase(Ease.OutExpo);
            yield return t4.WaitForCompletion();

            moved = true;
            isMoving = false;
        }
         
        
    }

    public IEnumerator ReturnObjects()
    {
        isMoving = true;
        {
            soundeffect.Play();

            Tween t1 = object1.DOMoveX(object1.position.x - moveDistance, moveDuration).SetEase(Ease.OutExpo);
            yield return t1.WaitForCompletion();
            yield return new WaitForSeconds(delayBetweenMoves);
            soundeffect.Play();

            Tween t2 = object2.DOMoveX(object2.position.x - moveDistance, moveDuration).SetEase(Ease.OutExpo);
            yield return t2.WaitForCompletion();
            yield return new WaitForSeconds(delayBetweenMoves);
            soundeffect.Play();

            Tween t3 = object3.DOMoveX(object3.position.x - moveDistance, moveDuration).SetEase(Ease.OutExpo);
            yield return t3.WaitForCompletion();
            yield return new WaitForSeconds(delayBetweenMoves);
            soundeffect.Play();

            Tween t4 = Copiryght.DOMoveX(Copiryght.position.x - moveDistanceCopiryght, moveDuration).SetEase(Ease.OutExpo);
            yield return t4.WaitForCompletion();

            Tween t5 = creditosTeamBloque1.DOMoveY(creditosTeamBloque1.position.y - moveDistancecreditosTeam1, moveDurationCreditos).SetEase(Ease.OutExpo);
            Tween t6 = creditosTeamBloque2.DOMoveY(creditosTeamBloque2.position.y - moveDistancecreditosTeam2, moveDurationCreditos).SetEase(Ease.OutExpo);
            soundeffect.Play();

            Tween t7 = CTitulo.DOMoveY(CTitulo.position.y - CreditsDistance, moveDurationCreditos).SetEase(Ease.OutExpo);
            yield return t7.WaitForCompletion();
            yield return new WaitForSeconds(delayBetweenMoves);
            soundeffect.Play();

            Tween t8 = CRoleGDD.DOMoveY(CRoleGDD.position.y - CreditsDistance, moveDurationCreditos).SetEase(Ease.OutExpo);
            yield return t8.WaitForCompletion();
            yield return new WaitForSeconds(delayBetweenMoves);
            soundeffect.Play();

            Tween t9 = CRoleGDDIntegrants.DOMoveY(CRoleGDDIntegrants.position.y - CreditsDistance, moveDurationCreditos).SetEase(Ease.OutExpo);
            yield return t9.WaitForCompletion();
            yield return new WaitForSeconds(delayBetweenMoves);
            soundeffect.Play();

            Tween t10 = CRoleProgramador.DOMoveY(CRoleProgramador.position.y - CreditsDistance, moveDurationCreditos).SetEase(Ease.OutExpo);
            yield return t10.WaitForCompletion();
            yield return new WaitForSeconds(delayBetweenMoves);
            soundeffect.Play();

            Tween t11 = CRoleProgramadorIntegrants.DOMoveY(CRoleProgramadorIntegrants.position.y - CreditsDistance, moveDurationCreditos).SetEase(Ease.OutExpo);
            yield return t11.WaitForCompletion();
            yield return new WaitForSeconds(delayBetweenMoves);
            soundeffect.Play();

            Tween t12 = CRoleArtista.DOMoveY(CRoleArtista.position.y - CreditsDistance, moveDurationCreditos).SetEase(Ease.OutExpo);
            yield return t12.WaitForCompletion();
            yield return new WaitForSeconds(delayBetweenMoves);
            soundeffect.Play();

            Tween t13 = CRoleArtistaIntegrants.DOMoveY(CRoleArtistaIntegrants.position.y - CreditsDistance, moveDurationCreditos).SetEase(Ease.OutExpo);
            yield return t13.WaitForCompletion();
            yield return new WaitForSeconds(delayBetweenMoves);
            soundeffect.Play();

            Tween t14 = CRoleMusica.DOMoveY(CRoleMusica.position.y - CreditsDistance, moveDurationCreditos).SetEase(Ease.OutExpo);
            yield return t14.WaitForCompletion();
            yield return new WaitForSeconds(delayBetweenMoves);
            soundeffect.Play();

            Tween t15 = CRoleMusicaIntegrants.DOMoveY(CRoleMusicaIntegrants.position.y - CreditsDistance, moveDurationCreditos).SetEase(Ease.OutExpo);
            yield return t15.WaitForCompletion();
            yield return new WaitForSeconds(delayBetweenMoves);
            soundeffect.Play();

            Tween t16 = CRoleExterno.DOMoveY(CRoleExterno.position.y - CreditsDistance, moveDurationCreditos).SetEase(Ease.OutExpo);
            yield return t16.WaitForCompletion();
            yield return new WaitForSeconds(delayBetweenMoves);
            soundeffect.Play();

            Tween t17 = CRoleExternoIntegrants.DOMoveY(CRoleExternoIntegrants.position.y - CreditsDistance, moveDurationCreditos).SetEase(Ease.OutExpo);
            yield return t17.WaitForCompletion();
            yield return new WaitForSeconds(delayBetweenMoves);
            soundeffect.Play();

            Tween t18 = Piso.DOMoveY(Piso.position.y - CreditsDistance, moveDurationCreditos).SetEase(Ease.OutExpo);
            yield return t18.WaitForCompletion();
            yield return new WaitForSeconds(delayBetweenMoves);
            soundeffect.Play();

            Tween t19 = Integrant1.transform.DOMoveY(Integrant1.transform.position.y - CreditsDistance, moveDurationCreditos).SetEase(Ease.OutExpo);
            yield return t19.WaitForCompletion();
            yield return new WaitForSeconds(delayBetweenMoves);
            soundeffect.Play();

            Tween t20 = Integrant2.transform.DOMoveY(Integrant2.transform.position.y - CreditsDistance, moveDurationCreditos).SetEase(Ease.OutExpo);
            yield return t20.WaitForCompletion();
            yield return new WaitForSeconds(delayBetweenMoves);
            soundeffect.Play();

            Tween t21 = Integrant3.transform.DOMoveY(Integrant3.transform.position.y - CreditsDistance, moveDurationCreditos).SetEase(Ease.OutExpo);
            yield return t21.WaitForCompletion();
            yield return new WaitForSeconds(delayBetweenMoves);
            soundeffect.Play();

            Tween t22 = Integrant4.transform.DOMoveY(Integrant4.transform.position.y - CreditsDistance, moveDurationCreditos).SetEase(Ease.OutExpo);
            yield return t22.WaitForCompletion();
            yield return new WaitForSeconds(delayBetweenMoves);
            soundeffect.Play();

            Tween t23 = CreditoInstruccions.transform.DOMoveY(CreditoInstruccions.transform.position.y - CreditsDistanceInstruccions, moveDurationCreditos).SetEase(Ease.OutExpo);
            yield return t23.WaitForCompletion();
            yield return new WaitForSeconds(delayBetweenMoves);


            soundeffect.Play();

            Tween t24 = creditosTeamBloqueRefuerzo1.DOMoveY(creditosTeamBloqueRefuerzo1.position.y - moveDistancecreditosTeam1, moveDurationCreditos).SetEase(Ease.OutExpo);
            yield return t24.WaitForCompletion();
            yield return new WaitForSeconds(delayBetweenMoves);

            soundeffect.Play();

            Tween t25 = creditosTeamBloqueRefuerzo2.DOMoveY(creditosTeamBloqueRefuerzo2.position.y - moveDistancecreditosTeam2, moveDurationCreditos).SetEase(Ease.OutExpo);
            yield return t25.WaitForCompletion();
            yield return new WaitForSeconds(delayBetweenMoves);
            


            moved = false;
            isMoving = false;
        }

    }

    public IEnumerator MoveObjects()
    {
        isMoving = true;
        {
            soundeffect.Play();

            Tween t1 = object1.DOMoveX(object1.position.x + moveDistance, moveDuration).SetEase(Ease.OutExpo);
            yield return t1.WaitForCompletion();
            yield return new WaitForSeconds(delayBetweenMoves);
            soundeffect.Play();

            Tween t2 = object2.DOMoveX(object2.position.x + moveDistance, moveDuration).SetEase(Ease.OutExpo);
            yield return t2.WaitForCompletion();
            yield return new WaitForSeconds(delayBetweenMoves);
            soundeffect.Play();

            Tween t3 = object3.DOMoveX(object3.position.x + moveDistance, moveDuration).SetEase(Ease.OutExpo);
            yield return t3.WaitForCompletion();
            yield return new WaitForSeconds(delayBetweenMoves);
            soundeffect.Play();

            Tween t4 = Copiryght.DOMoveX(Copiryght.position.x + moveDistanceCopiryght, moveDuration).SetEase(Ease.OutExpo);
            yield return t4.WaitForCompletion();
            soundeffect.Play();


            Tween t5 = creditosTeamBloque1.DOMoveY(creditosTeamBloque1.position.y + moveDistancecreditosTeam1, moveDurationCreditos).SetEase(Ease.OutExpo);
            Tween t6 = creditosTeamBloque2.DOMoveY(creditosTeamBloque2.position.y + moveDistancecreditosTeam2, moveDurationCreditos).SetEase(Ease.OutExpo);
            soundeffect.Play();

            Tween t7 = CTitulo.DOMoveY(CTitulo.position.y + CreditsDistance, moveDurationCreditos).SetEase(Ease.OutExpo);
            yield return t7.WaitForCompletion();
            yield return new WaitForSeconds(delayBetweenMoves);
            soundeffect.Play();

            Tween t8 = CRoleGDD.DOMoveY(CRoleGDD.position.y + CreditsDistance, moveDurationCreditos).SetEase(Ease.OutExpo);
            yield return t8.WaitForCompletion();
            yield return new WaitForSeconds(delayBetweenMoves);
            soundeffect.Play();

            Tween t9 = CRoleGDDIntegrants.DOMoveY(CRoleGDDIntegrants.position.y + CreditsDistance, moveDurationCreditos).SetEase(Ease.OutExpo);
            yield return t9.WaitForCompletion();
            yield return new WaitForSeconds(delayBetweenMoves);
            soundeffect.Play();

            Tween t10 = CRoleProgramador.DOMoveY(CRoleProgramador.position.y + CreditsDistance, moveDurationCreditos).SetEase(Ease.OutExpo);
            yield return t10.WaitForCompletion();
            yield return new WaitForSeconds(delayBetweenMoves);
            soundeffect.Play();

            Tween t11 = CRoleProgramadorIntegrants.DOMoveY(CRoleProgramadorIntegrants.position.y + CreditsDistance, moveDurationCreditos).SetEase(Ease.OutExpo);
            yield return t11.WaitForCompletion();
            yield return new WaitForSeconds(delayBetweenMoves);
            soundeffect.Play();

            Tween t12 = CRoleArtista.DOMoveY(CRoleArtista.position.y + CreditsDistance, moveDurationCreditos).SetEase(Ease.OutExpo);
            yield return t12.WaitForCompletion();
            yield return new WaitForSeconds(delayBetweenMoves);
            soundeffect.Play();

            Tween t13 = CRoleArtistaIntegrants.DOMoveY(CRoleArtistaIntegrants.position.y + CreditsDistance, moveDurationCreditos).SetEase(Ease.OutExpo);
            yield return t13.WaitForCompletion();
            yield return new WaitForSeconds(delayBetweenMoves);
            soundeffect.Play();

            Tween t14 = CRoleMusica.DOMoveY(CRoleMusica.position.y + CreditsDistance, moveDurationCreditos).SetEase(Ease.OutExpo);
            yield return t14.WaitForCompletion();
            yield return new WaitForSeconds(delayBetweenMoves);
            soundeffect.Play();

            Tween t15 = CRoleMusicaIntegrants.DOMoveY(CRoleMusicaIntegrants.position.y + CreditsDistance, moveDurationCreditos).SetEase(Ease.OutExpo);
            yield return t15.WaitForCompletion();
            yield return new WaitForSeconds(delayBetweenMoves);
            soundeffect.Play();

            Tween t16 = CRoleExterno.DOMoveY(CRoleExterno.position.y + CreditsDistance, moveDurationCreditos).SetEase(Ease.OutExpo);
            yield return t16.WaitForCompletion();
            yield return new WaitForSeconds(delayBetweenMoves);
            soundeffect.Play();

            Tween t17 = CRoleExternoIntegrants.DOMoveY(CRoleExternoIntegrants.position.y + CreditsDistance, moveDurationCreditos).SetEase(Ease.OutExpo);
            yield return t17.WaitForCompletion();
            yield return new WaitForSeconds(delayBetweenMoves);
            soundeffect.Play();

            Tween t18 = Piso.DOMoveY(Piso.position.y + CreditsDistance, moveDurationCreditos).SetEase(Ease.OutExpo);
            yield return t18.WaitForCompletion();
            yield return new WaitForSeconds(delayBetweenMoves);
            soundeffect.Play();

            Tween t19 = Integrant1.transform.DOMoveY(Integrant1.transform.position.y + CreditsDistance, moveDurationCreditos).SetEase(Ease.OutExpo);
            yield return t19.WaitForCompletion();
            yield return new WaitForSeconds(delayBetweenMoves);
            soundeffect.Play();

            Tween t20 = Integrant2.transform.DOMoveY(Integrant2.transform.position.y + CreditsDistance, moveDurationCreditos).SetEase(Ease.OutExpo);
            yield return t20.WaitForCompletion();
            yield return new WaitForSeconds(delayBetweenMoves);
            soundeffect.Play();

            Tween t21 = Integrant3.transform.DOMoveY(Integrant3.transform.position.y + CreditsDistance, moveDurationCreditos).SetEase(Ease.OutExpo);
            yield return t21.WaitForCompletion();
            yield return new WaitForSeconds(delayBetweenMoves);
            soundeffect.Play();

            Tween t22 = Integrant4.transform.DOMoveY(Integrant4.transform.position.y + CreditsDistance, moveDurationCreditos).SetEase(Ease.OutExpo);
            yield return t22.WaitForCompletion();
            yield return new WaitForSeconds(delayBetweenMoves);
            soundeffect.Play();

            Tween t23 = CreditoInstruccions.transform.DOMoveY(CreditoInstruccions.transform.position.y + CreditsDistanceInstruccions, moveDurationCreditos).SetEase(Ease.OutExpo);
            yield return t23.WaitForCompletion();
            yield return new WaitForSeconds(delayBetweenMoves);


            Tween t24 = creditosTeamBloqueRefuerzo1.DOMoveY(creditosTeamBloqueRefuerzo1.position.y + moveDistancecreditosTeam1, moveDurationCreditos).SetEase(Ease.OutExpo);
            yield return t24.WaitForCompletion();
            yield return new WaitForSeconds(delayBetweenMoves);

            Tween t25 = creditosTeamBloqueRefuerzo2.DOMoveY(creditosTeamBloqueRefuerzo2.position.y + moveDistancecreditosTeam2, moveDurationCreditos).SetEase(Ease.OutExpo);
            yield return t25.WaitForCompletion();
            yield return new WaitForSeconds(delayBetweenMoves);


            moved = true;
            isMoving = false;
        }


    }
}
