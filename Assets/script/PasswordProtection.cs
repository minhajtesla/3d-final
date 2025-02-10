using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PasswordProtection : MonoBehaviour
{
    
    public TMP_InputField passwordInputField;
    public TMP_Text feedbackText; 

    
    private string correctPassword = "bauhof";

    // Start is called before the first frame update
    void Start()
    {
        feedbackText.color = Color.white;
        feedbackText.text = "Enter password!";
    }

    // Method to check the entered password
    public void OnSubmitPassword()
    {
        
        string enteredPassword = passwordInputField.text;

        if (enteredPassword == correctPassword)
        {
            feedbackText.text = "Access Granted!";
            feedbackText.color = Color.green;
            feedbackText.gameObject.SetActive(true);

            SceneManager.LoadScene("SampleScene 2");  
        }
        else
        {
            
            feedbackText.text = "Incorrect Password. Try Again.";
            feedbackText.color = Color.red;
            feedbackText.gameObject.SetActive(true);
        }
    }
}
