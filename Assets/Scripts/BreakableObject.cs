using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class BreakableObject : MonoBehaviour
{
    [SerializeField] private Transform visuals;
    [SerializeField] private VisualEffect effect;
    [SerializeField] private Transform floatTarget;

    void Start()
    {
        
    }

    public void Break(Transform player)
    {
        floatTarget.position = player.position;
        visuals.gameObject.SetActive(false);
        effect.Play();
        FMODUnity.RuntimeManager.PlayOneShot(AudioManager.Instance.BoxBreak, transform.position);
        StartCoroutine(SelfDestruct());
    }

    private IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }
}
