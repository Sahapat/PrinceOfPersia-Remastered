using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardAnimationEventHandler : MonoBehaviour
{
	private GameObject objectParent;
	private Guard guard;
    void Awake()
	{
		objectParent = transform.parent.gameObject;
		guard = GetComponentInParent<Guard>();
	}

    public void SetControlable(string msg)
	{
		bool status = bool.Parse(msg);
		guard.SetControlable(status);
	}
	public void ParticlePlay()
	{
		guard.ParticlePlay();
	}
}
