using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // Assign these in Unity Inspector
    public GameObject feesback1;
    public GameObject feesback2;
    public GameObject feesback3;
    public GameObject mainMenuPanel;       // MMB02 - New Game / Load Game
    public GameObject newGamePanel;        // MMB03 - Quick Start / Sign Up
    public GameObject registrationPanel;   // MMB04 - Sign Up Page
    public GameObject verificationPanel;   // MMB05 - Successfully Verified (Sign In Button)
    public GameObject loginPanel;          // MMB06 - Login Page
    public GameObject forgotPasswordPanel; // MMB07 - Forgot Password
    public GameObject resetPasswordPanel;  // MMB08 - Reset Password
    public GameObject startGamePanel;      // MMB10 - Start or Load Game
    public GameObject ONConfirmationemaipassweord;
    public GameObject Onresetpassword;
    public GameObject mainMenuPanel2;
    public GameObject gameover;

    public Button newGameButton;
    public Button loadGameButton;
    public Button quickStartButton;
    public Button signUpButton;
    public Button signInButton;
    public Button forgotPasswordButton;
    public Button passwordResetButton;
    public Button backButton;
    public Button Onconfirmpassemailbutton;
    public Button confirmRegistrationButton;
    public Button confirmSignButton;
    public Button Onresetpasswordbutton;
    public Button feedback1;
    public Button feedback2;
    void Start()
    {
        // Attach button event listeners
        

        if (newGameButton) newGameButton.onClick.AddListener(() => ShowPanel(newGamePanel));
        if (loadGameButton) loadGameButton.onClick.AddListener(() => ShowPanel(loginPanel));
        if (quickStartButton) quickStartButton.onClick.AddListener(StartGame);
        //if (signUpButton)
        //{
        //    if (confirmRegistrationButton)
        //        confirmRegistrationButton.onClick.AddListener(() => ShowPanel(verificationPanel));
        //}
signUpButton.onClick.AddListener(() => ShowPanel(registrationPanel));
        //if (signInButton) signInButton.onClick.AddListener(() => ShowPanel(loginPanel));
        if (forgotPasswordButton) forgotPasswordButton.onClick.AddListener(() => ShowPanel(forgotPasswordPanel));
        //if (passwordResetButton) passwordResetButton.onClick.AddListener(() => ShowPanel());
        //if(confirmSignButton) confirmSignButton.onClick.AddListener(() => ShowPanel(mainMenuPanel2));
        //if (Onconfirmpassemailbutton) Onconfirmpassemailbutton.onClick.AddListener(() => ShowPanel(resetPasswordPanel));
        //if (Onresetpasswordbutton) Onresetpasswordbutton.onClick.AddListener(() => ShowPanel(Onresetpassword));
        if(feedback1) feedback1.onClick.AddListener(() => ShowPanel(feesback2));
        if (feedback2) feedback2.onClick.AddListener(() => ShowPanel(feesback3));
        // Initialize with Main Menu active
        ShowPanel(mainMenuPanel);
    }

    // Function to
    // show only the selected panel
    public void ShowPanel(GameObject panelToShow)
    {
        // Disable all panels
        mainMenuPanel.SetActive(false);
        newGamePanel.SetActive(false);
        registrationPanel.SetActive(false);
        verificationPanel.SetActive(false);
        loginPanel.SetActive(false);
        forgotPasswordPanel.SetActive(false);
        resetPasswordPanel.SetActive(false);
        startGamePanel.SetActive(false);
        Onresetpassword.SetActive(false);
        mainMenuPanel2.SetActive(false);

        // Activate the selected panel
        panelToShow.SetActive(true);
    }

    void StartGame()
    {
        // Load the actual game scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene 2");
    }
    public void signin()
    {
        ShowPanel(loginPanel);
    }
    public void signup()
    {
        ShowPanel(registrationPanel);
    }  
    public void playgame()
    {
        SceneManager.LoadScene("SampleScene 2");
    }

}

