using UnityEngine;

[CreateAssetMenu(fileName = "Buff_", menuName = "Scriptable Objects/Status/Buff")]
public class BuffId : ScriptableObject {

    [SerializeField]
    [Header("バフの表示名")]
    private string displayName;

    [SerializeField]
    [Header("バフの能力の説明")]
    [TextArea] private string description;

    [SerializeField]
    [Header("アイコン")]
    private Sprite icon;
}