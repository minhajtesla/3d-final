/*using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FormValidatorTMP : MonoBehaviour
{
    public TMP_InputField[] inputFields;  // Assign all TMP_InputFields in the Inspector
    public TMP_Text[] charCountTexts;     // Assign TMP_Text for character count display
    public Button submitButton;           // Assign the submit button
    private const int maxCharLimit = 500;

    void Start()
    {
        submitButton.onClick.AddListener(ValidateForm);

        for (int i = 0; i < inputFields.Length; i++)
        {
            inputFields[i].characterLimit = maxCharLimit;
            int index = i; // Local copy for lambda capture
            inputFields[i].onValueChanged.AddListener(delegate { UpdateCharCount(index); });
            UpdateCharCount(index); // Initialize with default count
        }
    }

    void UpdateCharCount(int index)
    {
        int currentLength = inputFields[index].text.Length;
        charCountTexts[index].text = currentLength + " / " + maxCharLimit;
    }

    void ValidateForm()
    {
        foreach (TMP_InputField field in inputFields)
        {
            if (string.IsNullOrWhiteSpace(field.text))
            {
                Debug.Log("All fields must be filled!");
                return; // Prevent submission
            }
        }

        Debug.Log("Form submitted successfully!");
        // Proceed with form submission logic here
    }
}
*/