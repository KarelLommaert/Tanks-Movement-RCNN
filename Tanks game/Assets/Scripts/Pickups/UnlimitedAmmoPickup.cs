using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlimitedAmmoPickup : BasePickup
{
    [SerializeField] private float _duration = 10.0f;

    protected override void Consume(GameObject recipient)
    {
        TankShooting tankShooting = recipient.GetComponent<TankShooting>();
        if (tankShooting)
            tankShooting.SetUnlimitedAmmo(_duration);

        PositionSaverUpgrade.Instance.StoreSequence(50, SequenceType.UnlimitedAmmo);

        Destroy(gameObject);
    }
}