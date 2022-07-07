using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CoinManager : PoolableMono
{
    public void Rolling()
    {
        transform.Rotate(Vector3.forward, Time.deltaTime * 90);
    }

    void Update()
    {
        Rolling();
    }

    public override void Reset()
    {
        
    }
}
