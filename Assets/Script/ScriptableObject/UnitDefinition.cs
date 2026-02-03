using UnityEngine;

[CreateAssetMenu(menuName = "Game/UnitDefinition")]
public sealed class UnitDefinition : ScriptableObject
{
    public string DisplayName;
    public int Cost;
    [TextArea] public string Description;
    public Sprite Icon;

    public GameObject UnitPrefab;
    [Range(0, 9)] public int HotkeySlot;
}
