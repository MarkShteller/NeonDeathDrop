using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShockwaveBehavior : MonoBehaviour
{
    public SphereCollider capsuleCollider;

    private void Awake()
    {
        capsuleCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "FloorCube")
        {
            BaseTileBehaviour tile = other.transform.GetComponent<BaseTileBehaviour>();
            if (tile != null)
                tile.Pulse();
            else
                Debug.LogError("Could not animate on GateCube");
        }
        if (other.tag == "Enemy")
        {
            Enemy e = other.GetComponent<Enemy>();
            if (e != null)
                e.Die();
            else
                Debug.LogError("Could not find Enemy component on Enemy GO");
        }
    }

    public IEnumerator Shockwave(float radius)
    {
        capsuleCollider.enabled = true;

        float ogRadius = capsuleCollider.radius;
        while (capsuleCollider.radius < radius)
        {
            capsuleCollider.radius += Time.deltaTime * 15;
            yield return null;
        }

        capsuleCollider.radius = ogRadius;
        capsuleCollider.enabled = false;

        gameObject.SetActive(false);
    }

}
