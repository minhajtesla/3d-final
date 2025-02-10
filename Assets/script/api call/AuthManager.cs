using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System;
using System.Web;

public class AuthManager : MonoBehaviour
{
    public TMP_InputField emailInputField;
    public TMP_InputField passwordInputField;
    public TMP_InputField tokenInputField; // Token input field
    public TMP_Text errorMessage;
    public Button signInButton;
    public Button forgotPasswordButton;
    public MenuManager menuManager;
    public GameObject errorPopupPanel; // Error popup panel
    public TMP_Text errorPopupText;
    public Button tryAgainButton;

    private string baseURL = "https://lets-play-delta.vercel.app/auth/login";

    void Start()
    {
        StartCoroutine(CheckServerConnection());
        signInButton.onClick.AddListener(OnSignInButtonClick);
        forgotPasswordButton.onClick.AddListener(() => menuManager.ShowPanel(menuManager.forgotPasswordPanel));
        tryAgainButton.onClick.AddListener(HideErrorPopup);
        errorPopupPanel.SetActive(false);

        // Capture the URL when the game is opened
        string url = Application.absoluteURL;

        if (!string.IsNullOrEmpty(url))
        {
            Debug.Log("Game opened with URL: " + url);
            ExtractAndSetToken(url);
        }
        else
        {
            Debug.LogWarning("No URL found.");
        }
    }

    private void ExtractAndSetToken(string url)
    {
        Uri uri = new Uri(url);
        var queryParams = HttpUtility.ParseQueryString(uri.Query);

        string email = queryParams["email"];
        string token = queryParams["token"];

        if (!string.IsNullOrEmpty(token))
        {
            Debug.Log($"Extracted Token: {token}");
            // Set the token in the token input field
            tokenInputField.text = token;
        }
        else
        {
            Debug.LogError("Failed to extract token from URL.");
        }
    }

    public void OnSignInButtonClick()
    {
        string email = emailInputField.text.Trim();
        string password = passwordInputField.text.Trim();

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ShowErrorPopup("Email and password are required!");
            return;
        }

        if (!IsValidEmail(email))
        {
            ShowErrorPopup("Invalid email format!");
            return;
        }

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
                ShowErrorPopup("Login Failed: " + request.error);
            }
            else
            {
                Debug.Log("Login Successful: " + request.downloadHandler.text);

                // Extract the access token from the JSON response
                string jsonResponse = request.downloadHandler.text;
                string accessToken = ExtractAccessToken(jsonResponse);

                // Save the token in PlayerPrefs
                PlayerPrefs.SetString("accessToken", accessToken);
                PlayerPrefs.Save();  // Ensure it is saved to disk

                // Optionally, display the token in an input field (e.g., tokenInputField)
                tokenInputField.text = accessToken;

                errorMessage.text = "Sign-in successful!";
                errorMessage.color = Color.green;
                menuManager.ShowPanel(menuManager.mainMenuPanel2);
            }
        }
    }

    // Helper method to extract the access token from the response
    private string ExtractAccessToken(string jsonResponse)
    {
        var data = JsonUtility.FromJson<AccessTokenResponse>(jsonResponse);
        return data.accessToken;
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

    void ShowErrorPopup(string message)
    {
        // Check if the message contains certain keywords that indicate an error
        if (message.ToLower().Contains("error") || message.ToLower().Contains("failed"))
        {
            message = "Invalid"; // Set the popup message to "Invalid" if it's an error
        }

        errorPopupText.text = message;  // Set the text in the popup
        errorPopupPanel.SetActive(true); // Show the popup
    }

    void HideErrorPopup()
    {
        errorPopupPanel.SetActive(false); // Hide the popup
    }

    bool IsValidEmail(string email)
    {
        string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, pattern);
    }

    [System.Serializable]
    public class LoginRequest
    {
        public string email;
        public string password;
    }

    [System.Serializable]
    public class AccessTokenResponse
    {
        public string accessToken;
        public string refreshToken;
    }
}
