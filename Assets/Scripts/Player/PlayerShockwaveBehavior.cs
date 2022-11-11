using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerShockwaveBehavior : MonoBehaviour
{
    public SphereCollider capsuleCollider;
    public PlayerBehaviour playerBehaviour;
    public VisualEffect explosionEffect;

    private bool isRegularShockwave;

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
                if (!isRegularShockwave)
                    tile.Pulse();
                else
                    tile.SmallPulse();
            else
                Debug.LogError("Could not animate on GateCube");
        }
        if (other.tag == "Enemy")
        {
            Enemy e = other.GetComponent<Enemy>();
            if (e != null)
                if (!isRegularShockwave)
                    e.Die(Enemy.DeathType.Shockwave);
                else
                {
                    Vector3 dir = transform.position - e.transform.position;
                    // We then get the opposite (-Vector3) and normalize it
                    dir = -dir.normalized;
                    e.ForcePush(dir, playerBehaviour.currentPushForce, PlayerBehaviour.PlayerAttackType.None);
                }
            else
                Debug.LogError("Could not find Enemy component on Enemy GO");
        }
    }

    public IEnumerator Shockwave(float radius, bool regularShockwave = false)
    {
        isRegularShockwave = regularShockwave;

        capsuleCollider.enabled = true;
        float ogRadius = capsuleCollider.radius;

        explosionEffect.gameObject.SetActive(true);
        explosionEffect.Play();

        while (capsuleCollider.radius < radius)
        {
            capsuleCollider.radius += Time.deltaTime * 13;
            yield return null;
        }

        //EnemyManager.Instance.isUpdateEnemies = true;

        explosionEffect.gameObject.SetActive(true);
        playerBehaviour.isInvinsible = false;
        if(!regularShockwave)
            playerBehaviour.enableControlls = true;
        capsuleCollider.radius = ogRadius;
        capsuleCollider.enabled = false;
        isRegularShockwave = false;
        playerBehaviour.TrailsEnabled(false);
        gameObject.SetActive(false);
    }

}
