using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private GameObject gateObject;
    [SerializeField] private Transform closePosition;
    [SerializeField] private Transform openPosition;

    [SerializeField] private float openSpeed;
    [SerializeField] private float closeSpeed;
    [SerializeField] private float normalCloseSpeed;
    [SerializeField] AudioClip close;
    [SerializeField] AudioClip closeNormal;
    [SerializeField] AudioClip open;

    AudioSource gateAudioSource;
    public bool isOpen;
    public bool isNormalClose;
    public bool isSpeedClose;
    private bool canClose;
    private float closeCount;
    private float openCount;
    private bool closeSetTrigger;
    void Awake()
    {
        gateAudioSource = GetComponent<AudioSource>();
        closeSetTrigger = true;
    }

    public void Open()
    {
        isOpen = true;
        isNormalClose = false;
    }
    public void ForClose()
    {
        var destination = new Vector3(gateObject.transform.localPosition.x, openPosition.localPosition.y, gateObject.transform.localPosition.z);
        if (gateObject.transform.localPosition == destination)
        {
            if (closeSetTrigger)
            {
                closeSetTrigger = false;
                StartCoroutine(waitForClose());
            }
        }
    }
    public void Close()
    {
        isSpeedClose = true;
    }
    private void Update()
    {
        if (isOpen)
        {
            var destination = new Vector3(gateObject.transform.localPosition.x, openPosition.localPosition.y, gateObject.transform.localPosition.z);
            gateObject.transform.localPosition = Vector3.MoveTowards(gateObject.transform.localPosition, destination, Time.deltaTime * openSpeed);
            if (gateObject.transform.localPosition == destination)
            {
                isOpen = false;
            }
            if (openCount <= Time.time && !(gateObject.transform.localPosition == destination))
            {
                gateAudioSource.PlayOneShot(open);
                openCount = Time.time + 0.3f;
            }
        }
        else if (isSpeedClose)
        {
            var destination = new Vector3(gateObject.transform.localPosition.x, closePosition.localPosition.y, gateObject.transform.localPosition.z);
            gateObject.transform.localPosition = Vector3.MoveTowards(gateObject.transform.localPosition, destination, Time.deltaTime * closeSpeed);
            if (gateObject.transform.localPosition == destination)
            {
                isSpeedClose = false;
                gateAudioSource.PlayOneShot(close);
            }
        }
        else if (!isOpen && isNormalClose)
        {
            var destination = new Vector3(gateObject.transform.localPosition.x, closePosition.localPosition.y, gateObject.transform.localPosition.z);
            gateObject.transform.localPosition = Vector3.MoveTowards(gateObject.transform.localPosition, destination, Time.deltaTime * normalCloseSpeed);
            if (gateObject.transform.localPosition == destination)
            {
                isNormalClose = false;
            }
            if (closeCount <= Time.time && !(gateObject.transform.localPosition == destination))
            {
                gateAudioSource.PlayOneShot(closeNormal);
                closeCount = Time.time + 0.6f;
            }
        }
    }
    private IEnumerator waitForClose()
    {
        isOpen = false;
        yield return new WaitForSeconds(6f);
        isNormalClose = true;
        closeSetTrigger = true;
    }
}
