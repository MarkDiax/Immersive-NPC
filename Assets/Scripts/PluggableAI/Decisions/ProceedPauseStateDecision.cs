using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/ProceedPauseStateDecision")]
public class ProceedPauseStateDecision : Decision {
    public override bool Decide(StateController controller) {
        controller.currentState.transitions[0].trueState = controller._pausedState;
        return true;
    }
}
