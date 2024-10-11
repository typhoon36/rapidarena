using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GunType { AssaultRifle = 0, Snifer,Shotgun ,Handgun, CombatKnife, Grenade }
public enum ThrowType { Default, Smoke }

[System.Serializable]
public class WeaponSetting
{
    public GunType name; // ���� �̸�
    public ThrowType Throwname; // ����ź �̸�
    public int m_Damage; // ���� ������
    public float m_AttackRate;
    public float m_AttackDist;
    public bool IsAutoAttack;
    public int m_ClipSize;
    public int m_MaxAmmo;
}

public class State : MonoBehaviour
{
    public enum DefState
    {
        Idle,
        Jump,
        Walk,
        Run,
        Fire,
        Reload,
        Inspect,
        Damaged, // ���ο� ���� �߰�
    }
}
