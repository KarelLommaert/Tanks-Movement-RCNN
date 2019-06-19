using UnityEngine;

public abstract class StateMachineDecision : ScriptableObject
{

	public abstract bool Decide(StateController controller);

}