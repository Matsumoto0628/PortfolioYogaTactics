using System;
using System.Collections.Generic;
using UnityEngine;
public class PawnStatus
{
    public PawnStatus(PawnSO pawnSO)
    {
        maxHp = pawnSO.hp;
        hp = pawnSO.hp;
        speed = pawnSO.speed;
        attack = pawnSO.attack;
        attackRange = pawnSO.attackRange;
        attackDurStart = pawnSO.attackDurStart;
        attackDurEnd = pawnSO.attackDurEnd;
        hitStrength = pawnSO.hitStrength;
        maxStun = pawnSO.stun;
        stun = 0; 
    }

    public PawnStatus(PawnStatus status)
    {
        maxHp = status.maxHp;
        hp = status.hp;
        speed = status.speed;
        attack = status.attack;
        attackRange = status.attackRange;
        attackDurStart = status.attackDurStart;
        attackDurEnd = status.attackDurEnd;
        hitStrength = status.hitStrength;
        maxStun = status.maxStun;
        stun = status.stun; 
    }

    private void Set(PawnStatus status)
    {
        maxHp = status.maxHp;
        hp = status.hp;
        speed = status.speed;
        attack = status.attack;
        attackRange = status.attackRange;
        attackDurStart = status.attackDurStart;
        attackDurEnd = status.attackDurEnd;
        hitStrength = status.hitStrength;
        maxStun = status.maxStun;
        stun = status.stun;
    }

    public void TakeDamage(int amount)
    {
        hp -= Math.Max(amount, 0);
        hp = Math.Clamp(hp, 0, maxHp);
    }

    public void TakeStun(float amount)
    {
        stun += Math.Max(amount, 0);
        stun = Math.Clamp(stun, 0, maxStun);
    }

    public bool IsStun()
    {
        return stun >= maxStun;
    }
    
    public void ResetStun()
    {
        stun = 0;
    }

    public void AddMaxHp(int amount)
    {
        maxHp += Math.Max(amount, 0);
        hp += Math.Max(amount, 0);
        hp = Mathf.Clamp(hp, 0, maxHp);
    }

    public void AddAttack(int amount)
    {
        attack += Math.Max(amount, 0);
    }

    public void AddSpeed(float amount)
    {
        speed += Math.Max(amount, 0);
    }

    public void ApplyOnce()
    {
        foreach (var modifier in statusModifierOnces)
        {
            Set(modifier.Apply(this));
        }
    }

    public void AddModifier(IStatusModifierOnce modifier)
    {
        statusModifierOnces.Add(modifier);
    }

    public int hp { get; private set; }
    public float speed { get; private set; }
    public int attack { get; private set; }
    public float attackRange { get; private set; }
    public float attackDurStart { get; private set; }
    public float attackDurEnd { get; private set; }
    public float hitStrength { get; private set; }
    public float stun { get; private set; }
    public int maxHp { get; private set; }
    public float maxStun { get; private set; }

    private List<IStatusModifierOnce> statusModifierOnces = new List<IStatusModifierOnce>();
}
