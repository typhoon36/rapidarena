using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GunType { AssaultRifle = 0, Snifer,Shotgun ,Handgun, CombatKnife, Grenade }
public enum ThrowType { Default, Smoke }

[System.Serializable]
public class WeaponSetting
{
    public GunType name; // 총의 이름
    public ThrowType Throwname; // 수류탄 이름
    public int m_Damage; // 총의 데미지
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
        Damaged, // 새로운 상태 추가
    }
}
