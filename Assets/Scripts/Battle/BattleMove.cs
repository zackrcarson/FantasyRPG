using UnityEngine;

[System.Serializable]
public class BattleMove
{
    [SerializeField] public string moveName;
    [SerializeField] public int movePower;
    [SerializeField] public int moveCost;
    [SerializeField] public AttackEffect effect;
}
