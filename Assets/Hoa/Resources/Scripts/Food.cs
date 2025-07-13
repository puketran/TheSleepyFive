using UnityEngine;

public class Food : Item
{
    [SerializeField] private int hungerRestored = 30;
    public override void ConsumeItem()
    {
        base.ConsumeItem();
        if (PlayerHealth.LocalPlayerHealth)
            PlayerHealth.LocalPlayerHealth.AddHunger(hungerRestored);
    }

    public override bool CanConsumeItem()
    {
        return true;
    }
}
