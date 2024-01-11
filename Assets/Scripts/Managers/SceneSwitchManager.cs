using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitchManager : MonoBehaviour
{
    private const string loadingSceneName = "LoadingScreen";
    private const string welcomeSceneName = "WelcomeScreen";
    private const string mainSceneName = "MainScreen";
    private const string userInfoScreen = "UserInfoScreen";
    // Singleton instance
    public static SceneSwitchManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Load scene by name
    private void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // Load scene by build index
    private void LoadSceneByIndex(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void LoadLoadingScene()
    {
        LoadSceneByName(loadingSceneName);
    }

    public void LoadWelcomeScene()
    {
        LoadSceneByName(welcomeSceneName);
    }

    public void LoadMainScene()
    {
        LoadSceneByName(mainSceneName);
    }

    public void LoadUserInfoScreen()
    {
        LoadSceneByName(userInfoScreen);
    }
}

