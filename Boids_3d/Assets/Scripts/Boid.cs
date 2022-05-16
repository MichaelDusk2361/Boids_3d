using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Update()
    {
        velocity = transform.forward * _maxSpeed;
        Vector3 separationTarget = Vector3.zero;
        Vector3 alignmentTarget = Vector3.zero;
        Vector3 cohesionTarget = Vector3.zero;
        int boidsInRangeCount = 0;
        foreach (var boid in _boidsController.Boids)
        {
            if (boid != this &&
                Vector3.Distance(boid.transform.position, transform.position) < _range &&
                Vector3.Angle(velocity, boid.transform.position - transform.position) < _fovDeg / 2)
            {
                boidsInRangeCount++;
                cohesionTarget += boid.transform.position;
                alignmentTarget += boid.velocity.normalized;
                separationTarget += (transform.position - boid.transform.position).normalized * 1 / Vector3.Distance(transform.position, boid.transform.position);
            }
        }
        if (boidsInRangeCount > 0)
        {
            cohesionTarget /= boidsInRangeCount;
            cohesionTarget -= transform.position;
        }

        Debug.DrawRay(transform.position, velocity, Color.white);
        Vector3 separationVelocity = Vector3.zero;
        Vector3 alignmentVelocity = Vector3.zero;
        Vector3 cohesionVelocity = Vector3.zero;
        if (separationTarget != Vector3.zero)
            separationVelocity = separationTarget.normalized * _maxSeparationVelocity;
        if (alignmentTarget != Vector3.zero)
            alignmentVelocity = alignmentTarget.normalized * _maxAlginmentVelocity;
        if (cohesionTarget != Vector3.zero)
            cohesionVelocity = cohesionTarget.normalized * _maxCohesionVelocity;

        Vector3 separationForce = Vector3.zero;
        Vector3 alignmentForce = Vector3.zero;
        Vector3 cohesionForce = Vector3.zero;
        if (separationVelocity != Vector3.zero)
            separationForce = separationVelocity - velocity;
        if (alignmentVelocity != Vector3.zero)
            alignmentForce = alignmentVelocity - velocity;
        if (cohesionVelocity != Vector3.zero)
            cohesionForce = cohesionVelocity - velocity;
        Debug.DrawRay(transform.position, separationVelocity, Color.magenta);
        Debug.DrawRay(transform.position + velocity, separationForce, Color.magenta);
        Debug.DrawRay(transform.position, alignmentVelocity, Color.cyan);
        Debug.DrawRay(transform.position + velocity, alignmentForce, Color.cyan);
        Debug.DrawRay(transform.position, cohesionVelocity, Color.yellow);
        Debug.DrawRay(transform.position + velocity, cohesionForce, Color.yellow);
        Vector3 steering = Vector3.zero;
        if (separationForce != Vector3.zero)
            steering += separationForce;
        if (cohesionForce != Vector3.zero)
            steering += cohesionForce;
        if (alignmentForce != Vector3.zero)
            steering += alignmentForce;

        steering = Vector3.ClampMagnitude(steering, _maxForce) * _mass;
        Debug.DrawRay(transform.position, steering, Color.black);
        if (steering != Vector3.zero)
            velocity = Vector3.ClampMagnitude(velocity + steering, _maxSpeed);
        transform.rotation = Quaternion.LookRotation(velocity);
        transform.position += Time.deltaTime * velocity;
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
