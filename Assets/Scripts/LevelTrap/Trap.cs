using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    [SerializeField] private float trapDuration;
    [SerializeField] private float trapWait;
	[SerializeField] private float activeTime;
	private Animator trapAnim;
	private BoxCollider2D checkerColider;
    float trapDurationCounter;
	bool isTrapActive;
	void Awake()
	{
		trapAnim = GetComponent<Animator>();
		checkerColider = GetComponent<BoxCollider2D>();
	}
    void Update()
    {
		if(trapDurationCounter <= Time.time)
		{
			trapDurationCounter = Time.time + trapDuration;
			StartCoroutine(active());
		}
		if(isTrapActive)
		{
			var checkPos = new Vector2(transform.position.x+checkerColider.offset.x,transform.position.y+checkerColider.offset.y);
			var playerCheck = Physics2D.OverlapBox(checkPos,checkerColider.size,0,LayerMask.GetMask("Player"));
			if(playerCheck)
			{
				playerCheck.GetComponent<Prince>().Dead();
			}
		}
    }
	private IEnumerator active()
	{
		trapAnim.SetBool("Active",true);
		yield return new WaitForSeconds(trapWait);
		isTrapActive = true;
		yield return new WaitForSeconds(activeTime);
		isTrapActive = false;
		trapAnim.SetBool("Active",false);
	}
}
