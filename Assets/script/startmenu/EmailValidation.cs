using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class EmailValidation : MonoBehaviour
{
    
    public MenuManager menuManager;
    public TMP_InputField emailField;  // Assign in Inspector
    public TMP_Text errorText;  // Assign in Inspector (for displaying errors)
    public GameObject errorPanel;  // Assign in Inspector (Panel that shows errors)
    public Button submitButton;  // Assign in Inspector (Submit Button)

    void Start()
    {
        errorPanel.SetActive(false); // Hide error panel at start
        submitButton.onClick.AddListener(ValidateEmail); // Attach the function to the button
    }

    public void ValidateEmail()
    {
        string email = emailField.text;

        // Check if field is empty
        if (string.IsNullOrEmpty(email))
        {
            ShowError("Email field cannot be empty!");
            return;
        }

        // Check if email is valid
        if (!IsValidEmail(email))
        {
            ShowError("Invalid email address!");
            return;
        }

        // If everything is correct
        ShowSuccess();
    }

    bool IsValidEmail(string email)
    {
        // Simple regex for email validation
        string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, emailPattern);
    }

    void ShowError(string message)
    {
        errorText.text = message;
        errorPanel.SetActive(true);
        StartCoroutine(HideErrorAfterDelay());
    }

    IEnumerator HideErrorAfterDelay()
    {
        yield return new WaitForSeconds(1.5f);
        errorPanel.SetActive(false);
    }

    void ShowSuccess()
    {
        menuManager.ShowPanel(menuManager.resetPasswordPanel);
        errorText.text = "Email is valid!";
        errorPanel.SetActive(true);
        StartCoroutine(HideErrorAfterDelay());
    }
}