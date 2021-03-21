using UnityEngine;
using UnityEngine.UI;

public class BattleTargetButton : MonoBehaviour
{
    // Config Parameters
    public string moveName = null;
    public int activeBattlerTarget = 0;
    [SerializeField] public Text targetName = null;

    public void PressButton()
    {
        BattleManager.instance.PlayerAttack(moveName, activeBattlerTarget);
    }
}
