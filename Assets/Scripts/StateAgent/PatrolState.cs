using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : State
{

    public PatrolState(StateAgent owner, string name) : base(owner, name)
    {
        
    }

    public override void OnEnter()
    {
        owner.timer.value = Random.Range(5, 10);   

        owner.pathFollower.targetNode = owner.pathFollower.pathNodes.GetNearestNode(owner.transform.position);
        owner.movement.Resume();
    }

    public override void OnExit()
    {
        owner.movement.Resume();
    }

    public override void OnUpdate()
    {
        owner.pathFollower.Move(owner.movement);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            owner.stateMachine.SetState(owner.stateMachine.StateFromName("patrol"));
        }
    }
}
