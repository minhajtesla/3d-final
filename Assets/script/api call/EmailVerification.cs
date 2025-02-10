/*using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Web;
using TMPro;

public class EmailVerification : MonoBehaviour
{
    public GameObject successPanel; // Panel for successful verification
    public GameObject failurePanel; // Panel for unsuccessful verification
    public TextMeshProUGUI statusMessage; // UI text for status message

    private void Start()
    {
        // Capture the URL when the game is opened
        string url = Application.absoluteURL;

        if (!string.IsNullOrEmpty(url))
        {
            Debug.Log("Game opened with URL: " + url);
            ExtractAndVerifyToken(url);
        }
        else
        {
            Debug.LogWarning("No URL found.");
            ShowFailure("No verification URL detected.");
        }
    }

    private void ExtractAndVerifyToken(string url)
    {
        Uri uri = new Uri(url);
        var queryParams = HttpUtility.ParseQueryString(uri.Query);

        string email = queryParams["email"];
        string token = queryParams["token"];

        if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(token))
        {
            Debug.Log($"Extracted Email: {email}");
            Debug.Log($"Extracted Token: {token}");

            StartCoroutine(VerifyToken(email, token));
        }
        else
        {
            ShowFailure("Invalid or missing verification details.");
        }
    }

    private IEnumerator VerifyToken(string email, string token)
    {
        string apiUrl = "https://your-api.com/verify"; // Replace with your actual API endpoint
        WWWForm form = new WWWForm();
        form.AddField("email", email);
        form.AddField("token", token);

        using (UnityWebRequest request = UnityWebRequest.Post(apiUrl, form))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Verification Successful: " + request.downloadHandler.text);
                ShowSuccess("Email Verified Successfully!");
            }
            else
            {
                Debug.LogError("Verification Failed: " + request.error);
                ShowFailure("Verification Failed. Invalid token or expired link.");
            }
        }
    }

    private void ShowSuccess(string message)
    {
        successPanel.SetActive(true);
        failurePanel.SetActive(false);
        statusMessage.text = message;
    }

    private void ShowFailure(string message)
    {
        successPanel.SetActive(false);
        failurePanel.SetActive(true);
        statusMessage.text = message;
    }
}
*/