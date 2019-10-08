using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenguinWalk : MonoBehaviour
{
    private ActivePlattform _platform;
    private Vector3? _target = null;
    private Animator _animator;
    private Transform _transform;
    public float RotationSpeed = 1F;
    public float WalkingSpeed = 1F;

    // Start is called before the first frame update
    void Start()
    {
        _platform = GetComponent<ActivePlattform>();
        _animator = GetComponent<Animator>();
        _transform = GetComponent<Transform>();
        InvokeRepeating("Walk", 3f, 3f);
    }

    public void Walk()
    {
        if (_target != null) return;//already walking
        var neighbours = _platform.ActiveFloe.OrderedAdjacents;
        var target = Random.Range(0, neighbours.Length + 5);
        if (target >= neighbours.Length) return;//stay here
        //play animation
        _target = new Vector3(neighbours[target].position.x, transform.position.y, neighbours[target].position.z);
        _animator.SetLayerWeight(2, 1F);
    }

    private void Update()
    {
        if (_target != null)
        {
            //walk
            var distance = Vector3.Distance(_transform.position, _target.Value);
            if (distance <= 0.01)
            {
                _animator.SetLayerWeight(2, 0F);
                _target = null;
                return;
            }
            var targetRot = Quaternion.LookRotation(_target.Value - _transform.position, Vector3.up);
            _transform.rotation = Quaternion.Lerp(_transform.rotation, targetRot, RotationSpeed * Time.deltaTime);
            _transform.position = Vector3.Lerp(_transform.position, _target.Value, WalkingSpeed * Time.deltaTime / distance);
        }
    }
}
