using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//(menuName = "PluggableAI/Actions/Patrol")]
public class PatrolAction : Action {
    public override void Act(StateController controller) {
        Patrol(controller);
    }

    public override void StartAction(StateController contoller) {
       // throw new NotImplementedException();
    }
    public override void StopAction(StateController controller) {
        //throw new NotImplementedException();
    }

    private void Patrol(StateController controller) {
        //controller.navMeshAgent.destination = controller.wayPointList[controller.nextWayPoint].position;
        //controller.navMeshAgent.Resume();

        //if(controller.navMeshAgent.remainingDistance <= controller.navMeshAgent.stoppingDistance && !controller.navMeshAgent.pathPending) {
        //    controller.nextWayPoint = (controller.nextWayPoint + 1) % controller.wayPointList.Count;
        //}
    }
}