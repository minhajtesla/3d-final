using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Crud : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public Button loginButton;
    private string baseURL = "https://lets-play-delta.vercel.app/auth/login";

    void Start()
    {
        StartCoroutine(CheckServerConnection());
        loginButton.onClick.AddListener(OnLoginButtonClick); // Add listener for login button click
    }

    public void OnLoginButtonClick()
    {
        string email = emailInput.text;
        string password = passwordInput.text;
        StartCoroutine(Login(email, password));
    }

    IEnumerator Login(string email, string password)
    {
        LoginRequest loginData = new LoginRequest { email = email, password = password };
        string json = JsonUtility.ToJson(loginData);

        using (UnityWebRequest request = new UnityWebRequest(baseURL, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Login Failed: " + request.error);
            }
            else
            {
                Debug.Log("Login Successful: " + request.downloadHandler.text);
            }
        }
    }

    IEnumerator CheckServerConnection()
    {
        using (UnityWebRequest request = UnityWebRequest.Get("https://lets-play-delta.vercel.app"))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Cannot Reach Server: " + request.error);
            }
            else
            {
                Debug.Log("Server is Reachable!");
            }
        }
    }

    [System.Serializable] // Make the LoginRequest class serializable
    public class LoginRequest
    {
        public string email;
        public string password;
    }
}
