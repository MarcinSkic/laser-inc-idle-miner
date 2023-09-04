using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LogoSceneLoader : MonoBehaviour {

    public Slider ProgressBar;
    public CanvasGroup ContainerLogo;
    public CanvasGroup ContainerBackground;

    

    public int SceneIndex;
    // Start is called before the first frame update
    void Start() {
        StartCoroutine(LoadGameAsync());
    }

    // Update is called once per frame
    void Update() {
        
    }

    IEnumerator LoadGameAsync() {
        yield return null;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneIndex, LoadSceneMode.Additive);

        while (!asyncLoad.isDone) {
            ProgressBar.value = asyncLoad.progress;
            yield return null;
        }
        ProgressBar.value = 1;
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(SceneIndex));
        yield return null;
        yield return null;
        yield return null;

        ProgressBar.gameObject.SetActive(false);

        while(ContainerBackground.alpha > 0) {
            ContainerBackground.alpha -= Time.deltaTime * 3;
            ContainerLogo.alpha -= Time.deltaTime * 3.3f;
            yield return null;
        }        
        yield return null;
        SceneManager.UnloadSceneAsync("Loader Scene");
    }


}
