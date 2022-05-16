using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;

public class Boid : MonoBehaviour
{
    BoidsController _boidsController;
    [SerializeField] float _speed = 5;
    Vector3 velocity;
    [SerializeField] float _mass = 1;
    [SerializeField] float _maxSeparationVelocity = 1;
    [SerializeField] float _maxAlginmentVelocity = 1;
    [SerializeField] float _maxCohesionVelocity = 1;
    [SerializeField] float _maxForce = 1;
    [SerializeField] float _maxSpeed = 1;
    [SerializeField] float _range = 1;
    [SerializeField] float _fovDeg = 270;

    internal void Init(
        BoidsController boidsController,
        float mass,
        float maxSeparationVelocity,
        float maxAlginmentVelocity,
        float maxCohesionVelocity,
        float maxForce,
        float maxSpeed,
        float range,
        float fovDeg)
    {
        velocity = transform.forward * maxSpeed;
        _boidsController = boidsController;
        _mass = mass;
        _maxSeparationVelocity = maxSeparationVelocity;
        _maxAlginmentVelocity = maxAlginmentVelocity;
        _maxCohesionVelocity = maxCohesionVelocity;
        _maxForce = maxForce;
        _maxSpeed = maxSpeed;
        _range = range;
        _fovDeg = fovDeg;
    }
    List<Vector3> _targetVectorParts = new();
    List<Vector3> _separationVectorParts = new();
    List<Vector3> _alignmentVectorParts = new();
    List<Vector3> _cohesionVectorParts = new();
    private void Update()
    {
        _separationVectorParts.Clear();
        _alignmentVectorParts.Clear();
        _cohesionVectorParts.Clear();
        _targetVectorParts.Clear();
        transform.Translate(_speed * Time.deltaTime * Vector3.forward);
        foreach (var boid in _boidsController.Boids)
        {

            if (boid != this && Vector3.Angle(transform.forward, boid.transform.forward) < _boidsController.FovDeg / 2)
            {
                float distance = Vector3.Distance(boid.transform.position, transform.position);
                if (distance < _boidsController.CohesionRange)
                    _cohesionVectorParts.Add(boid.transform.position);
                if (distance < _boidsController.AlignmentRange)
                    _alignmentVectorParts.Add(boid.transform.forward);
                if (distance < _boidsController.SeparationRange)
                    _separationVectorParts.Add((transform.position - boid.transform.position).normalized * 1 / Vector3.Distance(transform.position, boid.transform.position));
            }
        }
        Vector3 alignmentTarget = Vector3.zero;
        _alignmentVectorParts.ForEach(v => alignmentTarget += v);
        Vector3 cohesionTarget = Vector3.zero;
        _cohesionVectorParts.ForEach(v => cohesionTarget += v);
        Vector3 separationTarget = Vector3.zero;
        _separationVectorParts.ForEach(v => separationTarget += v);

        if (alignmentTarget != Vector3.zero)
        {
            alignmentTarget = alignmentTarget.normalized;
            alignmentTarget = (alignmentTarget - Vector3.Dot(transform.forward, alignmentTarget) * transform.forward).normalized;
            _targetVectorParts.Add(alignmentTarget * _boidsController.AlignmentFactor);
        }
        if (cohesionTarget != Vector3.zero)
        {
            cohesionTarget /= _cohesionVectorParts.Count;
            cohesionTarget -= transform.position;
            _targetVectorParts.Add(cohesionTarget * _boidsController.CohesionFactor);
        }
        if(separationTarget != Vector3.zero)
        {
            _targetVectorParts.Add(separationTarget * _boidsController.SeparationFactor);
        }

        Vector3 targetVector = Vector3.zero;
        _targetVectorParts.ForEach(v => targetVector += v);
        if (targetVector == Vector3.zero)
            targetVector = transform.forward;
        transform.rotation = Quaternion.Lerp(
                                transform.rotation,
                                Quaternion.LookRotation(targetVector),
                                Time.deltaTime * _boidsController.TurnSpeed);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, _range);
    }
    //private void LateUpdate()
    //{
    //    velocity = Vector3.ClampMagnitude(velocity + steering, _maxSpeed);
    //    //Vector3 targetVector =
    //    //    (cohesionTarget - transform.position) * _boidsController.CohesionFactor +
    //    //    separationTarget * _boidsController.SeparationFactor +
    //    //    alignmentTarget * _boidsController.AlignmentFactor;

    //    //if (targetVector == Vector3.zero)
    //    //    targetVector = Vector3.forward;
    //    transform.rotation = Quaternion.LookRotation(velocity);
    //    transform.position += Time.deltaTime * velocity;
    //    //transform.rotation = Quaternion.Lerp(
    //    //                        transform.rotation,
    //    //                        Quaternion.LookRotation(velocity),
    //    //                        Time.deltaTime * _boidsController.TurnSpeed);
    //}
}
