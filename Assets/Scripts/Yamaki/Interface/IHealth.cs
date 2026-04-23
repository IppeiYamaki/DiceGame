using UnityEngine;

/// 体力が変動する際に呼び出されるメソッドを定義するインターフェース
public interface IHealth {
    void TakeDamage(int amount);
    void Heal(int amount);
}