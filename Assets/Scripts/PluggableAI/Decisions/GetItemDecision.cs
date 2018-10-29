using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "PluggableAI/Decisions/GetItem")]
public class GetItemDecision : Decision {

    public override bool Decide(StateController controller) {
        if(Input.GetKeyDown(KeyCode.I)) {
            return true;
        } else {
            return false;
        }
    }
}
