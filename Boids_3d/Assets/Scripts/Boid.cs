using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    BoidsController _boidsController;
    [SerializeField] float _speed = 5;
    [SerializeField] Vector3 _boidRegion;
    public Vector3 BoidRegion { get => _boidRegion; private set => _boidRegion = value; }

    internal void Init(BoidsController boidsController)
    {
        _boidsController = boidsController;
    }
    
    private void Update()
    {
        _boidRegion = _boidsController.UpdateBoidRegion(this);
        transform.Translate(_speed * Time.deltaTime * Vector3.forward);
        //transform.rotation = Quaternion.LookRotation(_boidsController.targetDirection);
        Vector3 separationTarget = Vector3.zero;
        Vector3 alignmentTarget = Vector3.zero;
        Vector3 cohesionPosition = Vector3.zero;
        foreach (var boid in _boidsController.BoidRegions[_boidRegion])
        {
            if (boid != this)
            {
                alignmentTarget += boid.transform.forward;
                cohesionPosition += boid.transform.position;
                separationTarget += (transform.position - boid.transform.position).normalized * 1 / Vector3.Distance(transform.position, boid.transform.position);
            }
        }
        alignmentTarget /= _boidsController.Boids.Count - 1;
        alignmentTarget = (alignmentTarget - Vector3.Dot(transform.forward, alignmentTarget) * transform.forward).normalized;

        //if (Vector3.Cross(alignmentTarget, transform.forward) != Vector3.zero)
        //{
        //    alignmentTarget = Vector3.LerpUnclamped(transform.forward, alignmentTarget, Mathf.Abs(90 / Vector3.Angle(transform.forward, alignmentTarget)));
        //}
        //else
        //    alignmentTarget = Vector3.zero;
        cohesionPosition /= _boidsController.Boids.Count - 1;

        Vector3 targetVector =
            ((cohesionPosition - transform.position) * _boidsController.CohesionFactor +
            separationTarget * _boidsController.SeparationFactor +
            alignmentTarget * _boidsController.AlignmentFactor);
        if (targetVector == Vector3.zero)
            targetVector = new Vector3(transform.forward.x, transform.forward.y, transform.forward.z);
        transform.rotation = Quaternion.Lerp(
                                transform.rotation,
                                Quaternion.LookRotation(targetVector),
                                Time.deltaTime * _boidsController.TurnSpeed);
    }
}
