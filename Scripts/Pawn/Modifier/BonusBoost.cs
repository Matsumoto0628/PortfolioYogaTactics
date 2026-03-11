public class BonusBoost : IStatusModifierOnce
{
    public PawnStatus Apply(PawnStatus prev)
    {
        var enhance = new PawnStatus(prev);

        float hpMultiplier = ModifierParameter.Instance.GetModifierSO().BonusBoostHealth;
        float attackMultiplier = ModifierParameter.Instance.GetModifierSO().BonusBoostAttack;
        float speedMultiplier = ModifierParameter.Instance.GetModifierSO().BonusBoostSpeed;
        enhance.AddMaxHp((int)(prev.maxHp * hpMultiplier));
        enhance.AddAttack((int)(prev.attack * attackMultiplier));
        enhance.AddSpeed(prev.speed * speedMultiplier);

        return enhance;
    }
}