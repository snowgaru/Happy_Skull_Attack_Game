using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : PoolableMono
{
    public float force = 1500.0f;

    private Rigidbody bulletRigidbody = null;
    private Transform bulletTransform = null;

    private TrailRenderer trailRenderer = null;

    public override void Reset()
    {

    }


    public void Fire()
    {
        StartCoroutine(Shoot());
    }

    private IEnumerator Shoot()
    {
        bulletRigidbody = GetComponent<Rigidbody>();
        bulletTransform = GetComponent<Transform>();
        trailRenderer = GetComponent<TrailRenderer>();
        bulletRigidbody.AddForce(bulletTransform.right * force);
        trailRenderer.Clear();
        yield return new WaitForSeconds(0.1f);
        Invoke("DestroyBullet", 1f);
    }

    void DestroyBullet()
    {
        bulletRigidbody.velocity = Vector3.zero;
        PoolManager.Instance.Push(this);
    }

}
