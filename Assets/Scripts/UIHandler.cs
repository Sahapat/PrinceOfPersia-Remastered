using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    [Header("HealthUI")]
    [SerializeField] Sprite[] princeUIHealthSprite;
    [SerializeField] Sprite[] enemyUIHealthSprite;
    [SerializeField] Image[] prince;
    [SerializeField] Image[] enemy;
    [SerializeField] GameObject showText;

    void Start()
    {
        Invoke("CloseText",4f);
    }

    void Update()
    {
        if (!GameCore.combatController.currentEnemy)
        {
			CloseUIEnemy();
        }
		else if(GameCore.gameManager.isGameEnd)
		{
			CloseUIPrince();
		}
    }
    public void UpdateUIPrince(Prince princeScript)
    {
        for (int i = 0; i < prince.Length; i++)
        {
            prince[i].enabled = false;
        }
        var maxHealth = princeScript.MaxHealth;
        for (int i = 0; i < maxHealth; i++)
        {
            prince[i].enabled = true;
        }
        for (int i = 0; i < prince.Length; i++)
        {
            prince[i].sprite = princeUIHealthSprite[0];
        }
        var currentHealth = princeScript.health;
        for (int i = 0; i < currentHealth; i++)
        {
            prince[i].sprite = princeUIHealthSprite[1];
        }
    }
    public void UpdateUIEnemy(Guard guardScript)
    {
        OpenUIEnemy();
        for (int i = 0; i < enemy.Length; i++)
        {
            enemy[i].enabled = false;
        }
        var maxHealth = guardScript.MaxHealth;
        for (int i = 0; i < maxHealth; i++)
        {
            enemy[i].enabled = true;
        }
        for (int i = 0; i < enemy.Length; i++)
        {
            enemy[i].sprite = enemyUIHealthSprite[0];
        }
        var currentHealth = guardScript.health;

        for (int i = 0; i < currentHealth; i++)
        {
            enemy[i].sprite = enemyUIHealthSprite[1];
        }
    }
	public void CloseUIPrince()
	{
		for (int i = 0; i < prince.Length; i++)
        {
            prince[i].enabled =false;
        }
	}
	public void CloseUIEnemy()
	{
		for (int i = 0; i < prince.Length; i++)
        {
            enemy[i].enabled = false;
        }
	}
    public void OpenUIEnemy()
    {
        for (int i = 0; i < prince.Length; i++)
        {
            enemy[i].enabled = true;
        }
    }
    private void CloseText()
    {
        showText.SetActive(false);
    }
}
