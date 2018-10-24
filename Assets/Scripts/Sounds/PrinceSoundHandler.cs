using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrinceSoundHandler : MonoBehaviour
{
	[SerializeField] AudioClip footStep;
	[SerializeField] AudioClip landSoft;
	[SerializeField] AudioClip landharm;
	[SerializeField] AudioClip scream;
	[SerializeField] AudioClip harm;
	[SerializeField] AudioClip potionSmall;
	[SerializeField] AudioClip potionBig;
	[SerializeField] AudioClip swordGet;
	[SerializeField] AudioClip drawSword;
	[SerializeField] AudioClip swordDefense;
	[SerializeField] AudioClip swordAttack;
	[SerializeField] AudioClip drinkSound;
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
	public void screamPlay()
	{
		mAudioSource.PlayOneShot(scream);
	}
	public void harmPlay()
	{
		mAudioSource.PlayOneShot(harm);
	}
	public void PotionSmallPlay()
	{
		mAudioSource.PlayOneShot(potionSmall);
	}
	public void PotionBigPlay()
	{
		mAudioSource.PlayOneShot(potionBig);
	}
	public void drawSwordPlay()
	{
		mAudioSource.PlayOneShot(drawSword);
	}
	public void swordAttackPlay()
	{
		mAudioSource.PlayOneShot(swordAttack);
	}
	
	public void swordDefensePlay()
	{
		mAudioSource.PlayOneShot(swordDefense);
	}
	
	public void swordGetPlay()
	{
		mAudioSource.PlayOneShot(swordGet);
	}
	public void drinkPlay()
	{
		mAudioSource.PlayOneShot(drinkSound);
	}
	
}
