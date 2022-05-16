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

    internal void Init(BoidsController boidsController)
    {
        _boidsController = boidsController;
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
}
