using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{

    public enum MovementState
    {
        Idle,
        Running
    }
    float walkSpeed = 1f;
    public enum FacingDirection
    {
        Right,
        Left
    }
    public FacingDirection facingDirection = FacingDirection.Right;
    public MovementState state = MovementState.Idle;
    public SpriteRenderer sprite;
    public Animator anim;
    // Start is called before the first frame update
    void Start()
    { 
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (facingDirection == FacingDirection.Right)
        {
            // translate the mob to the right
            transform.Translate(Vector2.right * walkSpeed * Time.deltaTime);
        }
        else
        {
            // translate the mob to the left
            transform.Translate(Vector2.left * walkSpeed * Time.deltaTime);
        }
        // the mob will move left and right
        // if it hits a wall, it will turn around
        // if it hits the player, it will kill the player
        // on default go to the right
        UpdateAnimation();
    }

    void UpdateAnimation()
    {
        state = MovementState.Idle;
        if (facingDirection == FacingDirection.Right)
        {
            sprite.flipX = true;
            state = MovementState.Running;
        }
        else
        {
            sprite.flipX = false;
            state = MovementState.Running;
        }

        anim.SetInteger("state", (int)state);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // invert the facing direction
            if (facingDirection == FacingDirection.Right)
            {
                facingDirection = FacingDirection.Left;
            }
            else
            {
                facingDirection = FacingDirection.Right;
            }
    }
}
