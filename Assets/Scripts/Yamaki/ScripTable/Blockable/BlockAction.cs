using UnityEngine;

[CreateAssetMenu(fileName = "BlockAction", menuName = "Scriptable Objects/DiceActions/Block")]
public class BlockAction : DiceAction {
    // 例：ブロックは「自分に付与」固定
    public override void Execute(ICombatEntity user, ICombatEntity target, int diceValue) {
        if (user is IBlockable blockable) {
            blockable.AddBlock(diceValue); // diceValue は “出目（基本値）”
        }
        else {
            Debug.LogWarning($"{user} is not IBlockable, cannot add block.");
        }
    }
}