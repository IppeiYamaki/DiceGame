using UnityEngine;

/// <summary>
/// <see cref="RuntimeDiceFace"/> の効果リストを順番に実行するユーティリティ（SO 版 <see cref="DiceFaceExecutor"/> のランタイム版）
///
/// RuntimeActionEntry は DiceAction を ID 参照で保持しているため、
/// 実行時に <see cref="DiceMasterRegistry"/> 経由で実体 SO に解決します
/// </summary>
public static class RuntimeDiceFaceExecutor
{
    /// <summary>
    /// 指定 RuntimeDiceFace の効果リストを上から順に実行します。
    /// レジストリは <see cref="DiceMasterRegistry.Active"/> を使います。
    /// </summary>
    public static void Execute(RuntimeDiceFace face, ICombatEntity user, ICombatEntity enemy)
    {
        Execute(face, user, enemy, DiceMasterRegistry.Active);
    }

    /// <summary>
    /// 指定 RuntimeDiceFace の効果リストを上から順に実行します（レジストリを明示指定）。
    /// </summary>
    /// <param name="face">実行する RuntimeDiceFace</param>
    /// <param name="user">効果を使用するエンティティ（Self 対象アクションに渡す）</param>
    /// <param name="enemy">効果を受けるエンティティ（Enemy 対象アクションに渡す）</param>
    /// <param name="registry">DiceAction の ID 解決に使うレジストリ</param>
    public static void Execute(RuntimeDiceFace face, ICombatEntity user, ICombatEntity enemy, DiceMasterRegistry registry)
    {
        if (face == null)
        {
            Debug.LogWarning("[RuntimeDiceFaceExecutor] face が null です。スキップします。");
            return;
        }

        if (registry == null)
        {
            Debug.LogWarning("[RuntimeDiceFaceExecutor] DiceMasterRegistry が設定されていません。" +
                             "DiceMasterRegistry.SetActive を事前に呼んでください。");
            return;
        }

        foreach (RuntimeActionEntry entry in face.Actions)
        {
            // ID から DiceAction 実体を解決する
            DiceAction action = registry.ResolveAction(entry.ActionId);
            if (action == null)
            {
                Debug.LogWarning($"[RuntimeDiceFaceExecutor] DiceAction の解決に失敗しました（id='{entry.ActionId}'）。スキップします。");
                continue;
            }

            // 対象を DiceFaceTarget に従って切り替える
            ICombatEntity target = entry.Target == DiceFaceTarget.Self ? user : enemy;

            // entry.Value を "基本値（diceValue 引数）" としてそのまま渡す
            // ※ バフ等込みの最終ダメージ計算はこのクラスの責務外
            action.Execute(user, target, entry.Value);
        }
    }
}
