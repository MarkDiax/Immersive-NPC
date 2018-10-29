using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum StateAIOld {
    Idle = 0,
    Walk = 1,
    Wave = 2,
    PickUp = 3
}

public class AIBaseOLD : Job {

    public StateAIOld CurrentState = StateAIOld.Idle;

    private Dictionary<StateAIOld, StateBase> _states = new Dictionary<StateAIOld, StateBase>();

    public float currentTimer, NewStateTime;

    public Queue<StateAI> QueueStates = new Queue<StateAI>();

	// Use this for initialization
	void Start () {
        StateBase[] states = GetComponents<StateBase>();
        for(int i = 0; i < states.Length; i++) {
            _states.Add(states[i].GetState(), GetComponents<StateBase>()[i]);
        }

        
    }
	
	// Update is called once per frame
	void Update () {


        _states[CurrentState].UpdateState();
    }
    
    //public void SetState(StateAI pState) {
    //    _states[CurrentState].StopState();
    //    _states[pState].StartState();
    //    CurrentState = pState;
        
    //}

    //public StateAI GetRandomState(StateAI pCurrentState,StateAI pRandomState = StateAI.Idle) {
    //    pRandomState = (StateAI)Random.Range(0, _states.Keys.Count);
    //    if( pRandomState != pCurrentState) {
    //        return pRandomState;
    //    } else {
    //       return GetRandomState(pCurrentState);
    //    }
    //}

}
