/// <summary>
/// ダイス効果の対象を表す列挙型 <br />
/// Self = 使用者自身, Enemy = 敵対対象
/// </summary>
public enum DiceFaceTarget
{
    // 効果を使用者自身へ適用する
    Self,

    // 効果を敵対対象へ適用する
    Enemy,
}
