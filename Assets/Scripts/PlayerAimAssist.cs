using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimAssist : MonoBehaviour
{
    public PlayerBehaviour PlayerBehaviour;

    private List<GameObject> enemyList;

    //[HideInInspector] public GameObject closestEnemy;

    private void Awake()
    {
        enemyList = new List<GameObject>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            if (!enemyList.Contains(other.gameObject))
            {
                enemyList.Add(other.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Enemy")
        {
            if (enemyList.Contains(other.gameObject))
            {
                enemyList.Remove(other.gameObject);
            }
        }
    }

    public GameObject GetClosestEnemyObject()
    {
        float minDistance = 10;
        GameObject closestEnemy = null;
        foreach (GameObject enemy in enemyList)
        {
            float dist = Vector3.Distance(PlayerBehaviour.gameObject.transform.position, enemy.transform.position);
            if (dist < minDistance && enemy.GetComponent<Enemy>().movementStatus != Enemy.MovementType.Dead)
            {
                minDistance = dist;
                closestEnemy = enemy;
            }
        }
        enemyList.Clear();
        return closestEnemy;
    }
}
