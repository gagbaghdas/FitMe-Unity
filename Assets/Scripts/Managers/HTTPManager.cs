using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class HttpManager : MonoBehaviour
{
    //office
    private const string ApiUrl = "http://192.168.0.131:5000/api/";

    //home
    //private const string ApiUrl = "http://192.168.10.12:5000/api/";
    private const string UploadImageEndPoint = ApiUrl + "upload_main_image";
    private const string SendPromptEndPoint = ApiUrl + "img2img";
    private const string CreateUserEndpoint = ApiUrl + "create_user";
    public static HttpManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep the instance alive across scenes
        }
        else
        {
            Destroy(gameObject); // Ensures only one instance exists
        }
    }

    // Example method for sending a basic request with JSON data
    private IEnumerator PostRequest(string url, string jsonData, Action<ResponseData> onSuccess = null, Action<string> onFailure = null)
    {
        if (DataManager.Instance.User != null && DataManager.Instance.User.id != null)
        {
            url = $"{url}?user_id={Uri.EscapeDataString(DataManager.Instance.User.id)}";
        }
        // Convert the JSON data to a byte array
        byte[] jsonToSend = new UTF8Encoding().GetBytes(jsonData);

        // Create a new UnityWebRequest, setting the URL and method explicitly
        using UnityWebRequest request = new(url, "POST");
        // Create a new UploadHandler with the JSON data byte array
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();

        // Set the Content-Type header to application/json
        request.SetRequestHeader("Content-Type", "application/json");

        // Send the request and wait for a response
        yield return request.SendWebRequest();

        OnResponse(request, onSuccess, onFailure);
    }

    private IEnumerator UploadImage(string url, Texture2D image, Action<ResponseData> onSuccess = null, Action<string> onFailure = null)
    {
        if (DataManager.Instance.User != null && DataManager.Instance.User.id != null)
        {
            url = $"{url}?user_id={Uri.EscapeDataString(DataManager.Instance.User.id)}";
        }
        
        byte[] imageData = image.EncodeToPNG();

        using UnityWebRequest request = UnityWebRequest.Put(url, imageData);
        request.SetRequestHeader("Content-Type", "application/octet-stream");

        yield return request.SendWebRequest();

        OnResponse(request, onSuccess, onFailure);
    }

    private IEnumerator DownloadImage(string url, Action<Texture2D> onCompleted = null)
    {
        using UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.error);
            onCompleted?.Invoke(null);
        }
        else
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            onCompleted?.Invoke(texture);
        }
    }

    private IEnumerator GetRequest(string url, Action<ResponseData> onSuccess = null, Action<string> onFailure = null)
    {
        if (DataManager.Instance.User != null && DataManager.Instance.User.id != null)
        {
            url = $"{url}?user_id={Uri.EscapeDataString(DataManager.Instance.User.id)}";
        }
        using UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        OnResponse(request, onSuccess, onFailure);
    }

    private void OnResponse(UnityWebRequest request, Action<ResponseData> onSuccess = null, Action<string> onFailure = null)
    {
        if (request.result != UnityWebRequest.Result.Success)
        {
            onFailure?.Invoke(request.error);
            Debug.LogError(request.error);
        }
        else
        {
            onSuccess?.Invoke(JsonConvert.DeserializeObject<ResponseData>(request.downloadHandler.text));
        }
    }

    public void UploadMainImage(Texture2D image, Action<ResponseData> onSuccess = null, Action<string> onFailure = null)
    {
        StartCoroutine(UploadImage(UploadImageEndPoint, image, onSuccess, onFailure));
    }

    public void SendPrompt(string prompt, Action<Texture2D> onSuccess)
    {
        LoadingAnimationManager.Instance.Show();

        Img2ImgData img2ImgData = new()
        {
            prompt = prompt,
            negative_prompt = "",
            init_image = "",
            width = 400,
            height = 480,
            samples = 1
        };

        string data = JsonConvert.SerializeObject(img2ImgData);
        StartCoroutine(PostRequest(SendPromptEndPoint, data, (ResponseData responseData) => {
            LoadingAnimationManager.Instance.Hide();
            if (responseData != null && responseData != null)
            {
                LoadingAnimationManager.Instance.Show();
                if (responseData.data.TryGetValue("output", out object outputObject))
                {
                    // Ensure that the object is a JArray and convert it to a List<string>.
                    var outputs = ((JArray)outputObject).ToObject<List<string>>();

                    // Access your data here
                    foreach (var outputUrl in outputs)
                    {
                        StartCoroutine(DownloadImage(outputUrl, (Texture2D image) => { LoadingAnimationManager.Instance.Hide(); onSuccess?.Invoke(image); }));
                    }
                }

                Debug.Log("Status Code: " + responseData.status_code);
            }
            else
            {
                Debug.LogError("Invalid JSON response");
            }
        }, (string b) => {

        }));
    }

    public void CreateUser(UserData userData, Action<ResponseData> onSuccess = null, Action<string> onFailure = null)
    {
        string data = JsonConvert.SerializeObject(userData);

        StartCoroutine(PostRequest(CreateUserEndpoint, data, onSuccess, onFailure));
    }
}
