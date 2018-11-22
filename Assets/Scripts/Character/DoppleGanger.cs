using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoppleGanger : MonoBehaviour
{
	public float speed;
	GameObject[] childs;
	Animator m_anim;
	SpriteRenderer m_spriteRenderer;

	private Vector3 destination;
	bool isRunning;
	void Awake()
	{
		childs = new GameObject[transform.childCount];
		for(int i =0;i<transform.childCount;i++)
		{
			childs[i] = transform.GetChild(i).gameObject;
			childs[i].SetActive(false);
		}
		m_anim = GetComponent<Animator>();
		m_spriteRenderer = GetComponent<SpriteRenderer>();
		m_spriteRenderer.enabled = false;
	}
	void Update()
	{
		if(isRunning)
		{
			transform.position = Vector3.MoveTowards(transform.position,destination,speed*Time.deltaTime);
		}
	}
	public void MoveToDestination()
	{
		for(int i =0;i<transform.childCount;i++)
		{
			childs[i].SetActive(false);
		}
		m_spriteRenderer.enabled = true;
		m_anim.SetBool("Run",true);
		isRunning = true;
		destination = new Vector3(transform.position.x+15,transform.position.y,transform.position.z);
	}
}
