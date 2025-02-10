using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System;
using System.Web;
using System.Collections.Generic;

public class FormValidatorTMP : MonoBehaviour
{
    public TMP_InputField[] inputFields;  // All TMP_InputFields (across different questions)
    public TMP_Text[] charCountTexts;     // Character count texts for all input fields
    public Button submitButton;           // Submit button
    private const int MaxCharLimit = 500;

    // Popup panel for validation messages
    public GameObject popupPanel;
    public TMP_Text popupText;

    // First set of toggle questions (1 toggle with an input field for 2nd option)
    public Toggle[] firstToggleQuestionToggles; // First question toggle options
    public TMP_InputField additionalInputField; // Additional input field for second toggle option

    // Second set of 2 toggle questions
    public Toggle[] secondToggleQuestionToggles;

    // Third set of 18 toggle questions
    public Toggle[] thirdToggleQuestionToggles;

    // Token input field
    public TMP_InputField tokenInputField;

    void Start()
    {
        submitButton.onClick.AddListener(ValidateForm);

        // Add listeners for the first toggle question
        firstToggleQuestionToggles[0].onValueChanged.AddListener(OnFirstToggleChanged);
        firstToggleQuestionToggles[1].onValueChanged.AddListener(OnFirstToggleChanged);  // second toggle listener

        // Add listeners for other input fields and toggles as before
        for (int i = 0; i < inputFields.Length; i++)
        {
            int index = i;  // Local copy for lambda capture
            inputFields[i].onValueChanged.AddListener(delegate { UpdateCharCount(index); });
            UpdateCharCount(index); // Initialize with default count
        }

        // Initially set the input field visibility based on the first toggle's state
        OnFirstToggleChanged(firstToggleQuestionToggles[0].isOn);
    }

    void OnFirstToggleChanged(bool isOn)
    {
        // If the second toggle is selected (index 1), show the input field
        if (firstToggleQuestionToggles[1].isOn)
        {
            additionalInputField.gameObject.SetActive(true); // Show the input field
        }
        else
        {
            additionalInputField.gameObject.SetActive(false); // Hide the input field
        }
    }

    void UpdateCharCount(int index)
    {
        int currentLength = inputFields[index].text.Length;
        charCountTexts[index].text = currentLength + " / " + MaxCharLimit;
    }

    void ValidateForm()
    {
        // Validate all input fields (they cannot be empty)
        foreach (TMP_InputField field in inputFields)
        {
            if (string.IsNullOrWhiteSpace(field.text))
            {
                ShowPopup("All fields must be filled!"); // Show popup on validation failure
                return; // Prevent submission
            }
        }

        // Check if the first set of toggles is filled correctly (e.g., show input field when the second toggle is selected)
        string additionalAnswer = additionalInputField.text;
        if (firstToggleQuestionToggles[1].isOn && string.IsNullOrWhiteSpace(additionalAnswer))
        {
            ShowPopup("Please provide an answer for the second toggle option!"); // Show popup if the second toggle is selected but no input provided
            return;
        }

        // Validate second set of toggles
        int selectedSecondToggle = GetSelectedToggleValue(secondToggleQuestionToggles);
        if (selectedSecondToggle == -1)
        {
            ShowPopup("Please select an option from the second toggle question!");
            return;
        }

        // Validate third set of 18 toggles
        int selectedThirdToggle = GetSelectedToggleValue(thirdToggleQuestionToggles);
        if (selectedThirdToggle == -1)
        {
            ShowPopup("Please select an option from the third toggle question!");
            return;
        }

        // Collect form data
        FormData formData = new FormData
        {
            ans1 = GetFirstQuestionAnswer(),
            ans2 = selectedSecondToggle.ToString(),
            ans3 = selectedThirdToggle.ToString(),
            additionalAnswer = additionalAnswer,
            inputAnswers = GetConcatenatedInputText()
        };

        // Send form data to backend
        string token = tokenInputField.text.Trim();
        if (string.IsNullOrEmpty(token))
        {
            ShowPopup("Please enter a valid token!");
            return;
        }

        StartCoroutine(SendFeedbackData(formData, token));  // Send feedback with token
    }

    IEnumerator SendFeedbackData(FormData formData, string token)
    {
        string url = "https://lets-play-delta.vercel.app/feedback";
        string json = JsonUtility.ToJson(formData);  // Convert formData to JSON
        Debug.Log("Sending Data: " + json);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + token);  // Add the token to the Authorization header

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                ShowPopup("Failed to send feedback: " + request.error);
            }
            else
            {
                Debug.Log("Feedback submitted successfully: " + request.downloadHandler.text);
                ShowPopup("Feedback submitted successfully!");
            }
        }
    }

    // Function to get selected toggle value from a set of toggles (return -1 if no toggle is selected)
    int GetSelectedToggleValue(Toggle[] toggles)
    {
        for (int i = 0; i < toggles.Length; i++)
        {
            if (toggles[i].isOn)
            {
                return i + 1; // Return corresponding value (1 to n)
            }
        }
        return -1; // Return -1 if no toggle is selected
    }

    // Function to get answer for the first question
    string GetFirstQuestionAnswer()
    {
        if (firstToggleQuestionToggles[1].isOn) // Check if second toggle is selected
        {
            return additionalInputField.text; // Return the value from the additional input field
        }
        return "No additional input"; // Or some default answer
    }

    // Concatenate all input field texts for the final answer string
    string GetConcatenatedInputText()
    {
        List<string> inputTexts = new List<string>();

        foreach (TMP_InputField field in inputFields)
        {
            if (!string.IsNullOrWhiteSpace(field.text))
            {
                inputTexts.Add(field.text);
            }
        }

        return string.Join(" ", inputTexts);
    }

    // Function to show the popup
    void ShowPopup(string message)
    {
        popupPanel.SetActive(true);         // Show the popup
        popupText.text = message;           // Set the message in the popup

        // Hide the popup after 2 seconds
        Invoke("HidePopup", 2f);
    }

    // Function to hide the popup
    void HidePopup()
    {
        popupPanel.SetActive(false);        // Hide the popup
    }

    // Create a data structure to hold the form data
    [System.Serializable]
    public class FormData
    {
        public string ans1;  // The answer for the first question
        public string ans2;   // Selected value for the second toggle question (1 to n)
        public string ans3;    // Selected value for the third toggle question (1 to n)
        public string additionalAnswer;    // The additional answer input if the second toggle is selected
        public string inputAnswers;        // Concatenated string from all input fields
    }
}
