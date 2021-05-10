using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    [SerializeField]
    private AudioClip fireClip;

    [SerializeField]
    private GameObject rocketPrefab;

    public Transform rotatePoint;
    public Transform resetTransform;

    //Overall Time to Aim-Fire-Reset is Given by Turret.cs.
    //The Following Are Percentage ratios of how this time is allocated to the different rotations.
    // Therefore these should sum to be 1 so the full Time given by turret is used.
    [Header("Fire Ratios: See Code For Explanation")]
    [Space]

    public float AimPercentage;
    public float IdleAfterFirePercentage;
    public float ResetPercentage;

    [Space]

    public float gravity;
    public float cannonTilt = 0.2f;

    private Vector3 initialVelocity;
    private float simulationTime;

    public void fire(Transform target, float delay)
    {
        fire(target.transform.position, delay);
    }

    public void fire(Vector3 target, float delay)
    {
        CalculateLaunchVelocity(rotatePoint, target, cannonTilt);
        StartCoroutine(RotateToTarget(target, rotatePoint, delay * AimPercentage));
        StartCoroutine(RotateToReset(resetTransform.transform.position, rotatePoint, delay * ResetPercentage, delay * (IdleAfterFirePercentage + AimPercentage)));
    }

    private void FireCannonballAt(Vector3 pos, Vector3 target)
    {
        GameObject rocket = Instantiate(rocketPrefab, pos, Quaternion.LookRotation(initialVelocity, Vector3.up));
        ParticleSystem rocketPart = rocket.GetComponent<ParticleSystem>();
        rocketPart.Play();
        AudioSource.PlayClipAtPoint(fireClip, pos);

        StartCoroutine(Parabola(rocket, initialVelocity, simulationTime, 100));
        StartCoroutine(Explosion(new Vector3(target.x, 0.0f, target.z), simulationTime));
        Destroy(rocket, simulationTime + 0.5f);
    }

    private void CalculateLaunchVelocity(Transform spawn, Vector3 target, float MaxHeight)
    {
        float Ychange = target.y - spawn.position.y;
        Vector3 velocityY = new Vector3(0, Mathf.Sqrt(-2 * gravity * MaxHeight), 0);

        simulationTime = Mathf.Sqrt(-2 * MaxHeight / gravity) + Mathf.Sqrt(2 * (Ychange - MaxHeight) / gravity);

        Vector3 XZchange = new Vector3(target.x - spawn.position.x, 0, target.z - spawn.position.z);
        Vector3 velocityXZ = XZchange / simulationTime;

        initialVelocity = velocityXZ + velocityY;
    }

    IEnumerator RotateToTarget(Vector3 t, Transform self, float duration = 1.0f)
    {
        float halfDuration = duration / 2;

        Vector3 direction =  new Vector3(initialVelocity.x,0, initialVelocity.z);
        Quaternion from = self.transform.rotation;
        Quaternion to = Quaternion.LookRotation(direction, Vector3.up);

        float elapsed = 0.0f;
        while (elapsed < halfDuration)
        {
            self.transform.rotation = Quaternion.Lerp(from, to, elapsed / halfDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        self.transform.rotation = to;

        direction = initialVelocity;
        from = self.transform.rotation;
        to = Quaternion.LookRotation(direction, Vector3.up);

        elapsed = 0.0f;
        while (elapsed < halfDuration)
        {
            self.transform.rotation = Quaternion.Lerp(from, to, elapsed / halfDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        self.transform.rotation = to;
        FireCannonballAt(rotatePoint.position, t);
    }
    IEnumerator RotateToReset(Vector3 t, Transform self, float duration = 1.0f, float delay = 0f)
    {
        if (delay > 0f)
        {
            yield return new WaitForSeconds(delay);
            delay = 0f;
        }

        float halfDuration = duration / 2;

        Vector3 direction = new Vector3(initialVelocity.x, t.y - self.transform.position.y, initialVelocity.z);
        Quaternion from = self.transform.rotation;
        Quaternion to = Quaternion.LookRotation(direction, Vector3.up);

        float elapsed = 0.0f;
        while (elapsed < halfDuration)
        {
            self.transform.rotation = Quaternion.Lerp(from, to, elapsed / halfDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        self.transform.rotation = to;

        direction = new Vector3(t.x - self.position.x, 0, t.z - self.position.z);
        from = self.transform.rotation;
        to = Quaternion.LookRotation(direction, Vector3.up);

        elapsed = 0.0f;
        while (elapsed < halfDuration)
        {
            self.transform.rotation = Quaternion.Lerp(from, to, elapsed / halfDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        self.transform.rotation = to;
    }

    IEnumerator Parabola(GameObject subject, Vector3 initialVelocity, float completionTime, int resolution)
    {
        // Transform Projectile over time
        Vector3 initialPosition = subject.transform.position;
        float t = 0f;
        while (t <= completionTime && subject != null)
        {
            t = t + Time.deltaTime;
            Vector3 displacement = initialVelocity * t + Vector3.up * gravity * t * t / 2f;
           subject.transform.position = initialPosition + displacement;        
            yield return null;
        }
    }

    IEnumerator Explosion(Vector3 pos, float delay)
    {
        yield return new WaitForSeconds(delay);
        this.GetComponent<Cannonball>().Explode(pos);
    }
}
