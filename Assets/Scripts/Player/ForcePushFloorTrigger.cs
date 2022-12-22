using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForcePushFloorTrigger : MonoBehaviour
{
    public float speed;
    public SphereCollider sCollider;

    public enum PulseType { Light, Heavy, Special }
    private PulseType currentPulseType;

    private void Awake()
    {
        sCollider.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "FloorCube")
        {
            BaseTileBehaviour tile = other.transform.GetComponent<BaseTileBehaviour>();
            if (tile != null)
            {
                switch (currentPulseType)
                {
                    case PulseType.Light:
                        tile.SmallPulse();
                        break;
                    case PulseType.Heavy:
                        tile.Pulse();
                        break;
                    case PulseType.Special:
                        break;
                }
            }
            else
                Debug.LogError("Could not animate on GateCube");
        }
    }

    public IEnumerator PlayEffectCoroutine(float time, float width, PulseType pulseType)
    {
        //Vector3 ogPos = transform.position;
        currentPulseType = pulseType;
        sCollider.enabled = true;
        Vector3 ogScale = sCollider.transform.localScale;
        sCollider.transform.localScale = new Vector3(ogScale.x + width * (pulseType == PulseType.Heavy ? 3 : 1), ogScale.y, ogScale.z);
        while (time > 0)
        {
            time -= Time.deltaTime;
            transform.Translate(Vector3.forward * -speed * Time.deltaTime);
            yield return null;
        }
        sCollider.transform.localScale = ogScale;
        transform.localPosition = Vector3.zero;
        sCollider.enabled = false;
    }
}
