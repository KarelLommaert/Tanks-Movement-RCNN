using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace MLAgents
{
    [CreateAssetMenu(fileName = "NewAdvancedPlayerBrain", menuName = "ML-Agents/Advanced Player Brain")]
    public class AdvancedPlayerBrain : PlayerBrain
    {
        [System.Serializable]
        public struct TDAimDirectionPlayerAction
        {
            public Vector3 aimDirection;
            public int indexLeft;
            public int indexRight;
        }

        [SerializeField]
        [FormerlySerializedAs("TDAimingPlayerActions")]
        [Tooltip("Top down aim at mouse position actions")]
        /// Contains the mapping from input to continuous actions
        public KeyContinuousPlayerAction[] TDAimingPlayerActions;

        protected override void DecideAction()
        {
        }
    }
}