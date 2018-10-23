using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrinceSoundHandler : MonoBehaviour
{
	[SerializeField] AudioClip footStep;
	[SerializeField] AudioClip landSoft;
	[SerializeField] AudioClip landharm;
	private AudioSource mAudioSource;
	void Awake()
	{
		mAudioSource = GetComponent<AudioSource>();
	}
	public void footStepPlay()
	{
		mAudioSource.PlayOneShot(footStep);
	}
	public void landSoftPlay()
	{
		mAudioSource.PlayOneShot(landSoft);
	}
	public void landharmPlay()
	{
		mAudioSource.PlayOneShot(landharm);
	}
}
