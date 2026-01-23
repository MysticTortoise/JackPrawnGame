using System;
using UnityEngine;

[Serializable]
public class JPCharacterAttack
{
    public int Damage;
    public JPCharacterDamageState DamageSeverity;
    public float EffectStrength = 1;
    public GameObject AttackEffect;
    public float ImpactTime;
    public AudioClip HitSound;
}