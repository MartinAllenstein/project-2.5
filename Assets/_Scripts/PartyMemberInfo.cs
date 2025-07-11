using UnityEngine;

[CreateAssetMenu(menuName = "New Party Member")]
public class PartyMemberInfo : ScriptableObject
{
    public string MemberName;
    public int StartingLevel;
    public int BaseHealth;
    public int BaseStr;
    public int BaseInitiative;
    public GameObject MemberBattleVisualPrefab;     // what will be displayed in Battle scene
    public GameObject MemberOverworldVisualPrefab;  // what will be displayed in Overworld scene
}
