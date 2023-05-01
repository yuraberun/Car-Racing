using System.Collections;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public int Time => Mathf.FloorToInt(_fTime);

    private float _fTime;

    private Coroutine _calculateTimeCoroutine;

    public void Init()
    {
        _fTime = 0f;
    }

    public void Start()
    {
        Stop();

        _calculateTimeCoroutine = StartCoroutine(CalculateTime());
    }

    public void Stop()
    {
        if (_calculateTimeCoroutine != null)
            StopCoroutine(_calculateTimeCoroutine);
    }

    public void Reset()
    {
        _fTime = 0f;
    }

    private IEnumerator CalculateTime()
    {
        while (true)
        {
            _fTime += UnityEngine.Time.deltaTime;

            yield return null;
        }
    }
}
