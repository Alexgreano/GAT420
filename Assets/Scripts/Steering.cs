using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Steering : MonoBehaviour
{
    [Range(0, 45)] public float wanderDisplacement = 5;
    [Range(0, 5)] public float wanderRadius = 3;
    [Range(0, 5)] public float wanderDistance = 1;

    float wanderAngle = 0;

   public Vector3 Seek(AutonomousAgent agent, GameObject target)
    {
        Vector3 force = CalculateSteering(agent, target.transform.position - agent.transform.position);

        return force;
    }

    public Vector3 Flee(AutonomousAgent agent, GameObject target)
    {
        Vector3 force = CalculateSteering(agent, agent.transform.position - target.transform.position);


        return force;
    }

    public Vector3 Wander(AutonomousAgent agent)
    {
        wanderAngle = wanderAngle + Random.Range(-wanderDisplacement, wanderDisplacement);
        Quaternion rotation = Quaternion.AngleAxis(wanderAngle, Vector3.up);
        Vector3 point = rotation * (Vector3.forward * wanderRadius);
        Vector3 forward = agent.transform.forward * wanderDistance;
        Vector3 force = CalculateSteering(agent, (forward + point ));

        return force;
    }


    public Vector3 Cohesion(AutonomousAgent agent, GameObject[] targets)
    {
        Vector3 centerOfTargets = Vector3.zero;
        foreach(GameObject target in targets)
        {
            centerOfTargets += target.transform.position;
        }

        centerOfTargets /= targets.Length;

        Vector3 force = CalculateSteering(agent, centerOfTargets - agent.transform.position);

        return force;
    }

    Vector3 CalculateSteering(AutonomousAgent agent, Vector3 vector)
    {
        Vector3 direction = vector.normalized;
        Vector3 desired = direction * agent.maxSpeed;
        Vector3 steer = desired - agent.velocity;
        Vector3 force = Vector3.ClampMagnitude(steer, agent.maxForce);
        return force;
    }
}
