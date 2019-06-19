using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "PluggableAI/Actions/RandomPatrol")]
public class RandomPatrolAction : Action
{
    public override void Act(StateController controller)
    {
        Patrol(controller);
    }

    void Patrol(StateController controller)
    {
        //controller.navMeshAgent.destination = controller.wayPointList[controller.nextWayPoint].position;
        controller.navMeshAgent.isStopped = false;

        if (controller.navMeshAgent.remainingDistance <= controller.navMeshAgent.stoppingDistance && !controller.navMeshAgent.pathPending)
            controller.navMeshAgent.destination = GetRandomPointInArea(controller);
    }

    private Vector3 GetRandomPointInArea(StateController controller)
    {
        Vector3 point = new Vector3(Random.Range(-45.0f, 45.0f), 0.0f, Random.Range(-45.0f, 45.0f));
        NavMeshHit hit;
        if (NavMesh.SamplePosition(point, out hit, 30.0f, controller.navMeshAgent.areaMask))
            point = hit.position;
        else
            Debug.Log("No rand pos found");

        return point;
    }
}