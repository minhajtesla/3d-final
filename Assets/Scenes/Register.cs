using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Register : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_InputField nameInput;
    public TMP_InputField passwordInput;
    public TMP_InputField ageInput;
    public TMP_InputField zipCodeInput;
    public TMP_Dropdown sexDropdown;  // Dropdown for sex selection
    public Button registerButton;

    private string baseURL = "https://lets-play-delta.vercel.app/auth/register"; // API Endpoint

    void Start()
    {
        registerButton.onClick.AddListener(OnRegisterButtonClick);
    }

    public void OnRegisterButtonClick()
    {
        string email = emailInput.text;
        string name = nameInput.text;
        string password = passwordInput.text;
        string age = ageInput.text;
        int zipCode;

        if (!int.TryParse(zipCodeInput.text, out zipCode))
        {
            Debug.LogError("Invalid Zip Code!");
            return;
        }

        string sex = GetSelectedSex(); // Get selected sex from dropdown

        StartCoroutine(RegisterUser(email, name, password, age, sex, zipCode));
    }

    string GetSelectedSex()
    {
        return sexDropdown.options[sexDropdown.value].text; // Get selected dropdown text
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
            request.SetRequestHeader("Accept", "*/*");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Registration Failed: " + request.error);
                Debug.LogError("Response: " + request.downloadHandler.text);
            }
            else
            {
                Debug.Log("Registration Successful: " + request.downloadHandler.text);
            }
        }
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


