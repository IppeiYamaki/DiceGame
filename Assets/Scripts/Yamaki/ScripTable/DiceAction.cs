using UnityEngine;

/// �퓬�̎Q���ҁi�v���C���[��G�Ȃǁj��\���C���^�[�t�F�[�X
public interface ICombatEntity { } // �g�퓬�̎Q���ҁh�Ƃ����^�O�i���ʌ^�j

/// �_�C�X�̌��ʂ��`���钊�ۃN���X�B�_�C�X�̌��ʂ́A���[�U�[�i�U���ҁj�ƃ^�[�Q�b�g�i��U���ҁj�ɑ΂��āA�_�C�X�̒l�Ɋ�Â��ĉ��炩�̃A�N�V���������s���邱�Ƃ��ł��܂��B
public abstract class DiceAction : PersistentScriptableObject {
    public abstract void Execute(ICombatEntity user, ICombatEntity target, int diceValue);
}