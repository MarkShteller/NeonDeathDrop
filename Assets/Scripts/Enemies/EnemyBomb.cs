using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class EnemyBomb : Enemy
{
    public float explotionRadius;
    public float holeTimeToRegen;
    public SphereCollider sphereCollider;
    public VisualEffect chargeEffect;
    public VisualEffect explosionEffect;

    private Coroutine explotionCoroutine = null;

    internal override void StunnedAction()
    {
        if(explotionCoroutine == null)
            explotionCoroutine = StartCoroutine(Explode(explotionRadius));
    }

    public IEnumerator Explode(float radius)
    {
        print("bomb explode action");
        FMODUnity.RuntimeManager.PlayOneShot(AudioManager.Instance.EnemyBombTriggered, transform.position);

        animator.SetTrigger("Exploding");
        chargeEffect.gameObject.SetActive(true);
        chargeEffect.Play();

        yield return new WaitForSeconds(stunnedTimer);

        sphereCollider.gameObject.SetActive(true);
        float ogRadius = sphereCollider.radius;
        while (sphereCollider.radius < radius)
        {
            sphereCollider.radius += Time.deltaTime * 4;//0.2f; ///
            yield return null;
        }

        chargeEffect.gameObject.SetActive(false);
        explosionEffect.gameObject.SetActive(true);
        explosionEffect.Play();

        yield return new WaitForSeconds(0.4f);

        sphereCollider.radius = ogRadius;
        Die(DeathType.EnemyPit);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FloorCube"))
        {
            GridNode node = LevelGenerator.Instance.GetGridNode(other.transform.parent.name);
            string name = other.transform.parent.name;
            string[] posArr = name.Split(',');

            Point pointOther = new Point(int.Parse(posArr[0]), int.Parse(posArr[1]));
            /*if (pointOther.x == pointPos.x && pointOther.y == pointPos.y)
            {
                return;
            }*/
            other.GetComponent<BaseTileBehaviour>().Drop();
            LevelGenerator.Instance.SetGridNodeType(node, TileType.EnemyPit, holeTimeToRegen);
        }
    }

    internal override void DyingAction()
    {
        base.DyingAction();
        //gameObject.SetActive(false);
    }
}
