using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    BoidsController _boidsController;
    [SerializeField] float _speed = 5;

    internal void Init(BoidsController boidsController)
    {
        _boidsController = boidsController;
    }

    private void Update()
    {
        transform.Translate(_speed * Time.deltaTime * Vector3.forward);
        Debug.Log(Quaternion.LookRotation(_boidsController.targetDirection));
        Debug.Log(Quaternion.FromToRotation(Vector3.forward, _boidsController.targetDirection));
        transform.rotation = Quaternion.LookRotation(_boidsController.targetDirection);
    }
}
