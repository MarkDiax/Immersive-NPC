using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "PluggableAI/Actions/PlayAnimation")]
public class PlayAnimationAction : Action {

    public override void Act(StateController controller) {

    }

    public override void StartAction(StateController contoller) {
        PlayAnimation(contoller);
    }
    public override void StopAction(StateController controller) {
       
    }

}

