using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateController : MonoBehaviour {

	public State currentState;
	public EnemyStats enemyStats;
	public Transform eyes;
	public State remainState;
    public State OriginalState;

	[HideInInspector] public NavMeshAgent navMeshAgent;
	[HideInInspector] public TankShooting tankShooting;
    [HideInInspector] public TankHealth tankHealth;
	[HideInInspector] public List<Transform> wayPointList;
	[HideInInspector] public int nextWayPoint;
	[HideInInspector] public Transform chaseTarget;
	[HideInInspector] public float stateTimeElapsed;

	private bool aiActive;


	void Awake () 
	{
		tankShooting = GetComponent<TankShooting> ();
		navMeshAgent = GetComponent<NavMeshAgent> ();
        tankHealth = GetComponent<TankHealth>();

    }

	public void SetupAI(bool aiActivationFromTankManager, List<Transform> wayPointsFromTankManager, Transform enemyTank)
	{
		wayPointList = wayPointsFromTankManager;
		aiActive = aiActivationFromTankManager;
		if (aiActive) 
		{
			navMeshAgent.enabled = true;
		} else 
		{
			navMeshAgent.enabled = false;
		}

        chaseTarget = enemyTank;
	}

	public void TransitionToState(State nextState)
	{
		if (nextState == remainState) return;
		currentState = nextState;
		OnExitState();
	}

	public bool CheckIfCountDownElapsed(float duration)
	{
		stateTimeElapsed += Time.deltaTime;
		return stateTimeElapsed >= duration;
	}

	void Update()
	{
		if (!aiActive) return;

		currentState.UpdateState(this);
	}

	void OnExitState()
	{
		stateTimeElapsed = 0;
	}

	void OnDrawGizmos()
	{
		if (currentState != null && eyes != null)
		{
			Gizmos.color = currentState.sceneGizmoColor;
			Gizmos.DrawWireSphere(eyes.position, enemyStats.lookSphereCastRadius);
		}
	}

    public void ResetState()
    {
        currentState = OriginalState;
    }
}