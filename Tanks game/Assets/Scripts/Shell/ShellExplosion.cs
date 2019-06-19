using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    public LayerMask m_TankMask;
    public ParticleSystem m_ExplosionParticles;       
    public AudioSource m_ExplosionAudio;              
    public float m_MaxDamage = 100f;                  
    public float m_ExplosionForce = 1000f;            
    public float m_MaxLifeTime = 2f;                  
    public float m_ExplosionRadius = 5f;

    private TanksAgent _owningAgent = null;
    private GameObject _owner = null;

    private void Start()
    {
        Destroy(gameObject, m_MaxLifeTime);
    }

    public void Initialize(GameObject owner)
    {
        _owner = owner;
        _owningAgent = owner.GetComponent<TanksAgent>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger && !other.CompareTag("Shell"))
            return;

        // Find all the tanks in an area around the shell and damage them.
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TankMask);

		for (int i = 0; i < colliders.Length; i++) {
			Rigidbody targetRigidBody = colliders[i].GetComponent<Rigidbody>();

			if (!targetRigidBody) {
				continue;
			}

			targetRigidBody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);

			TankHealth targetHealth = targetRigidBody.GetComponent<TankHealth>();

			if (!targetHealth) {
				continue;
			}

			float damage = CalculateDamage(targetRigidBody.position);

			targetHealth.TakeDamage(damage);

            bool wasKill = targetHealth.CurrentHealth <= 0 || (targetHealth.CurrentHealth == 100 && damage > 0);

            // If we have an agent, this is the player
            if (_owningAgent)
            {
                // Hit other
                if (colliders[i].gameObject != _owner)
                {
                    if (wasKill)
                    {
                        PositionSaverUpgrade.Instance.StoreSequence(50, SequenceType.Kill);
                        PositionSaverUpgrade.Instance.StoreSequence(50, SequenceType.EnemyHit);
                    }
                    else
                        PositionSaverUpgrade.Instance.StoreSequence(50, SequenceType.EnemyHit);
                }
                // Hit self
                else
                {
                    if (wasKill)
                    {
                        PositionSaverUpgrade.Instance.StoreSequence(50, SequenceType.Death);
                        PositionSaverUpgrade.Instance.StoreSequence(50, SequenceType.SelfHit);
                    }
                    else
                        PositionSaverUpgrade.Instance.StoreSequence(50, SequenceType.SelfHit);
                }
            }
            // Else this is not the player, so check if it hit the player
            else
            {
                TanksAgent agent = colliders[i].GetComponent<TanksAgent>();
                // Hit player
                if (agent)
                {
                    if (wasKill)
                    {
                        PositionSaverUpgrade.Instance.StoreSequence(50, SequenceType.Death);
                        PositionSaverUpgrade.Instance.StoreSequence(50, SequenceType.GetHit);
                    }
                    else
                        PositionSaverUpgrade.Instance.StoreSequence(50, SequenceType.GetHit);
                }
            }

            //// Agent reward
            //TanksAgent agent = colliders[i].GetComponent<TanksAgent>();

            //// Agent survived shot
            //if (targetHealth.CurrentHealth > 0)
            //{
            //    // Hit opponent
            //    if (other.gameObject != _owner)
            //    {
            //        if (_owningAgent)
            //        {
            //            _owningAgent.AddReward(0.25f);
            //            PositionSaverUpgrade.Instance.StoreSequence(50, SequenceType.EnemyHit);
            //        }
            //        else
            //            PositionSaverUpgrade.Instance.StoreSequence(50, SequenceType.GetHit);
            //    }
            //    // Hit self
            //    else
            //    {
            //        if (_owningAgent)
            //        {
            //            _owningAgent.AddReward(-0.1f);
            //            PositionSaverUpgrade.Instance.StoreSequence(50, SequenceType.SelfHit);
            //        }
            //    }
            //}
            //// Agent got killed by shot
            //else
            //{
            //    // Hit opponent
            //    if (agent != _owningAgent)
            //    {
            //        if (_owningAgent)
            //        {
            //            _owningAgent.AddReward(0.5f);
            //            PositionSaverUpgrade.Instance.StoreSequence(50, SequenceType.EnemyHit);
            //            PositionSaverUpgrade.Instance.StoreSequence(50, SequenceType.Kill);
            //        }
            //        else
            //            PositionSaverUpgrade.Instance.StoreSequence(50, SequenceType.Death);
            //    }
            //    // Hit self
            //    else
            //    {
            //        if (_owningAgent)
            //        {
            //            _owningAgent.AddReward(-0.5f);
            //            PositionSaverUpgrade.Instance.StoreSequence(50, SequenceType.SelfHit);
            //        }
            //    }
            //}
        }

        m_ExplosionParticles.transform.parent = null;
		m_ExplosionParticles.Play();
		m_ExplosionAudio.Play();

		Destroy(m_ExplosionParticles.gameObject, m_ExplosionParticles.main.duration);
		Destroy(gameObject);
    }

    private float CalculateDamage(Vector3 targetPosition)
    {
        // Calculate the amount of damage a target should take based on it's position.
		Vector3 explosionToTarget = targetPosition - transform.position;

		float explosionDistance = explosionToTarget.magnitude;

		float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;

		float damage = relativeDistance * m_MaxDamage;

		damage = Mathf.Max(0f, damage);

		return damage;
    }
}