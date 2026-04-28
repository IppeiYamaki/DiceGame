using UnityEngine;

/// 戦闘の参加者（プレイヤーや敵など）を表すインターフェース
public interface ICombatEntity { } // “戦闘の参加者”というタグ（共通型）

/// ダイスの効果を定義する抽象クラス。ダイスの効果は、ユーザー（攻撃者）とターゲット（被攻撃者）に対して、ダイスの値に基づいて何らかのアクションを実行することができます。
public abstract class DiceAction : ScriptableObject {
    public abstract void Execute(ICombatEntity user, ICombatEntity target, int diceValue);
}