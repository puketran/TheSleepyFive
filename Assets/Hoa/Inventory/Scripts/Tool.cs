using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool : Item
{
    [SerializeField] private Animator _animator;
    [SerializeField] private float hitRadius = 0.5f;
    [SerializeField] private List<Vector3> hitPoints = new();
    [SerializeField] private float waitBeforeFirstHit = 0.1f;
    [SerializeField] private float waitBetweenHits = 0.2f;
    [SerializeField] private float damage = 5f;
    [SerializeField] private float _useCooldown = 1f;
    private float _lastUseTime;

    private WaitForSeconds _waitBeforeFirstHit, _waitBetweenHits;

    void Awake()
    {
        _waitBeforeFirstHit = new WaitForSeconds(waitBeforeFirstHit);
        _waitBetweenHits = new WaitForSeconds(waitBetweenHits);
    }
    public override void UseItem()
    {
        base.UseItem();

        if (_lastUseTime + _useCooldown > Time.time)
            return;

        Debug.Log($"Using tool: {ItemName}");
        StartCoroutine(HandleHit());
    }

    private Collider[] _coliders = new Collider[15];

    private IEnumerator HandleHit()
    {
        _lastUseTime = Time.time;
        var alreadyHit = new List<IDamageable>();
        _animator?.SetTrigger("Hit");
        yield return _waitBeforeFirstHit;

        foreach (var point in hitPoints)
        {
            var hitPosition = transform.TransformPoint(point);
            var hits = Physics.OverlapSphereNonAlloc(hitPosition, hitRadius, _coliders);
            for (int i = 0; i < hits; i++)
            {
                var hit = _coliders[i];
                if (!hit.TryGetComponent(out IDamageable damageable))
                    continue;

                if (alreadyHit.Contains(damageable))
                    continue;

                damageable.TakeDamage(damage); // Assuming a fixed damage value of 1 for simplicity
                alreadyHit.Add(damageable);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        foreach (var point in hitPoints)
        {
            Gizmos.DrawWireSphere(transform.TransformPoint(point), hitRadius);
        }
    }
}
