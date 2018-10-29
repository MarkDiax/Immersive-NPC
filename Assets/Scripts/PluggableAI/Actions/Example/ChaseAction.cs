using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(menuName = "PluggableAI/Actions/Chase")]
public class ChaseAction : Action {
    public override void Act(StateController controller) {
        Chase(controller);
    }

    public override void StartAction(StateController contoller) {
        //throw new NotImplementedException();
    }
    public override void StopAction(StateController controller) {
       // throw new NotImplementedException();
    }

    private void Chase(StateController controller) {
        //controller.navMeshAgent.destination = controller.chaseTarget.position;
        //controller.navMeshAgent.Resume();
    }
}