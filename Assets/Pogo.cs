using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Pogo : MonoBehaviour {
    public Transform rayTransformation;
    public Transform minSpringTransformation;
    public Transform indicator;
    public float springLength;
    public float bounciness = .8f;
    public float maxPower = 5;
    public float minPower = 1;
    public float jumpWindow = 0.1f;
    public float maxTorque = 30;
    public LayerMask groundMask;
    
    private RaycastHit2D[] _results = new RaycastHit2D[1];
    private float _rayOffset = 0f;
    private Rigidbody2D _body;
    private float _lastJumpInput;
    private bool _allowBounce;

    private void Awake() {
        _body = GetComponent<Rigidbody2D>();
        _rayOffset = (rayTransformation.position - minSpringTransformation.position).magnitude;
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Space)) {
            _lastJumpInput = Time.time;
        }
    }

    private void FixedUpdate() {
        BounceDetection();
        LookAtMouse();
    }

    private void LookAtMouse() {
        var plane = new Plane(Vector3.back, Vector3.zero);
        var ray  = Camera.main.ScreenPointToRay(Input.mousePosition);
        plane.Raycast(ray, out float distance);
        var pointedPos = ray.GetPoint(distance);
        var down = (pointedPos - transform.position).normalized;
        var targetRot = Quaternion.LookRotation(transform.forward, -down);
        Rotate2(targetRot.eulerAngles.z);
    }

    private void BounceDetection() {
        if (TryGetGroundDistance(out var hit)) {
            var distanceToGround = hit.distance - _rayOffset;
            if (distanceToGround > springLength) {
                _allowBounce = true;
                return;
            }

            if (!_allowBounce) {
                return;
            }

            if (distanceToGround <= 0) {
                Bounce(hit.normal, minPower);
                indicator.position = minSpringTransformation.position;
            }
            else if(Time.time - _lastJumpInput <= jumpWindow) {
                var springCenter = springLength / 2;
                var distanceToCenter = Mathf.Abs(distanceToGround - springCenter);
                var normDistanceToCenter = distanceToCenter / springCenter;
                var power = Mathf.Lerp(minPower, maxPower, 1 - normDistanceToCenter);
                indicator.position = minSpringTransformation.position + minSpringTransformation.forward * distanceToGround;
                Bounce(hit.normal, power);
            }
        }
    }

    private void Bounce(Vector2 normal, float power) {
        Debug.Log("bounced");
        var reflection = Vector2.Reflect(_body.velocity, normal);
        reflection *= bounciness;

        Vector2 pogoForce = transform.up * power;

        _body.velocity = pogoForce + reflection;
        _allowBounce = false;
    }
    
    private bool TryGetGroundDistance(out RaycastHit2D hit) {
        var hitCount = Physics2D.RaycastNonAlloc(
            rayTransformation.position,
            rayTransformation.forward,
            _results,
            100f,
            groundMask.value);
        if (hitCount >= 1) {
            hit = _results[0];
            return true;
        }
        hit = default;
        return false;
    }

    private void Rotate2(float targetAngle) {
        var angle = JustifyAngle(_body.rotation);
        var deltaAngle = targetAngle - angle;
        if (deltaAngle > 180f) {
            deltaAngle -= 360f;
        }
        else if (deltaAngle < -180) {
            deltaAngle += 360f;
        }
        Debug.Log($"{nameof(deltaAngle)}: {deltaAngle}");
        var targetAngleVel = deltaAngle / Time.fixedDeltaTime;
        var deltaAngleVel = targetAngleVel - _body.angularVelocity;
        var torque = deltaAngleVel * _body.inertia;
        torque = Mathf.Clamp(torque, -maxTorque, +maxTorque);
        _body.AddTorque(torque, ForceMode2D.Force);
    }
    
    private void Rotate(Quaternion targetRot) {
        var deltaRot = targetRot * Quaternion.Inverse(transform.rotation);
        deltaRot.ToAngleAxis(out float angle, out Vector3 axis);
        
        if (float.IsInfinity(axis.x)) {
            axis = Vector3.zero;
        }
        
        if (angle > 180f) {
            angle -= 360f;
        }
        
        var rad = angle * Mathf.Deg2Rad; 
        var targetAngularVel = axis * rad / Time.deltaTime; 
        var deltaAngularVel = targetAngularVel - new Vector3(0, 0, _body.angularVelocity);
        var torque = Vector3.Scale(deltaAngularVel, new Vector3(0, 0, _body.inertia));
        _body.AddTorque(torque.z, ForceMode2D.Impulse);
    }

    private float JustifyAngle(float angle) {
        return (angle % 360 + 360) % 360;
    }
}
