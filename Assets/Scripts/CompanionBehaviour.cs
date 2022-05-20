using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionBehaviour : MonoBehaviour
{
    private PlayerBehaviour playerRef;
    private Vector3 targetPosition;
    private Transform playerLookDirection;

    public float heightPosition = 4f;
    public float stopDistance = 2;

    [FMODUnity.EventRef]
    public FMOD.Studio.EventInstance floatingSoundEvent;
    public string velocityParam = "";
    public float velocitySoundMultiplier;

    void Start()
    {
        floatingSoundEvent = FMODUnity.RuntimeManager.CreateInstance(AudioManager.Instance.CompanionFloating);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(floatingSoundEvent, transform, GetComponent<Rigidbody>());
        floatingSoundEvent.start();
    }


    void FixedUpdate()
    {
        if (playerRef != null && Vector3.Distance(transform.position, playerRef.transform.position) > stopDistance)
        {
            Vector3 endPos = new Vector3(playerRef.transform.position.x - stopDistance, heightPosition, playerRef.transform.position.z);
            Vector3 newPos = Vector3.Lerp(transform.position, endPos, 0.05f);
            floatingSoundEvent.setParameterByName(velocityParam, Vector3.Distance(transform.position, newPos) * velocitySoundMultiplier);
            transform.position = newPos;
            transform.LookAt(playerLookDirection);
        }
    }

    public void SetPlayerRef(PlayerBehaviour player)
    {
        playerRef = player;
        playerLookDirection = player.GetComponentInChildren<DetectFloorTiles>().transform;
    }
}
