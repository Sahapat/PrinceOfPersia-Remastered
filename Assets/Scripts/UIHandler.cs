using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
	[Header("UI reference")]
	[SerializeField] private Text princeHealthText;
	[SerializeField] private Text enemyHealthText;

	void Update()
	{
		if(GameCore.combatController.currentEnemy)
		{
			enemyHealthText.enabled = true;
		}
		else
		{
			enemyHealthText.enabled = false;
		}
	}

	public void SetPrinceHealthText(int health)
	{
		princeHealthText.text = "Prince health: "+health;
	}
	public void SetEnemyHealthText(int health)
	{
		enemyHealthText.text = health+" :Enemy health";
	}
}
