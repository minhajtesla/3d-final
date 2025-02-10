using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class PasswordValidation : MonoBehaviour
{
    public MenuManager menuManager;
    public TMP_InputField newPasswordField;  // Assign in Inspector
    public TMP_InputField confirmPasswordField;  // Assign in Inspector
    public TMP_Text errorText;  // Assign in Inspector (for displaying errors)
    public GameObject errorPanel;  // Assign in Inspector (Panel that shows errors)
    public Button submitButton;  // Assign in Inspector (Submit Button)

    void Start()
    {
        errorPanel.SetActive(false); // Hide error panel at start
        submitButton.onClick.AddListener(ConfirmPassword); // Attach the function to the button
    }

    public void ConfirmPassword()
    {
        string newPassword = newPasswordField.text;
        string confirmPassword = confirmPasswordField.text;

        // Check if fields are empty
        if (string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
        {
            ShowError("Fields cannot be empty!");
            return;
        }

        // Check if password is at least 6 characters
        if (newPassword.Length < 6)
        {
            ShowError("Password must be at least 6 characters!");
            return;
        }

        // Check if passwords match
        if (newPassword != confirmPassword)
        {
            ShowError("Passwords do not match!");
            return;
        }

        // If everything is correct
        ShowSuccess();
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
        errorText.text = "Password changed successfully!";
        menuManager.ShowPanel(menuManager.Onresetpassword);
        errorPanel.SetActive(true);
        StartCoroutine(HideErrorAfterDelay());
    }
}
