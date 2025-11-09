using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    [SerializeField] private GameObject TraansitionScene;
    public void switchScene(string sceneName)
    {

        StartCoroutine(LoadSceneWithTransition(sceneName));
    }
    public void onEnableMenu(GameObject menu)
    {
        menu.SetActive(true);
    }
    public void onDisableMenu(GameObject menu)
    {
        menu.SetActive(false);
    }
    public void ApplicationQuit()
    {
        Application.Quit();
    }

    private IEnumerator LoadSceneWithTransition(string sceneName)
    {

        TraansitionScene.SetActive(true);


        yield return new WaitForSeconds(2f);


        SceneManager.LoadScene(sceneName);
    }
}
