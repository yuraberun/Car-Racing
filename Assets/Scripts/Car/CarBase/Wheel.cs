using UnityEngine;

[RequireComponent(typeof(WheelCollider))]
public class Wheel : MonoBehaviour
{
    private static float _detectRoadOffset = 0.2f;

    private BoxCollider _roadDetectCollider;

    public WheelCollider WheelCollider { get; private set; }

    public bool OnRoad { get; private set; }

    public void Init()
    {
        WheelCollider = GetComponent<WheelCollider>();

        var size = WheelCollider.radius * 2f + _detectRoadOffset;

        _roadDetectCollider = gameObject.AddComponent<BoxCollider>();
        _roadDetectCollider.size = new Vector3(size, size, 0.1f);
        _roadDetectCollider.isTrigger = true;
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
