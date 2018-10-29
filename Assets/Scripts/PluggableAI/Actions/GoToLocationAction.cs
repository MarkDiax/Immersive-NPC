using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "PluggableAI/Actions/GoToLoaction")]
public class GoToLocationAction : Action {


    public override void Act(StateController controller) {
       
    }

    public override void StartAction(StateController contoller) {
        if(contoller.navMeshAgent.isStopped) {
            contoller.navMeshAgent.isStopped = false;
        }
        contoller.navMeshAgent.SetDestination(GetDestinaton(contoller));
    }

    public override void StopAction(StateController controller) {
        //throw new NotImplementedException();
    }

    private Vector3 GetDestinaton(StateController contoller) {
        if(contoller.CurrentSpot != null) {
            contoller.CurrentSpot.ClaimendBy = -1;
        }
        contoller.CurrentSpot = SpotManager.Instance.GetSpot(contoller.GetActionSpotType(this), contoller.NPCID);
        contoller.CurrentSpot.ClaimendBy = contoller.NPCID;
        return contoller.CurrentSpot.gameObject.transform.position;
    }


}
