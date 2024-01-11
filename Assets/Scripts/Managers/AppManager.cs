using UnityEngine;

public class AppManager : MonoBehaviour
{
    public static AppManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Init();   
    }

    private void Init()
    {
        string userId = SecurePlayerPrefsManager.GetUserId();
        if (userId != string.Empty)
        {
            DataManager.Instance.User = new data.UserData() { id = userId };
            SceneSwitchManager.Instance.LoadMainScene();
        }
        else
        {
            SceneSwitchManager.Instance.LoadUserInfoScreen();
        }
    }
}
