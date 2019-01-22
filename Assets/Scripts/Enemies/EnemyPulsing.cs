using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPulsing : Enemy
{
    public float pulseDistanceFromPlayer;
    public float pulseInterval;

    public GameObject pulseEffect;

    private float pulseIntervalLocal = 0;

    internal override void Init()
    {
        base.Init();
        constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
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

    public void PulseTrigger()
    {
        StartCoroutine(Pulse(1.5f));
    }

    private IEnumerator Pulse(float duration)
    {
        pulseEffect.SetActive(true);

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
