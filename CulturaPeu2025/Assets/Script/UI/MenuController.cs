using UnityEngine;
using System.Collections;
using Unity.Collections;
using DG.Tweening;

public class MenuController : MonoBehaviour
{
    public Transform object1;
    public Transform object2;
    public Transform object3;
    public Transform Copiryght;
    public float moveDistance = 5f;
    public float moveDistanceCopiryght = 5f;
    public float moveDuration = 1f;
    public float delayBetweenMoves = 0.5f;

    private bool isMoving = false;
    private bool moved = false;

    [Header("Créditos del Team")]

    private bool creditosTeamMOve;
    public Transform creditosTeamBloque1;
    public Transform creditosTeamBloque2;
    public float moveDurationCreditos = 1f;
    public Transform creditosTeam;



    void Start()
    {
        StartCoroutine(MoveObjects());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isMoving && !moved)
        {
            StartCoroutine(MoveObjects());
        }

        if (Input.GetKeyDown(KeyCode.Escape) && !isMoving && moved)
        {
            StartCoroutine(ReturnObjects());
        }
    }

    private IEnumerator MoveObjects()
    {
        isMoving = true;
        {
            Tween t1 = object1.DOMoveX(object1.position.x + moveDistance, moveDuration).SetEase(Ease.OutExpo);
            yield return t1.WaitForCompletion();
            yield return new WaitForSeconds(delayBetweenMoves);

            Tween t2 = object2.DOMoveX(object2.position.x + moveDistance, moveDuration).SetEase(Ease.OutExpo);
            yield return t2.WaitForCompletion();
            yield return new WaitForSeconds(delayBetweenMoves);

            Tween t3 = object3.DOMoveX(object3.position.x + moveDistance, moveDuration).SetEase(Ease.OutExpo);
            yield return t3.WaitForCompletion();
            yield return new WaitForSeconds(delayBetweenMoves);

            Tween t4 = Copiryght.DOMoveX(Copiryght.position.x + moveDistanceCopiryght, moveDuration).SetEase(Ease.OutExpo);
            yield return t4.WaitForCompletion();

            moved = true;
            isMoving = false;
        }
         
        
    }

    private IEnumerator ReturnObjects()
    {
        isMoving = true;
        {

            Tween t1 = object1.DOMoveX(object1.position.x - moveDistance, moveDuration).SetEase(Ease.OutExpo);
            yield return t1.WaitForCompletion();
            yield return new WaitForSeconds(delayBetweenMoves);

            Tween t2 = object2.DOMoveX(object2.position.x - moveDistance, moveDuration).SetEase(Ease.OutExpo);
            yield return t2.WaitForCompletion();
            yield return new WaitForSeconds(delayBetweenMoves);

            Tween t3 = object3.DOMoveX(object3.position.x - moveDistance, moveDuration).SetEase(Ease.OutExpo);
            yield return t3.WaitForCompletion();
            yield return new WaitForSeconds(delayBetweenMoves);

            Tween t4 = Copiryght.DOMoveX(Copiryght.position.x - moveDistanceCopiryght, moveDuration).SetEase(Ease.OutExpo);
            yield return t4.WaitForCompletion();

            moved = false;
            isMoving = false;
        }

    }
}
