using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySoundHandler : MonoBehaviour
{
	[SerializeField] AudioClip attack;
	[SerializeField] AudioClip defense;
	[SerializeField] AudioClip hit;
	[SerializeField] AudioClip draw;

	AudioSource mAudiosource;
	void Awake()
	{
		mAudiosource = GetComponent<AudioSource>();
	}
	public void AttackPlay()
	{
		mAudiosource.PlayOneShot(attack);
	}
	public void DefensePlay()
	{
		mAudiosource.PlayOneShot(defense);
	}
	public void HitPlay()
	{
		mAudiosource.PlayOneShot(hit);
	}
	public void DrawPlay()
	{
		mAudiosource.PlayOneShot(draw);
	}
}
