using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    [Header("Menu property")]
    [SerializeField] Sprite[] slideShow;
    [SerializeField] float changeTime;
	[SerializeField] Sprite loadingSprite;


    private WaitForSeconds waitForFade;
    private SpriteRenderer mSpriteRenderer;
    private bool changeTrigger;
    private int currentIndex;
    void Awake()
    {
        mSpriteRenderer = GetComponent<SpriteRenderer>();
        waitForFade = new WaitForSeconds(changeTime);
        changeTrigger = true;
        currentIndex = 0;
    }
    void Start()
    {
        mSpriteRenderer.sprite = slideShow[currentIndex];
    }
    void Update()
    {
        if (InputManager.GetKeyDown_Interact())
        {
			StartCoroutine(LoadNewScene());
        }
        if (changeTrigger)
        {
            StartCoroutine(changeSlide());
            changeTrigger = false;
        }
    }
    private IEnumerator changeSlide()
    {
        currentIndex++;
        var index = currentIndex;
        index = (index < slideShow.Length) ? index : 0;
        yield return waitForFade;
        mSpriteRenderer.sprite = slideShow[index];
        changeTrigger = true;
    }
    private IEnumerator LoadNewScene()
    {
        // Start an asynchronous operation to load the scene that was passed to the LoadNewScene coroutine.
        AsyncOperation async = SceneManager.LoadSceneAsync("Level1");

        // While the asynchronous operation to load the new scene is not yet complete, continue waiting until it's done.
        while (!async.isDone)
        {
			mSpriteRenderer.sprite = loadingSprite;
            yield return null;
        }

    }
}
