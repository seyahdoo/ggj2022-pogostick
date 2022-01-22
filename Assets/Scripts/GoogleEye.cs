using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoogleEye : MonoBehaviour {
    [SerializeField] private Transform iris;
    [SerializeField] private float maxDistance;
    [SerializeField] private Vector2 gravity = Vector2.down;
    [SerializeField] private float bouncy = 0.2f;
    

    private Vector2 _velocity;
    private Vector3 _irisPos;

    private void Awake() {
        _irisPos = iris.position;
    }

    void Update() {
        _velocity += gravity * Time.deltaTime;
        var forecastPos = _irisPos + (Vector3)_velocity * Time.deltaTime;
        var deltaToCenter = forecastPos - transform.position;
        var hasCollision = deltaToCenter.magnitude > maxDistance;
        if (hasCollision) {
            _irisPos = transform.position + deltaToCenter.normalized * maxDistance;
            _velocity = Vector2.Reflect(_velocity, -deltaToCenter.normalized) * bouncy;
        }
        else {
            _irisPos += (Vector3)_velocity * Time.deltaTime;
        }
        
        iris.position = _irisPos;
    } 
}
