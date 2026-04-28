using UnityEngine;

/// <summary>
/// マスタ <see cref="DiceDefinition"/> から <see cref="RuntimeDice"/> を生成するファクトリ
/// ラン開始時に各ダイスについてこれを呼び、ランタイム表現を作ってください
/// </summary>
public static class RuntimeDiceFactory
{
    /// <summary>
    /// マスタ <see cref="DiceDefinition"/> を Deep Copy して <see cref="RuntimeDice"/> を生成します
    /// </summary>
    /// <param name="master">コピー元のマスタ SO</param>
    /// <returns>新しい RuntimeDice。master が null の場合は null を返します</returns>
    public static RuntimeDice CreateFrom(DiceDefinition master)
    {
        if (master == null)
        {
            Debug.LogWarning("[RuntimeDiceFactory] master が null です。");
            return null;
        }

        if (string.IsNullOrEmpty(master.PersistentId))
        {
            Debug.LogWarning($"[RuntimeDiceFactory] DiceDefinition '{master.name}' に PersistentId がありません。" +
                             "セーブ後にロードできない可能性があります。");
        }

        return RuntimeDice.CreateInternal(master);
    }
}
