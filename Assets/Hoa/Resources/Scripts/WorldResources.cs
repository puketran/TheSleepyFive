using PurrNet;
using UnityEngine;

public class WorldResources : NetworkBehaviour, IDamageable
{
    [SerializeField] private SyncVar<int> resourcesHealth = new(20);

    public void TakeDamage(float damage)
    {
        TakeDamage_Server((int)damage);
    }

    public void Die()
    {
        Destroy(gameObject);
        // Optionally, you can add logic to drop resources or notify other systems
        Debug.Log("World resource destroyed.");
    }

    [ServerRpc(requireOwnership: false)]
    private void TakeDamage_Server(int damageToTake)
    {
        resourcesHealth.value -= damageToTake;
        Debug.Log($"World resource took {damageToTake} damage. Remaining health: {resourcesHealth.value}");

        if (resourcesHealth <= 0)
        {
            Die();
        }
    }

}