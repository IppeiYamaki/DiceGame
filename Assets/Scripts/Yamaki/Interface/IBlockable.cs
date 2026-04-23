using UnityEngine;

/// Blockを付与する際に呼び出されるメソッドを定義するインターフェース
public interface IBlockable {
    void AddBlock(int baseValue);
}