using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Pogo : MonoBehaviour {
    [SerializeField] private Transform rayTransformation;
    [SerializeField] private Transform springHeadTransformation;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float springLength;
    
    [Header("Tweak Parameters")]
    [SerializeField] private float maxPower = 5;
    [SerializeField] private float minPower = 1;
    [SerializeField] private float maxTorque = 30;
    [SerializeField] private float bounciness = .8f;
    [SerializeField] private float jumpWindow = 0.3f;
    [SerializeField] private float forceBounceTreshold = 0.1f;
    
    [Header("Effect")]
    [SerializeField] private ParticleSystem bounceParticleSystem;
    [SerializeField] private float bounceEffectCooldown = 0.6f;

    [Header("Animator")] 
    [SerializeField] private Animator animator;
    [SerializeField] private float springAnimSpeed = 12f;

    private Dictionary<int, SpecializedGround> _specializedGroundDic = new Dictionary<int, SpecializedGround>();
    private RaycastHit2D[] _results = new RaycastHit2D[1];
    private float _rayOffset = 0f;
    private Rigidbody2D _body;
    private float _lastJumpInput;
    private bool _allowBounce;
    private float _lastBounceEffectPlayedTime;
    private float _targetSpringBlend;

    private int _springBlend = Animator.StringToHash("springBlend");

    private void Awake() {
        _body = GetComponent<Rigidbody2D>();
        _rayOffset = (rayTransformation.position - springHeadTransformation.position).magnitude;
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) {
            _lastJumpInput = Time.time;
        }
        
        UpdateSpringAnimation(_targetSpringBlend);
    }

    private void FixedUpdate() {
        LookAtMouse();

        if (RaycastGround(out var hit)) {
            var distanceToGround = hit.distance - _rayOffset;
            _targetSpringBlend = Mathf.Clamp(1 - distanceToGround / springLength, 0, 1);
            BounceUpdate(hit);
        }
        else {
            _targetSpringBlend = 0;
        }
    }

    private void BounceUpdate(RaycastHit2D hit) {
        var distance = hit.distance - _rayOffset;
        if (distance > springLength) {
            _allowBounce = true;
            return;
        }
        
        var forceBounce = distance <= forceBounceTreshold * springLength;
        if (!forceBounce && !_allowBounce) {
            return;
        }

        if (forceBounce || distance <= 0) {
            Bounce(hit, minPower);
        }
        else if(Time.time - _lastJumpInput <= jumpWindow) {
            var timeElapsed = Time.time - _lastJumpInput;
            var timeElapsedNormalized = timeElapsed / jumpWindow;
            var power = Mathf.Lerp(minPower, maxPower, 1 - timeElapsedNormalized);
            Bounce(hit, power);
        }
    }

    private void UpdateSpringAnimation(float targetBlend) {
        var blend = animator.GetFloat(_springBlend);
        blend = Mathf.MoveTowards(blend, targetBlend, springAnimSpeed * Time.deltaTime);
        animator.SetFloat(_springBlend, blend);
    }

    private bool RaycastGround(out RaycastHit2D hit) {
        var hitCount = Physics2D.RaycastNonAlloc(
            rayTransformation.position,
            rayTransformation.up,
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
    
    private void Bounce(RaycastHit2D hit, float power) {
        var reflection = Vector2.Reflect(_body.velocity, hit.normal);
        reflection *= bounciness;
        
        var instanceId = hit.collider.GetInstanceID();
        if (!_specializedGroundDic.TryGetValue(instanceId, out SpecializedGround specializedGround)) {
            specializedGround = hit.collider.GetComponent<SpecializedGround>();
            _specializedGroundDic.Add(instanceId, specializedGround);
        }

        if (specializedGround) {
            power *= specializedGround.PowerMultiplier;
        }
        
        Vector2 pogoForce = transform.up * power;

        _body.velocity = pogoForce + reflection;
        _allowBounce = false;
        
        var collidedBody = hit.rigidbody;
        if (collidedBody) {
            var a = -_body.velocity / Time.deltaTime;
            var force = a * _body.mass;
            collidedBody.AddForceAtPosition(force, hit.point);
        }
        
        PlayBounceEffect(hit.point, hit.normal);
    }
    
    private void PlayBounceEffect(Vector2 position, Vector2 normal) {
        if (Time.time - _lastBounceEffectPlayedTime > bounceEffectCooldown) {
            _lastBounceEffectPlayedTime = Time.time;
            var tr = bounceParticleSystem.transform;
            tr.up = normal;
            tr.position = position;
            bounceParticleSystem.Play();
        }
    }

    private void LookAtMouse() {
        var plane = new Plane(Vector3.back, Vector3.zero);
        var ray  = Camera.main.ScreenPointToRay(Input.mousePosition);
        plane.Raycast(ray, out float distance);
        var pointedPos = ray.GetPoint(distance);
        var down = (pointedPos - transform.position).normalized;
        var targetRot = Quaternion.LookRotation(transform.forward, -down);
        LookAt(targetRot.eulerAngles.z);
    }

    private void LookAt(float targetAngle) {
        var angle = JustifyAngle(_body.rotation);
        var deltaAngle = targetAngle - angle;
        if (deltaAngle > 180f) {
            deltaAngle -= 360f;
        }
        else if (deltaAngle < -180) {
            deltaAngle += 360f;
        }
        var targetAngleVel = deltaAngle / Time.fixedDeltaTime;
        var deltaAngleVel = targetAngleVel - _body.angularVelocity;
        var torque = deltaAngleVel * _body.inertia;
        torque = Mathf.Clamp(torque, -maxTorque, +maxTorque);
        _body.AddTorque(torque, ForceMode2D.Force);
    }
    
    private float JustifyAngle(float angle) {
        return (angle % 360 + 360) % 360;
    }
}
