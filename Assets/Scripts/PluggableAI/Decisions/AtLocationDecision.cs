using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "PluggableAI/Decisions/AtLocation")]
public class AtLocationDecision : Decision {

    public override bool Decide(StateController controller) {
        if(controller.navMeshAgent.remainingDistance < 0.01f) {
            return true;
        } else {
            return false;
        }
    }
}
