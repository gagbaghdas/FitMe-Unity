using UnityEngine;
using UnityEngine.UI;

public class LoadingAnimationManager : MonoBehaviour
{
    public static LoadingAnimationManager Instance { get; private set; }

    public float rotationSpeed = 200f;
    public GameObject loadingGameObject;
    public Image inputBlocker; // Assign a full-screen transparent image here

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
        Hide();
    }

    void Update()
    {
        // Rotate the image every frame to create a spinning effect
        loadingGameObject.transform.Rotate(Vector3.back, rotationSpeed * Time.deltaTime);
    }

    public void Show()
    {
        loadingGameObject.SetActive(true);
        inputBlocker.gameObject.SetActive(true);

    }

    public void Hide()
    {
        loadingGameObject.SetActive(false);
        inputBlocker.gameObject.SetActive(false);
    }
}
