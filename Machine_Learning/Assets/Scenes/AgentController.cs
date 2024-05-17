using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AgentController : Agent
{
    [SerializeField] private Transform target; //SerializeField to see private variables from inside Unity
    [SerializeField] private float moveSpeed = 10f;

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(Random.Range(-7f,7f), 2.5f, Random.Range(-7f, 7f));
        target.localPosition = new Vector3(Random.Range(-7f, 7f), 2.5f, Random.Range(-7f, 7f));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(target.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        Vector3 velocity = new Vector3(moveX, 0f, moveZ);
        velocity = velocity.normalized * Time.deltaTime * moveSpeed;

        transform.localPosition += velocity;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Destination")
        {
            AddReward(5f);
            EndEpisode();
        }
        if (other.gameObject.tag == "Wall")
        {
            AddReward(-2f);
            EndEpisode();
        }
        else
        {
            AddReward(-1f);
        }
    }
}