using System;
public class PawnStatus
{
    public PawnStatus(PawnSO pawnSO)
    {
        hp = pawnSO.hp;
        speed = pawnSO.speed;
        attack = pawnSO.attack;
    }

    public void TakeDamage(int amount)
    {
        hp -= Math.Max(amount, 0);
        hp = Math.Max(hp, 0);
    }

    public int hp { get; private set; }
    public float speed { get; private set; }
    public int attack { get; private set; }
}
