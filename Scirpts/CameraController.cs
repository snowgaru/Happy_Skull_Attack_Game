using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraController : MonoBehaviour
{
    public GameObject target;

    public float offsetX;
    public float offsetY;
    public float offsetZ;

    void Update()
    {
        Vector3 FixedPos =
            new Vector3(
                target.transform.position.x + offsetX,
                target.transform.position.y + offsetY,
                target.transform.position.z + offsetZ);
        transform.position = FixedPos;
    }

    public void PerfectDodgeFunction()
    {
        StartCoroutine(PerfectDodge());
    }
        
    public IEnumerator PerfectDodge()
    {
        for(int i = 0; i < 30; i++)
        {
            offsetY -= 0.02f;
            yield return new WaitForSeconds(0.01f);
        }
        for(int i = 0; i < 15; i++)
        {
            offsetY += 0.04f ;
            yield return new WaitForSeconds(0.01f);
        }
        //transform.DOMoveY(2.75f, 1.4f).OnComplete(() =>
        //{
        //    transform.position = new Vector3(
        //        target.transform.position.x,
        //        5,
        //        target.transform.position.z);
        //});
    }
}
