using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkState : StateBase {

    public override void InitialzieState() {
        base.InitialzieState();
    }

    public override void StartState() {
        DetemineStateTime();
        Animator.SetTrigger(AnimationTrigger);
        base.StartState();
    }

    public override void StopState() {
        base.StopState();
    }

    public override void UpdateState() {
        if(AIBase.currentTimer >= AIBase.NewStateTime) {
            //AIBase.SetState(AIBase.GetRandomState(AIBase.CurrentState));
        }
        base.UpdateState();
    }
}
