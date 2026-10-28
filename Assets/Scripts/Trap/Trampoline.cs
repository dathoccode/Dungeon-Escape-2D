using UnityEngine;

public class Trampoline : MonoBehaviour
{
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private Animator animator;
    [SerializeField] private float pushForce = 10f;


    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            animator.SetTrigger("Jump");
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            rb.linearVelocity = new Vector2(rb.linearVelocityX, pushForce);
        }
    }
}
