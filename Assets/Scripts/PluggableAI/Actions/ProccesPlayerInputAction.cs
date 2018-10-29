using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/ProccesPlayerInput")]
public class ProccesPlayerInputAction : Action {


    public override void Act(StateController controller) {
        if(Input.GetKeyDown(KeyCode.Space)) {
            controller.Response = "Here is some text";
        }
    }

    public override void StartAction(StateController contoller) {
       
    }

    public override void StopAction(StateController controller) {
        
    }
}
