using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopTime : MonoBehaviour
{
    bool stopping;
    public float _slowTime = 0.4f;
    public float slowTimeScale = 0.7f;

    public void TimeStop(float slowTime)
    {
        if(!stopping)
        {
            _slowTime = slowTime;
            stopping = true;
            //Time.timeScale = 0;
            StartCoroutine("Stop");
            Debug.Log("¾î·Æ³×");
        }
    }

    IEnumerator Stop()
    {
        //yield return new WaitForSeconds(stopTime);
        Time.timeScale = 0.7f;
        yield return new WaitForSeconds(_slowTime);
        Time.timeScale = 1f;

        stopping = false;
    }

}
