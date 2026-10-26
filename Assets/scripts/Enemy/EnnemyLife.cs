using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLife : MonoBehaviour
{
    Rigidbody2D rb;
    Animator anim;
    BoxCollider2D boxCollider2D;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider2D = GetComponent<BoxCollider2D>();
    }
    public void Die()
    {
        boxCollider2D.enabled = false;
        rb.bodyType = RigidbodyType2D.Static;
        anim.SetTrigger("death");
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
