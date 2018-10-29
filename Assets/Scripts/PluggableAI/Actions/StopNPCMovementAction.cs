using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/StopNPCMovement")]
public class StopNPCMovementAction : Action {
    public override void Act(StateController controller) {
        
    }

    public override void StartAction(StateController contoller) {
        contoller.navMeshAgent.isStopped = true;
    }

    public override void StopAction(StateController controller) {
       
    }
}
