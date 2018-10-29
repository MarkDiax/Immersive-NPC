using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum StateCategory {
    None = -1,
    Random = 0,
    Food = 1,
}


public class StateController : MonoBehaviour {

    public int NPCID = 1;

    public StateAI currentState;
    public StateAI remainState;

    [HideInInspector]
    public NavMeshAgent navMeshAgent;
    [HideInInspector]
    public Animator Animator;

    //[HideInInspector]
    //public int nextWayPoint;
    //[HideInInspector]
    //public Transform chaseTarget;
    [HideInInspector]
    public float stateTimeElapsed;

    public string GivenText;
    public string Response;

    public StateCategory StateCategory = StateCategory.None;

    [SerializeField]
    private StateAI _answerPlayerState;
    [HideInInspector]
    public StateAI _pausedState;
    [HideInInspector]
    public Spot CurrentSpot;

    void Start() {
        navMeshAgent = GetComponent<NavMeshAgent>();
        Animator = GetComponent<Animator>();

        for(int i = 0; i < currentState.actions.Length; i++) {
            currentState.actions[i].StartAction(this);
        }
    }

    //public void SetupAI(bool aiActivationFromTankManager, List<Transform> wayPointsFromTankManager) {
    //    wayPointList = wayPointsFromTankManager;
    //    aiActive = aiActivationFromTankManager;
    //    if(aiActive) {
    //        navMeshAgent.enabled = true;
    //    } else {
    //        navMeshAgent.enabled = false;
    //    }
    //}

    void Update() {
        currentState.UpdateState(this);

        if(Input.GetKeyDown(KeyCode.Space)) {
            if(currentState.Interutable) {
                _pausedState = currentState;
                TransitionToState(_answerPlayerState);
            }
        }

    }

    void OnDrawGizmos() {
        if(currentState != null) {
            Gizmos.color = currentState.sceneGizmoColor;
            Gizmos.DrawWireSphere(transform.position, 3);
        }
    }

    public void TransitionToState(StateAI nextState) {
        if(nextState != remainState) {
            currentState = nextState;
            for(int i = 0; i < currentState.actions.Length; i++) {
                currentState.actions[i].StartAction(this);
            }
           // OnExitState();
        }
    }

    public SpotType GetActionSpotType(Action pAction) {
        for(int i = 0; i < currentState.actions.Length; i++) {
            if(currentState.actions[i] == pAction) {
                return currentState.actions[i].SpotType;
            }
        }
        return SpotType.None;

    }

 

    //public bool CheckIfCountDownElapsed(float duration) {
    //    stateTimeElapsed += Time.deltaTime;
    //    return (stateTimeElapsed >= duration);
    //}

    //private void OnExitState() {
    //    stateTimeElapsed = 0;
    //}
}