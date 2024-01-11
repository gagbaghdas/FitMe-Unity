using UnityEngine;
using TMPro;
using UnityEngine.UI;
using data;
using UnityEngine.SceneManagement;

public class MainScreenHandler : MonoBehaviour
{
    public Image displayedImage;
    public TMP_InputField inputField;
    public Button sendButton;

    void Start()
    {
        DataManager.Instance.MainImage = Utils.LoadTextureFromPath("MainImage");
        // Set the uploaded image to the displayedImage
        if (DataManager.Instance.MainImage != null)
        {
            SetImage(DataManager.Instance.MainImage);
        }

        // Add click listener for sendButton
        sendButton.onClick.AddListener(OnSendButtonClick);
    }


    private void SetImage(Texture2D image)
    {
        displayedImage.sprite = Sprite.Create(image, new Rect(0.0f, 0.0f, image.width, image.height), new Vector2(0.5f, 0.5f));
    }

    public void OnSendButtonClick()
    {
        string inputText = inputField.text;
        HttpManager.Instance.SendPrompt(inputText , (Texture2D image) => {

            SetImage(image);

        });
        // Code to handle sending inputText to the backend
    }

    public void OnUploadButtonClick()
    {
        // Call a method to open the camera/gallery
        PickImage(1024); // 1024 is the max size of the image
    }

    public void OnCameraButtonClick()
    {
        // Open the device camera to take a picture
        TakePicture(1024);
    }

    private void PickImage(int maxSize)
    {
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            OnImageSelect(path, maxSize);
        }, "Select a PNG image", "image/png");

        Debug.Log("Permission result: " + permission);
    }

    private void TakePicture(int maxSize)
    {
        NativeCamera.Permission permission = NativeCamera.TakePicture((path) =>
        {
            OnImageSelect(path, maxSize);
        }, maxSize);

        Debug.Log("Camera permission result: " + permission);
    }

    private void OnImageSelect(string path, int maxSize)
    {
        if (path != null)
        {
            // Create Texture from the taken picture
            Texture2D texture = NativeGallery.LoadImageAtPath(path, maxSize, false, false);
            if (texture == null)
            {
                Debug.Log("Couldn't load texture from " + path);
                return;
            }
            LoadingAnimationManager.Instance.Show();
            HttpManager.Instance.UploadMainImage(texture, (ResponseData a) => {
                LoadingAnimationManager.Instance.Hide();
                SetImage(DataManager.Instance.MainImage = texture);
                Utils.SaveTextureAsPNG(texture, "MainImage");
            });
        }
    }
}
