using EZCameraShake;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPulsing : Enemy
{
    public float pulseDistanceFromPlayer;
    public float pulseInterval;

    public GameObject pulseEffect;

    private float pulseIntervalLocal = 0;
    internal bool shakeScreen;

    internal override void Init()
    {
        base.Init();
        constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
        shakeScreen = false;
    }

    internal override void StaticAction()
    {
        base.StaticAction();
        float distanceFromPlayer = Vector3.Distance(transform.position, playerObject.position);
        if (distanceFromPlayer < pulseDistanceFromPlayer)
        {
            movementStatus = MovementType.Pulse;
        }
    }

    internal override void PulseAction()
    {
        pulseIntervalLocal -= Time.deltaTime;
        if(pulseIntervalLocal <= 0)
        {
            animator.SetTrigger("Stomp");
            pulseIntervalLocal = pulseInterval;
        }
    }

    internal override void DyingAction()
    {
        base.DyingAction();
        gameObject.SetActive(false);
    }

    //triggered by animation
    public void PulseTrigger()
    {
        StartCoroutine(Pulse(1.5f, shakeScreen));
    }

    private IEnumerator Pulse(float duration, bool shakeScreen = false)
    {
        pulseEffect.SetActive(true);

        if (shakeScreen)
        {
            CameraShaker.Instance.ShakeOnce(2f, 4f, 0.1f, 1f);
        }
        yield return new WaitForSeconds(duration);

        pulseEffect.SetActive(false);
        movementStatus = MovementType.Static;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            print("taken damage from pulse");
            playerObject.GetComponent<PlayerBehaviour>().TakeDamage(damage);
        }
    }
}
