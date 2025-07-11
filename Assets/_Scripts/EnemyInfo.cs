using UnityEngine;

[CreateAssetMenu(menuName = "New Enemy")]
public class EnemyInfo : ScriptableObject
{
    public string EnemyName;
    public int BaseHealth;
    public int BaseStr;
    public int BaseInitiative;
    public GameObject EnemyBattleVisualPrefab;     // use in Battle scene
    //public GameObject EnemyOverworldVisualPrefab;  // use in Overworld scene
    
}
