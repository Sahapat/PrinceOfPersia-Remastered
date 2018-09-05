using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TimeCounter : MonoBehaviour
{
    public float gameTime { get; private set; }
    private WaitForSeconds second;

    void Awake()
    {
        second = new WaitForSeconds(1f);
    }
    public void StartCountUp()
    {
        StopAllCoroutines();
        StartCoroutine(Counting(1f));
    }
    public void StartCountDown()
    {
        StopAllCoroutines();
        StartCoroutine(Counting(-1f));
    }
    public void StopCount()
    {
        StopAllCoroutines();
    }
    private IEnumerator Counting(float time)
    {
        while(true)
        {
            gameTime += time;
            yield return second;
        }
    }
}