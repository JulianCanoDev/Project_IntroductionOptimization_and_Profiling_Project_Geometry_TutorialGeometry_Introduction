using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Cannonball : MonoBehaviour
{
    [SerializeField]
    private AudioClip explosionClip;

    [SerializeField]
    private AudioClip screamClip;

    [SerializeField]
    private GameObject crater;

    [SerializeField]
    private int killRadius = 5;

    GameObject[] soldiers;
    List<GameObject> deadSoldiers;

    public void Explode(Vector3 pos)
    {
        deadSoldiers = new List<GameObject>();

        soldiers = GameObject.FindGameObjectsWithTag("Soldier");
        bool soldierKilled = false;
        for(int i = 0; i < soldiers.Length; i++)
        {
            if (Vector3.Distance(pos, soldiers[i].transform.position) <= killRadius)
            {
                deadSoldiers.Add(soldiers[i].gameObject);
                soldierKilled = true;
            }
        }

        if(soldierKilled)
        {
            StartCoroutine(KillEnemies(pos, 3, 0));
        }
    }

    IEnumerator KillEnemies(Vector3 pos, int batchSize, int count)
    {
        AudioSource.PlayClipAtPoint(explosionClip, pos);

        if (count < batchSize)
        {
            int increment = count;
            int lower = (int)(increment * (deadSoldiers.Count / batchSize));
            int upper = (int)((increment + 1) * (deadSoldiers.Count / batchSize));

            if(count == batchSize)
            {
                count = deadSoldiers.Count;
            }
            for (int i = lower; i < upper; i++)
            {
                deadSoldiers[i].GetComponent<SoldierController>().Explode(pos);
            }
            count++;
            yield return null;
        }


        if (count < batchSize)
        {
            AudioSource.PlayClipAtPoint(screamClip, pos);


            // Crater prefab.
            NavMeshHit closestHit;
            if (NavMesh.SamplePosition(pos, out closestHit, 500, 1))
            {
                GameObject craterInstance = Instantiate(crater, closestHit.position, crater.transform.rotation);
                craterInstance.AddComponent<DestroyAfterTimeout>();
                craterInstance.GetComponent<DestroyAfterTimeout>().timeout = 10;
            }
        }

    }
}
