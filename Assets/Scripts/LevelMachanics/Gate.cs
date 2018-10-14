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

    public bool isOpen;

    public void Open()
    {
        isOpen = true;
    }
    public void Close()
    {
        isOpen = false;
    }
    private void Update()
    {
        if (isOpen)
        {
            var destination = new Vector3(gateObject.transform.localPosition.x, openPosition.localPosition.y, gateObject.transform.localPosition.z);
            gateObject.transform.localPosition = Vector3.MoveTowards(gateObject.transform.localPosition, destination, Time.deltaTime * openSpeed);
        }
        else
        {
            var destination = new Vector3(gateObject.transform.localPosition.x, closePosition.localPosition.y, gateObject.transform.localPosition.z);
            gateObject.transform.localPosition = Vector3.MoveTowards(gateObject.transform.localPosition, destination, Time.deltaTime * closeSpeed);
        }
    }
}
