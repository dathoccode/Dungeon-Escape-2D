using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSkin", menuName = "Scriptable Objects/PlayerSkin")]
public class PlayerSkin : ScriptableObject
{
    public string skinName;
    public Sprite sprite;
    public AnimatorOverrideController controller;
}
