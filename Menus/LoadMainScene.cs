using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadMainScene : MonoBehaviour
{
    [SerializeField] private bool startAsyncLoad = true;
    AsyncOperation asyncLoad;
    private Transform loadingPanel;
    private float countdown = 5f;

    private void Start()
    {
        loadingPanel = transform.Find("MainMenu").Find("MainPanel").Find("LoadingPanel");
        Application.backgroundLoadingPriority = ThreadPriority.Low;
    }

    void Update()
    {
        if (asyncLoad != null)
        {
            loadingPanel.Find("Description").GetComponent<TextMeshProUGUI>().text = Mathf.RoundToInt((asyncLoad.progress / 9) * 1000) + "%";
        }
    }

    void LateUpdate()
    {
        if (startAsyncLoad == true)
        {
            if (countdown <= 0)
            {
                startAsyncLoad = false;
                StartCoroutine(LoadAsyncScene());
            }
            else
            {
                countdown -= Time.fixedDeltaTime;
            }
        }
    }

    IEnumerator LoadAsyncScene()
    {
        asyncLoad = SceneManager.LoadSceneAsync("MainScene");
        asyncLoad.allowSceneActivation = false;
        loadingPanel.GetComponent<Animator>().Play("Finding Match In");

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        loadingPanel.Find("Title").GetComponent<TextMeshProUGUI>().text = "WORLD LOADED!";
    }

    public IEnumerator EnableMainScene()
    {
        transform.Find("MainMenu").Find("Loading").GetComponent<Animator>().Play("Panel In");
        transform.Find("MainMenu").Find("MainPanel").gameObject.SetActive(false);

        yield return asyncLoad.isDone;
        yield return new WaitForSecondsRealtime(0.5f);
        asyncLoad.allowSceneActivation = true;
    }
}
