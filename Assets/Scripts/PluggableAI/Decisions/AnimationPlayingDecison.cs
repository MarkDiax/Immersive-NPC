using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/AnimationPlayingDecision")]
public class AnimationPlayingDecison : Decision {
    public override bool Decide(StateController controller) {
        if(controller.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 && !controller.Animator.IsInTransition(0)) {
            return true;
        } else {
            return false;
        }
    }
}
