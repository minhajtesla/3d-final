using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Web;

public class RegistrationManager : MonoBehaviour
{
    public MenuManager menuManager;
    public TMP_InputField emailInput;
    public TMP_InputField nameInput;
    public TMP_InputField passwordInput;
    public TMP_InputField zipCodeInput;
    public TMP_Dropdown ageDropdown;
    public TMP_Dropdown sexDropdown;
    public Button registerButton;
    public TextMeshProUGUI statusMessage;
    public GameObject successPanel;
    public GameObject failurePanel;
    public Button tryAgainButton;
    public GameObject popupPanel;
    public TextMeshProUGUI popupMessage;

    private string baseURL = "https://lets-play-delta.vercel.app/auth/register";
    private string verifyURL = "https://bauhof.arrimo.studio/verify-email";

    void Start()
    {
        registerButton.onClick.AddListener(OnRegisterButtonClick);
        tryAgainButton.onClick.AddListener(OnTryAgainButtonClick);
    }

    public void OnRegisterButtonClick()
    {
        string email = emailInput.text;
        string name = nameInput.text;
        string password = passwordInput.text;
        int zipCode;

        if (string.IsNullOrEmpty(email))
        {
            ShowPopup("Email field is required.");
            return;
        }
        if (string.IsNullOrEmpty(name))
        {
            ShowPopup("Name field is required.");
            return;
        }
        if (string.IsNullOrEmpty(password))
        {
            ShowPopup("Password field is required.");
            return;
        }
        if (string.IsNullOrEmpty(zipCodeInput.text) || !int.TryParse(zipCodeInput.text, out zipCode) || zipCodeInput.text.Length != 5)
        {
            ShowPopup("Zip Code must be exactly 5 digits.");
            return;
        }

        string age = GetSelectedAge();
        string sex = GetSelectedSex();

        StartCoroutine(RegisterUser(email, name, password, age, sex, zipCode));
    }

    void OnTryAgainButtonClick()
    {
        failurePanel.SetActive(false);
    }

    string GetSelectedAge()
    {
        return ageDropdown.options[ageDropdown.value].text;
    }

    string GetSelectedSex()
    {
        return sexDropdown.options[sexDropdown.value].text;
    }

    IEnumerator RegisterUser(string email, string name, string password, string age, string sex, int zipCode)
    {
        RegistrationRequest registrationData = new RegistrationRequest
        {
            email = email,
            name = name,
            password = password,
            age = age,
            sex = sex,
            zipCode = zipCode
        };

        string json = JsonUtility.ToJson(registrationData);
        Debug.Log("Sending JSON: " + json);

        using (UnityWebRequest request = new UnityWebRequest(baseURL, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                ShowFailure("Registration Failed: " + request.downloadHandler.text);
            }
            else
            {
                menuManager.ShowPanel(menuManager.loginPanel);
            }
        }
    }

    void ShowSuccess(string message)
    {
        successPanel.SetActive(true);
        failurePanel.SetActive(false);
        statusMessage.text = message;
    }

    void ShowFailure(string message)
    {
        successPanel.SetActive(false);
        failurePanel.SetActive(true);
        statusMessage.text = message;
    }

    void ShowPopup(string message)
    {
        popupPanel.SetActive(true);
        popupMessage.text = message;
        Invoke("HidePopup", 3f);
    }

    void HidePopup()
    {
        popupPanel.SetActive(false);
    }

    [System.Serializable]
    public class RegistrationRequest
    {
        public string email;
        public string name;
        public string password;
        public string age;
        public string sex;
        public int zipCode;
    }
}
