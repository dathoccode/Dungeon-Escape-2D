using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanLogic : MonoBehaviour
{
    [SerializeField] private float power = 5f;
    [SerializeField] private float range = 5f;
    [SerializeField] private float width = 1.5f;
    private BoxCollider2D boxCollider;
    public enum FanDirection { Up, Down, Left, Right };
    [SerializeField] private FanDirection fanDirection;
    public bool isOn = true;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        switch (fanDirection)
        {
            case FanDirection.Up:
                boxCollider.size = new Vector2(range, width);
                boxCollider.offset = new Vector2(range / 2, 0);
                break;
            case FanDirection.Down:
                boxCollider.size = new Vector2(range, width);
                boxCollider.offset = new Vector2(-range / 2, 0);
                break;
            case FanDirection.Left:
                boxCollider.size = new Vector2(width, range);
                boxCollider.offset = new Vector2(0, -range / 2);
                break;
            case FanDirection.Right:
                boxCollider.size = new Vector2(width, range);
                boxCollider.offset = new Vector2(0, range / 2);
                break;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            switch (fanDirection)
            {
                case FanDirection.Up:
                    collision.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.up * power);
                    break;
                case FanDirection.Down:
                    collision.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.down * power);
                    break;
                case FanDirection.Left:
                    collision.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.left * power);
                    break;
                case FanDirection.Right:
                    collision.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.right * power);
                    break;
            }
        }
    }
}
