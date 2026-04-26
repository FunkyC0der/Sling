using UnityEngine;

public class MovingSawView : HazardView
{
    [SerializeField] private Transform _pointA;
    [SerializeField] private Transform _pointB;
    [SerializeField] private float _speed = 2f;

    private Vector3 _target;

    private void Start()
    {
        _target = _pointB.position;
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, _target, _speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, _target) < 0.01f)
            _target = _target == _pointA.position ? _pointB.position : _pointA.position;
    }
}
