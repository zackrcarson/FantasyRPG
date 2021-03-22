using UnityEngine;
using UnityEngine.UI;

public class BattleMagicButton : MonoBehaviour
{
    // Config Parameters
    [SerializeField] public Text nameText = null;
    [SerializeField] public Text costText = null;
    [HideInInspector] public string spellName = "";
    [HideInInspector] public int spellCost = 0;


    public void PressButton()
    {
        BattleCharacter battler = BattleManager.instance.activeBattlers[BattleManager.instance.currentTurn];

        if (battler.currentMP >= spellCost)
        {
            BattleManager.instance.magicMenu.SetActive(false);
            BattleManager.instance.OpenTargetMenu(spellName);

            BattleManager.instance.isCastingSpell = true;
            BattleManager.instance.currentSpellCost = spellCost;

            //battler.currentMP -= spellCost;
        }
        else
        {
            BattleManager.instance.battleNotification.notificationMessage.text = "Not enough MP!";
            BattleManager.instance.battleNotification.Activate();
        }
    }

    public void BackButton()
    {
        BattleManager.instance.magicMenu.SetActive(false);
    }
}
