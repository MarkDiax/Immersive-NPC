using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/Input")]
public class InputDecision : Decision {

    public override bool Decide(StateController controller) {
        if(Input.GetKeyDown(KeyCode.Space)) {
            return true;
        } else {
            return false;
        }
    }
}
