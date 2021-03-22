using UnityEngine;
using UnityEngine.UI;

public class BattleTargetButton : MonoBehaviour
{
    // Config Parameters
    [HideInInspector] public string moveName = null;
    [HideInInspector] public int activeBattlerTarget = 0;
    [SerializeField] public Text targetName = null;

    public void PressButton()
    {
        if (BattleManager.instance.isCastingSpell)
        {
            BattleManager.instance.activeBattlers[BattleManager.instance.currentTurn].currentMP -= BattleManager.instance.currentSpellCost;
        }

        BattleManager.instance.PlayerAttack(moveName, activeBattlerTarget);
    }

    public void BackButton()
    {
        BattleManager.instance.targetMenu.SetActive(false);
    }
}
