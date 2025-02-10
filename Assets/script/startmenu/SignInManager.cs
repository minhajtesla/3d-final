using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class SignInManager : MonoBehaviour
{
    public MenuManager menuManager;

    public TMP_InputField emailInputField;
    public TMP_InputField passwordInputField;
    public TMP_Text errorMessage;
    public Button signInButton;
    public Button forgotPasswordButton;

    void Start()
    {
        // Add button listeners
        signInButton.onClick.AddListener(ValidateSignIn);
        forgotPasswordButton.onClick.AddListener(() => menuManager.ShowPanel(menuManager.forgotPasswordPanel));
    }

    void ValidateSignIn()
    {
        string email = emailInputField.text.Trim();
        string password = passwordInputField.text.Trim();

        // Check if fields are empty
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            errorMessage.text = "Email and password are required!";
            errorMessage.gameObject.SetActive(true);
            return;
        }

        // Validate email format
        if (!IsValidEmail(email))
        {
            errorMessage.text = "Invalid email format!";
            errorMessage.gameObject.SetActive(true);
            return;
        }

        // Password length check (optional)
        if (password.Length < 6)
        {
            errorMessage.text = "Password must be at least 6 characters!";
            errorMessage.gameObject.SetActive(true);
            return;
        }

        // If everything is valid, proceed to sign-in (CRUD logic will be added later)
        Debug.Log("Sign-in successful! Email: " + email);
        errorMessage.text = "Sign-in successful!";
        errorMessage.color = Color.green;

        // Navigate to the next panel
        menuManager.ShowPanel(menuManager.mainMenuPanel2);
    }

    bool IsValidEmail(string email)
    {
        string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, pattern);
    }
}
