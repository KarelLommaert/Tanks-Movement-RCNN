using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasePickup : MonoBehaviour
{
    protected PickupSpawner _spawner = null;

    public virtual void Initialize(PickupSpawner spawner)
    {
        _spawner = spawner;
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tank"))
        {
            Consume(other.gameObject);
            _spawner.PickupConsumed();
        }
    }

    protected void Update()
    {
        transform.Rotate(0.0f, 0.7f + Time.deltaTime, 0.0f);
    }

    protected abstract void Consume(GameObject recipient);
}