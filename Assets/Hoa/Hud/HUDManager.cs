using PurrNet;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    [SerializeField] private Slider healthSlider, hungerSlider;

    private void Awake()
    {
        InstanceHandler.RegisterInstance(this);
    }

    private void ODestroy()
    {
        InstanceHandler.UnregisterInstance<HUDManager>();
    }

    public void SetMaxHealth(float maxHealth)
    {
        healthSlider.maxValue = maxHealth;
    }

    public void SetMaxHunger(float maxHunger)
    {
        hungerSlider.maxValue = maxHunger;
    }

    public void SetHealth(float health)
    {
        healthSlider.value = health;
    }

    public void SetHunger(float hunger)
    {
        hungerSlider.value = hunger;
    }
}
