using UnityEngine;
using DG.Tweening;

public class bossWeapon : MonoBehaviour
{
    private Vector3 originalPosition;

    void Start()
    {
        originalPosition = transform.localPosition;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            ShakeWeapon();
        }
    }

    void ShakeWeapon()
    {
        // Kill any existing tween to prevent overlapping
        DOTween.Kill(transform);

        Sequence shakeSequence = DOTween.Sequence();
        shakeSequence.Append(transform.DOLocalMoveX(originalPosition.x + 0.65f, 0.1f))
                     .Append(transform.DOLocalMoveX(originalPosition.x, 0.2f));
    }
}
