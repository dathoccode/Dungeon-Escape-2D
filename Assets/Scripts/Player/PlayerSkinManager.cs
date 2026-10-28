using UnityEngine;

public class PlayerSkinManager : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private PlayerSkin[] skins;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            int index = Random.Range(0, skins.Length);
            EquipSkin(index);
        }
    }

    public void EquipSkin(int index)
    {
        if (index < 0 || index >= skins.Length)
            return;

        animator.runtimeAnimatorController = skins[index].controller;
        sprite.sprite = skins[index].sprite;
    }
}
