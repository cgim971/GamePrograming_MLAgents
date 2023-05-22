using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class PenguinAgent : Agent {

    public float MoveSpeed = 5f;
    public float TurnSpeed = 180f;
    public GameObject HeartPrefab;
    public GameObject RegurgitatedFishPrefab;

    private PenguinArea _penguinArea;
    private Rigidbody _rigidbody;
    private GameObject _baby;
    private bool _isFull;

    public override void Initialize() {
        _penguinArea = GetComponentInParent<PenguinArea>();
        _baby = _penguinArea.PenguinBaby;
        _rigidbody = GetComponent<Rigidbody>();
    }

    public override void OnActionReceived(ActionBuffers actions) {
        float forwardAmount = actions.DiscreteActions[0];
        float turnAmount = 00f;

        if (actions.DiscreteActions[1] == 1f) {
            turnAmount = -1f;
        }
        else if (actions.DiscreteActions[1] == 2f) {
            turnAmount = 1f;
        }

        _rigidbody.MovePosition(transform.position + transform.forward * forwardAmount * MoveSpeed * Time.fixedDeltaTime);
        transform.Rotate(transform.up * turnAmount * TurnSpeed * Time.fixedDeltaTime);

        if (MaxStep > 0)
            AddReward(-1f / MaxStep);
    }

    public override void Heuristic(in ActionBuffers actionsOut) {
        int forwardAction = 0;
        int turnAction = 0;

        if (Input.GetKey(KeyCode.W)) {
            forwardAction = 1;
        }
        if (Input.GetKey(KeyCode.A)) {
            turnAction = 1;
        }
        else if (Input.GetKey(KeyCode.D)) {
            turnAction = 2;
        }

        actionsOut.DiscreteActions.Array[0] = forwardAction;
        actionsOut.DiscreteActions.Array[1] = turnAction;
    }

    public override void OnEpisodeBegin() {
        _isFull = false;
        _penguinArea.ResetArea();
    }

    public override void CollectObservations(VectorSensor sensor) {
        sensor.AddObservation(_isFull);
        sensor.AddObservation(Vector3.Distance(_baby.transform.position, transform.position));
        sensor.AddObservation((_baby.transform.position - transform.position).normalized);
        sensor.AddObservation(transform.forward);
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.transform.CompareTag("Fish")) {
            EatFish(collision.gameObject);
        }
        else if (collision.transform.CompareTag("Baby")) {
            RegurgitateFish();
        }
    }

    private void EatFish(GameObject fishObject) {
        if (_isFull)
            return;
        _isFull = true;

        _penguinArea.RemoveSpecificFish(fishObject);
        AddReward(1f);
    }

    private void RegurgitateFish() {
        if (!_isFull)
            return;
        _isFull = false;

        GameObject regurgitatedFish = Instantiate<GameObject>(RegurgitatedFishPrefab);
        regurgitatedFish.transform.parent = transform.parent;
        regurgitatedFish.transform.position = _baby.transform.position;
        Destroy(regurgitatedFish, 4f);

        GameObject heart = Instantiate<GameObject>(HeartPrefab);
        heart.transform.parent = transform.parent;
        heart.transform.position = _baby.transform.position + Vector3.up;
        Destroy(heart, 4f);

        AddReward(1f);

        if (_penguinArea.FishRemaining <= 0) {
            EndEpisode();
        }
    }
}
