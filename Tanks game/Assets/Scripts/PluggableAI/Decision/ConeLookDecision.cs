using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/ConeLook")]
public class ConeLookDecision : StateMachineDecision
{
    public float coneAngle = 30.0f;

    public override bool Decide(StateController controller)
    {
        bool targetVisible = Look(controller);
        return targetVisible;
    }

    private bool Look(StateController controller)
    {
        Vector3 dir = (controller.chaseTarget.position - controller.transform.position).normalized;
        dir.y = 0.0f;
        float angle = Vector3.Angle(controller.transform.forward, dir);
        Ray ray = new Ray(controller.transform.position, dir);
        RaycastHit hit;

        if (angle <= coneAngle && Physics.Raycast(ray, out hit) && hit.collider.CompareTag("Tank"))
        {
            controller.chaseTarget = hit.transform;
            return true;
        }
        else if (controller.tankHealth.CurrentHealth < controller.tankHealth.MaxHealth)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}