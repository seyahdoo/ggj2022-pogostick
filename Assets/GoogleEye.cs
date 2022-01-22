using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoogleEye : MonoBehaviour {
    [SerializeField] private Transform iris;
    [SerializeField] private float maxDistance;
    [SerializeField] private Vector2 graviy = Vector2.down;

    private Vector2 _velocity;
    private Vector3 _irisPos;

    private void Awake() {
        _irisPos = iris.position;
    }

    void Update() {
        var d = (transform.position - _irisPos).magnitude;
        if (d < maxDistance) {
            _velocity += graviy * Time.deltaTime;
        }
        else {
            _velocity = graviy;
        }

        var newPos = _irisPos + (Vector3)_velocity * Time.deltaTime;
        var deltaToCenter = newPos - transform.position;
        _irisPos = transform.position + deltaToCenter.normalized * Mathf.Min(deltaToCenter.magnitude, maxDistance);
        Debug.Log($"{nameof(_irisPos)}: {_irisPos}");
        iris.position = _irisPos;
    } 
}
