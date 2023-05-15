using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour {
    public float FishSpeed;

    private float _randomizedSpeed = 0f;
    private float _nextActionTime = -1f;
    private Vector3 _targetPosition;

    private void FixedUpdate() {
        if (FishSpeed > 0f) {
            Swim();
        }
    }

    private void Swim() {
        if (Time.fixedTime >= _nextActionTime) {
            _randomizedSpeed = FishSpeed * Random.Range(.5f, 1.5f);

            _targetPosition = PenguinArea.ChooseRandomPosition(transform.parent.position, 100f, 260f, 2f, 13f);
            transform.rotation = Quaternion.LookRotation(_targetPosition - transform.position, Vector3.up);

            float timeToGetThere = Vector3.Distance(transform.position, _targetPosition) / _randomizedSpeed;
            _nextActionTime = Time.fixedTime + timeToGetThere;
        }
        else {
            Vector3 moveVector = _randomizedSpeed * transform.forward * Time.fixedDeltaTime;
            if(moveVector.magnitude <= Vector3.Distance(transform.position, _targetPosition)) {
                transform.position += moveVector;
            }
            else {
                transform.position = _targetPosition;
                _nextActionTime = Time.fixedTime;
            }
        }
    }

}
