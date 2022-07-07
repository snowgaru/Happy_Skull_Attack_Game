using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Attack1 : MonoBehaviour
{
    // Start is called before the first frame update

    void Start()
    {
        //StartCoroutine(Move());
    }

    IEnumerator Move()
    {
        //yield return new WaitForSeconds(0.05f);
        //transform.DOMove(-transform.right * 5, 2.5f);
        yield return new WaitForSeconds(5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
