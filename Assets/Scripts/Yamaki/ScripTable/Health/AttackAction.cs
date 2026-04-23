using UnityEngine;

[CreateAssetMenu(fileName = "AttackAction", menuName = "Scriptable Objects/DiceActions/Attack")]
public class AttackAction : DiceAction {
    public override void Execute(ICombatEntity user, ICombatEntity target, int diceValue) {
        if (target is IHealth hp)
            hp.TakeDamage(diceValue);
    }
}