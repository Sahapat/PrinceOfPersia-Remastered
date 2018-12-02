using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameCore
{
    public static TimeCounter timeCounter;
    public static GameManager gameManager;
    public static CombatController combatController;
    public static CameraController cameraController;
    public static UIHandler uIHandler;
}
public class GameManager : MonoBehaviour
{
    [SerializeField] AudioClip startSound;
    [SerializeField] AudioClip killSound;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip fightDeathSound;
    [SerializeField] AudioClip successSound1;
    [SerializeField] AudioClip successSound2;
    public bool isGameEnd { get; private set; }

    AudioSource mAudiosSource;
    void Awake()
    {
        GameCore.gameManager = this;
        GameCore.timeCounter = GetComponent<TimeCounter>();
        GameCore.combatController = GetComponent<CombatController>();
        GameCore.cameraController = Camera.main.GetComponent<CameraController>();
        GameCore.uIHandler = GetComponent<UIHandler>();
        mAudiosSource = GetComponent<AudioSource>();
    }
    private void Update()
    {
        if (isGameEnd)
        {
            if (InputManager.GetKeyDown_Interact())
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }
    public void GameEnd()
    {
        isGameEnd = true;
        GameCore.combatController.canCombat = false;
    }
    public void startSoundPlay()
    {
        if (startSound)
        {
            mAudiosSource.PlayOneShot(startSound);
        }
    }
    public void killSoundPlay()
    {
        mAudiosSource.PlayOneShot(killSound);
    }
    public void deathSoundPlay()
    {
        mAudiosSource.PlayOneShot(deathSound);
    }
    public void FightdeathSoundPlay()
    {
        mAudiosSource.PlayOneShot(fightDeathSound);
    }
    public void SuccessSoundPlay()
    {
        mAudiosSource.PlayOneShot(successSound1);
    }
    public void SuccessSoundPlay2()
    {
        mAudiosSource.PlayOneShot(successSound2);
    }
}
