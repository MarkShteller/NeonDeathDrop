using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBomb : Enemy
{
    public float explotionRadius;
    public float holeTimeToRegen;
    public SphereCollider sphereCollider;


    internal override void StunnedAction()
    {
        StartCoroutine(Explode(explotionRadius));
    }

    public IEnumerator Explode(float radius)
    {
        yield return new WaitForSeconds(stunnedTimer);
        sphereCollider.gameObject.SetActive(true);
        float ogRadius = sphereCollider.radius;
        while (sphereCollider.radius < radius)
        {
            sphereCollider.radius += Time.deltaTime * 0.2f; ///
            yield return null;
        }

        sphereCollider.radius = ogRadius;
        Die();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FloorCube"))
        {
            GridNode node = LevelGenerator.Instance.GetGridNode(other.transform.parent.name);
            string name = other.transform.parent.name;
            string[] posArr = name.Split(',');

            Point pointOther = new Point(int.Parse(posArr[0]), int.Parse(posArr[1]));
            if (pointOther.x == pointPos.x && pointOther.y == pointPos.y)
            {
                return;
            }
            other.GetComponent<BaseTileBehaviour>().Drop();
            LevelGenerator.Instance.SetGridNodeType(node, TileType.Pit, holeTimeToRegen);
        }
    }

    internal override void DyingAction()
    {
        base.DyingAction();
        gameObject.SetActive(false);
    }
}
