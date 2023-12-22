using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerShockwaveBehavior : MonoBehaviour
{
    public SphereCollider capsuleCollider;
    public PlayerBehaviour playerBehaviour;
    public VisualEffect explosionEffect;

    private bool isDeathShockwave;

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
                if (isDeathShockwave)
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
                if (isDeathShockwave)
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

    public IEnumerator Shockwave(float radius, bool regularShockwave)
    {
        print("shockwaving "+ regularShockwave);
        isDeathShockwave = regularShockwave;

        capsuleCollider.enabled = true;
        float ogRadius = capsuleCollider.radius;

        if (regularShockwave)
        {
            explosionEffect.gameObject.SetActive(true);
            explosionEffect.Play();
        }

        while (capsuleCollider.radius < radius)
        {
            capsuleCollider.radius += Time.deltaTime * 13;
            yield return null;
        }

        //EnemyManager.Instance.isUpdateEnemies = true;

        playerBehaviour.isInvinsible = false;
        if(regularShockwave)
            playerBehaviour.enableControlls = true;
        capsuleCollider.radius = ogRadius;
        capsuleCollider.enabled = false;
        isDeathShockwave = false;
        playerBehaviour.TrailsEnabled(false);
        gameObject.SetActive(false);
        
        yield return new WaitForSeconds(0.5f);
        explosionEffect.gameObject.SetActive(false);
    }

}
