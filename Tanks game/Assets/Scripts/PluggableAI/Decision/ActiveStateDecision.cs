using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/ActiveState")]
public class ActiveStateDecision : StateMachineDecision
{

    public override bool Decide(StateController controller)
    {
		bool chaseTargetIsActive = controller.chaseTarget.gameObject.activeSelf;
		return chaseTargetIsActive;
    }

}