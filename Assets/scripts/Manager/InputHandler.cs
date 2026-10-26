using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputHandler : MonoBehaviour
{
    public static InputHandler Instance { get; private set; }

    enum InputMode
    {
        PC,
        Mobile
    }

    [SerializeField] private VirtualJoystick virtualJoystick;
    [SerializeField] private Button jumpButton;
    [SerializeField] private InputMode inputMode;

    public Vector2 MoveDirection { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        jumpButton.onClick.AddListener(() =>
        {
            MoveDirection = new Vector2(MoveDirection.x, 1);
        });
    }

    void Update()
    {
        MoveDirection = new Vector2(virtualJoystick.Direction.x, 0);

        if (inputMode == InputMode.PC)
        {
            MoveDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetButtonDown("Jump") ? 1 : 0);
        }
    }
}
