using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateBase : MonoBehaviour {

   // [HideInInspector]
    protected Animator Animator;
    protected AIBaseOLD AIBase;

    public string AnimationTrigger;
    public string AnimationName;

    [SerializeField]
    private StateAIOld _state;
    [SerializeField]
    private bool _useAnimationTime = false;
    public float AnimationTime;

	// Use this for initialization
	void Start () {
        Animator = GetComponent<Animator>();
        AIBase = GetComponent<AIBaseOLD>();

        if(_useAnimationTime) {
            RuntimeAnimatorController ac = Animator.runtimeAnimatorController;
            for(int i = 0; i < ac.animationClips.Length; i++) {
                if(ac.animationClips[i].name == AnimationName) {
                    Debug.Log("anim time " + ac.animationClips[i].length ); 
                }
            }
            //Debug.Log("Anim time : "+ Animator.tim(AnimationName) + " of " + AnimationName  );
        }
	}


    public virtual void InitialzieState() {

    }

    public virtual void StartState() {
        
    }

    public virtual void StopState() {

    }

    public virtual void UpdateState() {
      
      
    }

    public virtual void DetemineStateTime() {
        AIBase.currentTimer = 0;
        AIBase.NewStateTime = Random.Range(4.0f, 5.0f);
    }


    #region Getters & Setters
    public StateAIOld GetState() {
        return _state;
    }
    #endregion

}
