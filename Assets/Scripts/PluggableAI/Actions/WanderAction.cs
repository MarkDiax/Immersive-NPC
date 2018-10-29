using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Wander")]
public class WanderAction : Action {

    public override void Act(StateController controller) {
        if(controller.navMeshAgent.remainingDistance < 0.01f) {
            controller.navMeshAgent.SetDestination(GetRandomPoint(controller));
        }
    }

    public override void StartAction(StateController contoller) {
        PlayAnimation(contoller);

        contoller.navMeshAgent.SetDestination(GetRandomPoint(contoller));
    }

    public override void StopAction(StateController controller) {
      
    }

    private Vector3 GetRandomPoint(StateController contoller) {
        Vector3 postion = UnityEngine.Random.insideUnitSphere * 5;
        postion.y = 0;
        //GameObject nothing = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //nothing.transform.localScale = Vector3.one * 0.2f;
        //nothing.transform.position = postion;
        //nothing.GetComponent<Renderer>().material.color = Color.black;
        //Destroy(nothing, 3.0f);
        return postion;
    }

}
