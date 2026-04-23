using UnityEngine;

public interface IBuffReceiver {
    void AddBuff(BuffId id, int stacks, int baseValue);
}