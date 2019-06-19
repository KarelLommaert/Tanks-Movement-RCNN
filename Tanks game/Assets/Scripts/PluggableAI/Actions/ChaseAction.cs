using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Chase")]
public class ChaseAction : Action
{

    public override void Act(StateController controller)
    {
		Chase(controller);
    }

	private void Chase(StateController controller)
	{
		controller.navMeshAgent.destination = controller.chaseTarget.position;
		controller.navMeshAgent.isStopped = false;

        if (controller.navMeshAgent.velocity.sqrMagnitude <= 0.1f)
        {
            Transform t = controller.navMeshAgent.transform;
            Vector3 targetDir = (controller.chaseTarget.position - controller.transform.position).normalized;
            Vector3 rotateTowards = Vector3.RotateTowards(t.forward, targetDir, 1.0f * Time.deltaTime, 0.0f);
            t.transform.rotation = Quaternion.LookRotation(rotateTowards);
        }
	}
}