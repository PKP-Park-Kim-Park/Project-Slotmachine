public abstract class ItemEffectBase
{
    public int amount;
}

public class SymbolEffect : ItemEffectBase
{
    public FlagSymbols symbols;
}

public class PatternEffect : ItemEffectBase
{
    public FlagPatterns patterns;
}

public class StressEffect : ItemEffectBase
{
    public StressType stressType;
}

public class UseItem
{
    public void Use(ItemDataModel itemDataModel)
    {
        switch(itemDataModel.hasRisk)
        {
            case HasRisk.Good :
                
                break;
            case HasRisk.Risk :

                break;
            case HasRisk.Both :

                break;
            default:
                break;
        }
    }

    public void CheckUseType(ItemDataModel itemDataModel)
    {
        switch (itemDataModel.itmeEffect.useType)
        {
            case UseType.Symbol:

                break;
            case UseType.SymbolReward:

                break;
            case UseType.PartternReward:

                break;
            case UseType.Stress:

                break;
            default:
                break;
        }
    }
}
