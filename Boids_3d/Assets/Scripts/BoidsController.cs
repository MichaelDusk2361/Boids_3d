using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public class BoidsController : MonoBehaviour
{
    [SerializeField] List<Boid> _boids = new();
    [SerializeField] GameObject _boidPrefab;
    [SerializeField] int _boidCount = 20;
    [SerializeField] int _spawnFieldHeight = 10;
    [SerializeField] int _spawnFieldWidth = 10;
    [SerializeField] int _spawnFieldDepth = 10;
    public Vector3 targetDirection = Vector3.left;

    public List<Boid> Boids { get => _boids; }

    private void OnValidate()
    {
        if (_boidCount < 1)
            _boidCount = 1;
        if (_spawnFieldDepth < 1)
            _spawnFieldDepth = 1;
        if (_spawnFieldHeight < 1)
            _spawnFieldHeight = 1;
        if (_spawnFieldWidth < 1)
            _spawnFieldWidth = 1;
    }

    private void Awake()
    {
        SpawnBoids();
    }

    private void SpawnBoids()
    {
        for (int i = 0; i < _boidCount; i++)
        {
            Vector3 randomPostion = new(
                Random.Range(-_spawnFieldWidth / 2f, _spawnFieldWidth / 2f),
                Random.Range(-_spawnFieldHeight / 2f, _spawnFieldHeight / 2f),
                Random.Range(-_spawnFieldDepth / 2f, _spawnFieldDepth / 2f));
            Boid newBoid = Instantiate(_boidPrefab, randomPostion + transform.position, Random.rotation).GetComponent<Boid>();
            newBoid.Init(this);
            _boids.Add(newBoid);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(_spawnFieldWidth, _spawnFieldHeight, _spawnFieldDepth));
    }
}
