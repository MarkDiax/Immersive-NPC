using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Transition {
    public Decision decision;
    public StateAI trueState;
    public StateAI falseState;
}