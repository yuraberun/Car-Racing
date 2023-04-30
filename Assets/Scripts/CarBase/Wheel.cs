using UnityEngine;
using System.Collections;

[RequireComponent(typeof(WheelCollider))]
public class Wheel : MonoBehaviour
{
    [SerializeField] private GameObject _visualWheel;

    private BoxCollider _roadDetectCollider;

    private Coroutine _updateVisualCoroutine;

    public WheelCollider WheelCollider { get; private set; }

    public bool OnRoad { get; private set; }

    public void Init(float blockRotationDistance)
    {
        WheelCollider = GetComponent<WheelCollider>();

        var size = WheelCollider.radius * 2f + blockRotationDistance * 2f;

        _roadDetectCollider = gameObject.AddComponent<BoxCollider>();
        _roadDetectCollider.size = new Vector3(0.1f, size, size);
        _roadDetectCollider.isTrigger = true;

        _updateVisualCoroutine = StartCoroutine(UpdateVisual());
    }

    private IEnumerator UpdateVisual()
    {
        while (true)
        {
            WheelCollider.GetWorldPose(out var position, out var rotation);
     
            _visualWheel.transform.position = position;
            _visualWheel.transform.rotation = rotation;

            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Road"))
        {
            OnRoad = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Road"))
        {
            OnRoad = false;
        }
    }
}
