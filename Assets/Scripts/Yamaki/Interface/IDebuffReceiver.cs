using UnityEngine;

public interface IDebuffReceiver {
    // baseValue ‚Í gƒ_ƒCƒX‚Ì–Úh ‚ğ‚»‚Ì‚Ü‚Ü“n‚·i‚ ‚È‚½‚Ì•ûjj
    void AddDebuff(DebuffId id, int stacks, int baseValue);
}