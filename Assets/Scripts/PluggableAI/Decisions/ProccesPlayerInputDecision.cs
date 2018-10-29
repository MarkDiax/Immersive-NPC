using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/ProccesPlayerInput")]
public class ProccesPlayerInputDecision : Decision {

    public override bool Decide(StateController controller) {
        if(controller.Response.Length > 0) {
            controller.Response = "";
            return true;
        } else {
            return false;
        }
    }
}
