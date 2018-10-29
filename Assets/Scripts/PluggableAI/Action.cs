using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Action : ScriptableObject {

    protected string AnimatorTirgger;
    public SpotType SpotType;

    public abstract void Act(StateController controller);
    public abstract void StartAction(StateController contoller);
    public abstract void StopAction(StateController controller);

    public void PlayAnimation(StateController contoller) {
        contoller.Animator.SetTrigger(contoller.currentState.animationTriggerName);
    }

}