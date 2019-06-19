using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PickupSpawner : MonoBehaviour
{
    public bool PickupAvailable { get { return _pickupIsSpawned; } }

    [SerializeField] private Transform _spawnPoint = null;
    [SerializeField] private BasePickup _pickupPrefab = null;
    [SerializeField] private float _respawnTime = 30.0f;
    [SerializeField] private bool _spawnAtStart = true;
    [SerializeField] private bool _randomLocation = false;

    private float _respawnTimer = 0.0f;
    private bool _pickupIsSpawned = false;

    private void Awake()
    {
        _respawnTimer = _respawnTime;

        if (_spawnAtStart)
        {
            SpawnPickup();
        }
    }

    private void Update()
    {
        if (!_pickupIsSpawned)
        {
            _respawnTimer -= Time.deltaTime;

            if (_respawnTimer <= 0.0f)
                SpawnPickup();
        }
    }

    private void SpawnPickup()
    {
        if (_randomLocation)
        {
            Vector3 point = new Vector3(Random.Range(-25.0f, 25.0f), 0.0f, Random.Range(-25.0f, 25.0f));
            NavMeshHit hit;
            int waterMask = 1 << NavMesh.GetAreaFromName("Walkable");
            if (NavMesh.SamplePosition(point, out hit, 30.0f, waterMask))
            {
                transform.position = point;
            }
        }

        _respawnTimer = _respawnTime;
        BasePickup pickup = Instantiate(_pickupPrefab, _spawnPoint);
        pickup.Initialize(this);
        _pickupIsSpawned = true;
    }

    public void PickupConsumed()
    {
        _pickupIsSpawned = false;
    }
}