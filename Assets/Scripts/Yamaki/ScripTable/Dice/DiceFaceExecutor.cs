using UnityEngine;

/// <summary>
/// DiceFace の効果リストを順番に実行するユーティリティクラス。
/// DiceAction.Execute の第3引数（diceValue）には、各 ActionEntry に設定された
/// 基本値（Value）をそのまま渡します。
/// バフ等による最終値計算が必要な場合は、呼び出し元で value を加工してから
/// 別途 Execute を呼んでください。
/// </summary>
public static class DiceFaceExecutor
{
    /// <summary>
    /// 指定した DiceFace の効果リストを上から順に実行します。
    /// </summary>
    /// <param name="face">実行する DiceFace アセット</param>
    /// <param name="user">効果を使用するエンティティ（Self 対象アクションに渡す）</param>
    /// <param name="enemy">効果を受けるエンティティ（Enemy 対象アクションに渡す）</param>
    public static void Execute(DiceFace face, ICombatEntity user, ICombatEntity enemy)
    {
        if (face == null)
        {
            Debug.LogWarning("[DiceFaceExecutor] face が null です。スキップします。");
            return;
        }

        foreach (ActionEntry entry in face.Actions)
        {
            if (entry.Action == null)
            {
                Debug.LogWarning("[DiceFaceExecutor] ActionEntry の Action が null です。スキップします。");
                continue;
            }

            // 対象を DiceFaceTarget に従って切り替える
            ICombatEntity target = entry.Target == DiceFaceTarget.Self ? user : enemy;

            // entry.Value を "基本値（diceValue 引数）" としてそのまま渡す
            // ※ 最終ダメージ等の計算はこのクラスの責務外
            entry.Action.Execute(user, target, entry.Value);
        }
    }
}
