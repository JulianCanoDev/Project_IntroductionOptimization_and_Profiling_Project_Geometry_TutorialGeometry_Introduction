using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SoldierController : MonoBehaviour
{

    public GameObject rootAnimObject;
    public Material deadMaterial;

    public ArmyCollection myTeam;
    public uint teamNumber;

    public Material teamMaterial;

    public bool alive = true;
    public bool respawn = false;
    public bool immortal = false;
    public uint destroyOrRespawnTimer = 3;

    public bool disableAnimatorOnDeath = true;

    Vector3 m_startingPosition;

    // Use this for initialization
    void Start()
    {
        m_startingPosition = transform.position;
        Respawn();
    }

    private void OnDisable()
    {
        myTeam.Remove(this);
    }

    void Respawn()
    {
        alive = true;
        myTeam.Add(this);
        UpdateTeamMaterial();

        transform.position = m_startingPosition;

        GetComponent<NavMeshAgent>().enabled = true;
        GetComponent<Animator>().enabled = true;

        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            // Avoid forces being accumulated on the rigidbodies;
            // without this fix, all forces would be applied at the same time
            rb.isKinematic = true;
        }

        // Disable animation root object to stop ragdoll animation
        if (rootAnimObject != null)
        {
            rootAnimObject.SetActive(false);
        }
    }

    void OnValidate()
    {
        UpdateTeamMaterial();
    }

    void UpdateTeamMaterial()
    {
     
        if (GetComponentInChildren<SkinnedMeshRenderer>())
        {
            GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterial = teamMaterial;
        }
        else
        {
            GetComponent<MeshRenderer>().material = teamMaterial;
        }
    }

    IEnumerator Parabola(Vector3 initialVelocity, float completionTime, int resolution)
    {
        Vector3 initialPosition = transform.position;
        Vector3 rotationAxis = Vector3.Cross(initialVelocity, Vector3.up);
        float t = 0f;
        while (t <= completionTime)
        {
            t = t + Time.deltaTime;
            Vector3 displacement = initialVelocity * t + Vector3.up * -9.81f * t * t / 2f;
            transform.position = initialPosition + displacement;
            transform.Rotate(rotationAxis, -80.0f * Time.deltaTime, Space.World);
            yield return null;
        }
    }

    public void Explode(Vector3 explosionPos)
    {
        Killed();
        float distance = (transform.position - explosionPos).magnitude;
        float intensity = 10.0f * 1.0f / (0.5f + distance);
        Vector3 initialVelocity = (transform.position - explosionPos + 10.0f * Vector3.up).normalized * 8.0f;
        StartCoroutine(Parabola(initialVelocity, destroyOrRespawnTimer, 100));
    }

    public void Killed()
    {
        if (!immortal && alive)
        {
            alive = false;
            myTeam.Remove(this);
            // Enable animation root object to start ragdoll simulation
            if (rootAnimObject != null)
            {
                rootAnimObject.SetActive(true);
            }

            GetComponent<NavMeshAgent>().enabled = false;
            if (disableAnimatorOnDeath)
                GetComponent<Animator>().enabled = false;
            else
                GetComponent<Animator>().SetTrigger("Die");

            /*
            Rigidbody mainRb = GetComponent<Rigidbody>();
            if (mainRb)
                mainRb.isKinematic = false;

            foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
            {
                rb.isKinematic = false;
            }
            */

            if (deadMaterial != null)
            {
                GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterial = deadMaterial;
            }

            if (respawn)
            {
                StartCoroutine(DelayedRespawn());
            }
            else
            {
                Destroy(gameObject, destroyOrRespawnTimer);
            }
        }
    }

    public IEnumerator DelayedRespawn()
    {
        yield return new WaitForSeconds(destroyOrRespawnTimer);
        Respawn();
    }
}
