using System.Collections;
using UnityEngine;

public class LevelCamera : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Camera _camera;

    private Coroutine _followCoroutine;

    private Transform _target;

    public void Init(Transform target)
    {
        _target = target;
    }

    public void Activate()
    {
        _followCoroutine = StartCoroutine(Follow());
    }

    public void Deactivate()
    {
        StopAllCoroutines();
    }

    private IEnumerator Follow()
    {
        while (true)
        {
            var cameraPos = transform.position;
            cameraPos.z = _target.position.z;
            transform.position = cameraPos;

            yield return null;
        }
    }
}