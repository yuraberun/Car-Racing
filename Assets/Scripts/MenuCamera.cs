using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCamera : MonoBehaviour
{
    [SerializeField] private Vector3 _defaultPosition;
    [SerializeField] private Vector3 _garagePosition;

    [SerializeField] private float _switchTime;

    public void MoveToGaragePosition()
    {
        StartCoroutine(LerpMove(transform.position, _garagePosition));
    }

    public void MoveToDefaultPosition()
    {
        StartCoroutine(LerpMove(transform.position, _defaultPosition));
    }

    private IEnumerator LerpMove(Vector3 startPosition, Vector3 endPosition)
    {
        var elapsedTime = 0f;
        var position = startPosition;

        while (elapsedTime <= _switchTime)
        {
            position.x = Mathf.Lerp(startPosition.x, endPosition.x, elapsedTime / _switchTime);
            position.y = Mathf.Lerp(startPosition.y, endPosition.y, elapsedTime / _switchTime);
            position.z = Mathf.Lerp(startPosition.z, endPosition.z, elapsedTime / _switchTime);

            transform.position = position;

            elapsedTime += Time.deltaTime;

            yield return null;
        }
    }
}
