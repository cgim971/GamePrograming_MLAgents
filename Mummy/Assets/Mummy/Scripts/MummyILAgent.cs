using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class MummyILAgent : Agent {

    private Transform transform;
    private Rigidbody rigidbody;

    public float moveSpeed = 1.5f;
    public float turnSpeed = 200.0f;

    private StageManagerIL stagemanager;
    private Renderer floorRd;
    private Material originalMt;
    public Material goodMt, badMt;


    public override void Initialize() {
        MaxStep = 2000;
        transform = GetComponent<Transform>();
        rigidbody = GetComponent<Rigidbody>();

        floorRd = transform.parent.Find("Floor").GetComponent<Renderer>();
        originalMt = floorRd.material;

        stagemanager = transform.parent.GetComponent<StageManagerIL>();
    }


    public override void OnEpisodeBegin() {
        stagemanager.InitStage();

        rigidbody.velocity = rigidbody.angularVelocity = Vector3.zero;
        transform.localPosition = new Vector3(0, 0.0f, -3.5f);
        transform.localRotation = Quaternion.identity;
    }

    public override void CollectObservations(VectorSensor sensor) {

    }


    public override void OnActionReceived(ActionBuffers actions) {
        var action = actions.DiscreteActions;

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

        transform.Rotate(rot, Time.fixedDeltaTime * turnSpeed);
        rigidbody.AddForce(dir * moveSpeed, ForceMode.VelocityChange);
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
        if (collision.gameObject.name == "Floor")
            return;

        if (collision.collider.tag == stagemanager.hintColor.ToString()) {
            SetReward(+1.0f);
            EndEpisode();
            StartCoroutine(RevertMaterial(goodMt));
        }
        else {
            if (collision.collider.CompareTag("WALL") || collision.gameObject.name == "Hint") {
                SetReward(-0.05f);
            }
            else {
                SetReward(-1.0f);
                EndEpisode();
                StartCoroutine(RevertMaterial(badMt));
            }
        }
    }

    IEnumerator RevertMaterial(Material changeMt) {
        floorRd.material = changeMt;
        yield return new WaitForSeconds(0.2f);
        floorRd.material = originalMt;
    }
}
