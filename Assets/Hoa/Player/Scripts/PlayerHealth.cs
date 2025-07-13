using PurrNet;
using PurrNet.Utils;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float maxHunger = 100f;
    [SerializeField] private float hungerDecayRate = 1f;
    [PurrReadOnly, SerializeField] private int _currentHealth;
    [PurrReadOnly, SerializeField] private float _currentHunger;

    public static PlayerHealth LocalPlayerHealth;

    private int _currentHungerInt;

    protected override void OnSpawned()
    {
        base.OnSpawned();

        enabled = isOwner;

        if (!isOwner)
            return;

        LocalPlayerHealth = this;
        _currentHealth = maxHealth;
        _currentHunger = maxHunger;


        if (!InstanceHandler.TryGetInstance(out HUDManager instance))
        {
            Debug.LogError("HUDManager instance not found!");
            return;
        }

        instance.SetMaxHealth(maxHealth);
        instance.SetMaxHunger(maxHunger);
        instance.SetHealth(_currentHealth);
        instance.SetHunger(_currentHunger);

        _currentHungerInt = Mathf.RoundToInt(maxHunger);
    }

    protected override void OnDespawned()
    {
        base.OnDespawned();

        if (!isOwner)
            return;

        LocalPlayerHealth = null;
    }

    public void AddHunger(int amount)
    {
        _currentHunger += amount;
        if (_currentHunger > maxHunger)
        {
            _currentHunger = maxHunger;
        }
        _currentHungerInt = Mathf.RoundToInt(_currentHunger);

        if (!InstanceHandler.TryGetInstance(out HUDManager hud))
        {
            Debug.LogError("HUDManager instance not found!");
            return;
        }
        hud.SetHunger(_currentHunger);
    }

    private void Update()
    {
        if (_currentHunger <= 0)
        {
            _currentHunger = 0;
            return;
        }

        _currentHunger -= hungerDecayRate * Time.deltaTime;
        if (_currentHunger < _currentHungerInt)
        {
            _currentHungerInt = Mathf.RoundToInt(_currentHunger);
            if (!InstanceHandler.TryGetInstance(out HUDManager hud))
            {
                Debug.LogError("HUDManager instance not found!");
                return;
            }
            hud.SetHunger(_currentHunger);
        }
    }
}
