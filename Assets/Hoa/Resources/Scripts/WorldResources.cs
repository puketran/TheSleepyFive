using System;
using System.Collections;
using PurrNet;
using UnityEngine;

public class WorldResources : NetworkBehaviour, IDamageable
{
    [SerializeField] private SyncVar<int> resourcesHealth = new(20);

    [SerializeField] private float popDuration = 0.2f;
    [SerializeField] private float popIntensity = 0.2f;
    [SerializeField] private AnimationCurve popCurve;
    [SerializeField] private Vector3 dropPosition;
    [SerializeField] private Item dropItem;

    private Coroutine _popCoroutine;

    void Awake()
    {
        // Initialize any necessary components or variables here
        resourcesHealth.onChanged += OnResourcesHealthChanged;
    }

    private void OnResourcesHealthChanged(int obj)
    {
        if (_popCoroutine != null)
        {
            StopCoroutine(_popCoroutine);

        }
        _popCoroutine = StartCoroutine(DoPop());
    }

    private IEnumerator DoPop()
    {
        float t = 0f;
        Vector3 startScale = transform.localScale;
        while (t < popDuration)
        {


            t += Time.deltaTime;

            float popAmount = popCurve.Evaluate(t / popDuration) * popIntensity;
            transform.localScale = startScale + Vector3.one * popAmount;

            yield return null;
        }

        transform.localScale = startScale; // Reset to original scale
    }

    public void TakeDamage(float damage)
    {
        TakeDamage_Server((int)damage);
    }

    public void Die()
    {

        Instantiate(dropItem, transform.TransformPoint(dropPosition), Quaternion.identity);
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.TransformPoint(dropPosition), 0.15f);
    }
}