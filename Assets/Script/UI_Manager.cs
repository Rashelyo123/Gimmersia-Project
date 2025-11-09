using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI messageText;
    public Button yesButton;
    public Button noButton;

    private GameManager gm;

    void Start()
    {
        gm = FindObjectOfType<GameManager>();



    }

    public void ShowQuestion(int index)
    {
        questionText.text = $"Question {index}: Awaiting your response.";

    }

    public void ShowMessage(string msg)
    {
        messageText.color = Color.white;
        messageText.text = msg;

    }

    public void ShowWarning(string msg)
    {
        messageText.color = Color.red;
        messageText.text = msg;

    }


}
