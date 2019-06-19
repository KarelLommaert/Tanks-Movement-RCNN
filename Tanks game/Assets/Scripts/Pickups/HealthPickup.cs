using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : BasePickup
{
    [SerializeField] private float _healAmount = 10.0f;

    protected override void Consume(GameObject recipient)
    {
        TankHealth health = recipient.GetComponent<TankHealth>();
        if (health)
            health.Heal(_healAmount);

        PositionSaverUpgrade.Instance.StoreSequence(50, SequenceType.HealthPickup);

        Destroy(gameObject);
    }
}