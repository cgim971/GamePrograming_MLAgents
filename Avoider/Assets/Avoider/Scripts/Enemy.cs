using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    private Transform _targetTs;
    private Rigidbody _rigidbody;

    public Vector3 SpawnPos;
    public float MoveSpeed = 3f;

    public void Init(Transform targetTs) {
        _targetTs = targetTs;
        _rigidbody = GetComponent<Rigidbody>();
        transform.localPosition = SpawnPos;
    }

    public void Update() {

        if (_targetTs != null) {
            Vector3 dir = _targetTs.position - transform.position;
            dir.y = 0;
            dir.Normalize();

            _rigidbody.MovePosition(transform.position + dir * MoveSpeed * Time.deltaTime);
        }
    }
}
