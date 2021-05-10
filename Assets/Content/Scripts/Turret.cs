using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Cannons
{
    public Cannon cannon;
    public float minBound;
    public float maxBound;
}
public class Turret : MonoBehaviour
{
    public ArmyCollection enemy;
    public string raycastLayer;

    [SerializeField]
    private bool shootAtPointer = false;

    [SerializeField]
    [Range(0.5f, 10f)]
    private float initialFireRate = 0.5f;

    [SerializeField]
    private Vector3 blastCentre;

    private float fireTime = 0.0f;
    public float minimumTargetDistanceFromCannon;
    public Transform LeftCorner;
    public Transform RightCorner;

    public Cannons[] cannons;
    private int cannonPointer = 0;


    public float OverallFireTime = 1f;
    public float clickShootSpeed = 0.4f;

    private void Start()
    { 
        fireTime = Time.time + initialFireRate + Random.Range(0f,2f);
    }
    private void FixedUpdate()
    {
        if (shootAtPointer && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            int layerMask = LayerMask.GetMask(raycastLayer);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                Debug.Log(hit.transform.gameObject.layer);
                Cannon c = cannons[cannonPointer%cannons.Length].cannon;

                c.fire(hit.point, clickShootSpeed);
            }
            cannonPointer++;
        }

        if (Time.time > fireTime)
        {
            //fireTime = Time.time + initialFireRate + Random.Range(5, 10f) / (1 + enemy.Count() / 3f);
            fireTime = Time.time + initialFireRate + Random.Range(5, 10f) / (1 + enemy.Count() / 3f);
            float posx = 0;
            float posz = 0;
            bool targetFound = false;

            while(!targetFound)
            {
                Transform soldier = enemy.GetRandom().transform;
                float minX, maxX, minZ, maxZ;
                minX = LeftCorner.position.x < RightCorner.position.x ? LeftCorner.position.x : RightCorner.position.x;
                maxX = LeftCorner.position.x > RightCorner.position.x ? LeftCorner.position.x : RightCorner.position.x;

                minZ = LeftCorner.position.z < RightCorner.position.z ? LeftCorner.position.z : RightCorner.position.z;
                maxZ = LeftCorner.position.z > RightCorner.position.z ? LeftCorner.position.z : RightCorner.position.z;

                bool xBound = minX < soldier.transform.position.x && maxX > soldier.transform.position.x ? true : false;
                bool zBound = minZ < soldier.transform.position.z && maxZ > soldier.transform.position.z ? true : false;

                if(xBound && zBound)
                {
                    targetFound = true;
                    posx = soldier.transform.position.x;
                    posz = soldier.transform.position.z;
                }
            }


            Vector3 target = new Vector3(posx,0,posz);

            bool fired = false;
            foreach(Cannons c in cannons)
            {
                if(target.z > c.minBound && target.z < c.maxBound && !fired)
                {
                    fired = true;
                    c.cannon.fire(target, OverallFireTime);
                }
            }
        }
    }
}