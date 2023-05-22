using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class AvoiderAgent : Agent {

    public float MoveSpeed = 5f;

    private Rigidbody _rigidbody;
    public List<Enemy> EnemeyList = new List<Enemy>();


    public override void Initialize() {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public override void OnActionReceived(ActionBuffers actions) {
        float h = Mathf.Clamp(actions.ContinuousActions[0], -1.0f, 1.0f);
        float v = Mathf.Clamp(actions.ContinuousActions[1], -1.0f, 1.0f);

        Vector3 dir = (Vector3.forward * v) + (Vector3.right * h);
        _rigidbody.MovePosition(transform.position + dir.normalized * MoveSpeed * Time.deltaTime);
    }

    public override void Heuristic(in ActionBuffers actionsOut) {
        actionsOut.ContinuousActions.Array[0] = Input.GetAxis("Horizontal");
        actionsOut.ContinuousActions.Array[1] = Input.GetAxis("Vertical");
    }

    public override void OnEpisodeBegin() {
        _rigidbody.velocity = Vector3.zero;
        transform.localPosition = new Vector3(0f, 1.05f, 0f);

        foreach (Enemy enemy in EnemeyList) {
            enemy.Init(transform);
        }
    }

    public override void CollectObservations(VectorSensor sensor) {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(_rigidbody.velocity.x);
        sensor.AddObservation(_rigidbody.velocity.z);
    }

    private void FixedUpdate() => AddReward(0.0001f);

    private void OnCollisionEnter(Collision collision) {
        if (collision.transform.CompareTag("Enemy")) {
            EndEpisode();
        }
    }
}
