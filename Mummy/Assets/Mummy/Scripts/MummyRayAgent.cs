using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class MummyRayAgent : Agent {

    private Transform _transform;
    private Rigidbody _rigidbody;

    public float MoveSpeed = 1.5f;
    public float TurnSpeed = 200.0f;

    private StageManager _stageManager;

    private Renderer _floorRenderer;
    public Material GoodMaterial, BadMaterial;
    private Material _originMaterial;

    public override void Initialize() {
        MaxStep = 5000;

        _transform = GetComponent<Transform>();
        _rigidbody = GetComponent<Rigidbody>();
        _stageManager = transform.parent.GetComponent<StageManager>();
        _floorRenderer = transform.parent.Find("Floor").GetComponent<Renderer>();
        _originMaterial = _floorRenderer.material;
    }

    public override void OnEpisodeBegin() {
        _stageManager.SetStageObject();
        _rigidbody.velocity = _rigidbody.angularVelocity = Vector3.zero;
        transform.localPosition = new Vector3(Random.Range(-22.0f, 22.0f), 0.05f, Random.Range(-22.0f, 22.0f));
        transform.localRotation = Quaternion.Euler(Vector3.up * Random.Range(0, 360f));
    }

    public override void CollectObservations(VectorSensor sensor) {

    }

    public override void OnActionReceived(ActionBuffers actions) {
        var action = actions.DiscreteActions;
        Debug.Log($"[0] : {action[0]}, [1] : {action[1]}");

        Vector3 dir = Vector3.zero;
        Vector3 rot = Vector3.zero;

        switch (action[0]) {
            case 1:
                dir = transform.forward;
                break;
            case 2:
                dir = -transform.forward;
                break;
        }

        switch (action[1]) {
            case 1:
                rot = -transform.up;
                break;
            case 2:
                rot = transform.up;
                break;
        }

        transform.Rotate(rot, Time.fixedDeltaTime * TurnSpeed);
        _rigidbody.AddForce(dir * MoveSpeed, ForceMode.VelocityChange);

        AddReward(-1 / (float)MaxStep);

    }
    public override void Heuristic(in ActionBuffers actionsOut) {
        var action = actionsOut.DiscreteActions;
        actionsOut.Clear();

        if (Input.GetKey(KeyCode.W))
            action[0] = 1;
        if (Input.GetKey(KeyCode.S))
            action[0] = 2;

        if (Input.GetKey(KeyCode.A))
            action[1] = 1;
        if (Input.GetKey(KeyCode.D))
            action[1] = 2;
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.collider.CompareTag("GOOD_ITEM")) {
            _rigidbody.velocity = _rigidbody.angularVelocity = Vector3.zero;
            Destroy(collision.gameObject);
            AddReward(1.0f);

            StartCoroutine(RevertMaterial(GoodMaterial));
        }

        if (collision.collider.CompareTag("BAD_ITEM")) {
            AddReward(-1.0f);
            EndEpisode();

            StartCoroutine(RevertMaterial(BadMaterial));
        }

        if (collision.collider.CompareTag("WALL"))
            AddReward(-0.1f);
    }

    IEnumerator RevertMaterial(Material changeMaterial) {
        _floorRenderer.material = changeMaterial;
        yield return new WaitForSeconds(0.2f);
        _floorRenderer.material = _originMaterial;
    }
}
