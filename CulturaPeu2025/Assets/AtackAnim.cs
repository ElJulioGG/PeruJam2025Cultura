using UnityEngine;

public class AtackAnim : MonoBehaviour
{
    [SerializeField] private Animator animator;

    void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            animator.Play("AtackPlayer", 0, 0f); // Play from the start
        }
    }
}
