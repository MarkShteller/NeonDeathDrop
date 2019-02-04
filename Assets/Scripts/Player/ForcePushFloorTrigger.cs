using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForcePushFloorTrigger : MonoBehaviour
{
    public float speed;
    public SphereCollider sCollider;

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
                tile.SmallPulse();
            else
                Debug.LogError("Could not animate on GateCube");
        }
    }

    public IEnumerator PlayEffectCoroutine(float time)
    {
        //Vector3 ogPos = transform.position;
        sCollider.enabled = true;
        while (time > 0)
        {
            time -= Time.deltaTime;
            transform.Translate(Vector3.forward * -speed * Time.deltaTime);
            yield return null;
        }
        transform.localPosition = Vector3.zero;
        sCollider.enabled = false;
    }
}
