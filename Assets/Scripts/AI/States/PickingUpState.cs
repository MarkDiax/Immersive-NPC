using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickingUpState : StateBase {

    public override void InitialzieState() {
        base.InitialzieState();
    }

    public override void StartState() {

        Animator.SetTrigger(AnimationTrigger);
        //base.StartState();
    }

    public override void StopState() {
        base.StopState();
    }

    public override void UpdateState() {
        if(Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >1 && !Animator.IsInTransition(0)) {
            //AIBase.SetState(AIBase.GetRandomState(AIBase.CurrentState));
        }
       
    }


}
