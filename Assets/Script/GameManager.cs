using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum GameState { StartLevel, Listening, AnswerQuestion, CheckAnswer, EndLevel }

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    public GameState currentState = GameState.StartLevel;
    public int maxQuestions = 5;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip[] questionAudios;
    [Header("Gameplay Mode")]
    public bool isTutorial = true;

    [Header("UI Elements")]
    [SerializeField] private Button retryButton;
    [SerializeField] private Button returnButton;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject Jumpscare;

    [SerializeField] private CanvasGroup blackScreen;


    [Header("Answer Settings")]
    public bool[] correctAnswers = new bool[5] { true, false, true, true, false };

    [Header("References")]
    private UI_Manager ui;
    private PaperUIManager paperUI;

    // Tracking
    private bool[] answered;
    private bool[] correctAnswerGiven;
    private int currentAudioIndex = 0;
    private int lastReadIndex = -1;
    private bool allAudioPlayed = false;
    private bool gameOver = false;


    void Start()
    {
        ui = FindObjectOfType<UI_Manager>();
        paperUI = FindObjectOfType<PaperUIManager>();

        answered = new bool[maxQuestions];
        correctAnswerGiven = new bool[maxQuestions];
        if (isTutorial)
            paperUI.LockAllExceptFirst();
        else paperUI.UnlockAllPrompts();

        StartCoroutine(PlayAllQuestions());
    }


    IEnumerator PlayAllQuestions()
    {
        ui.ShowMessage("Audio log started...");

        for (currentAudioIndex = 0; currentAudioIndex < maxQuestions; currentAudioIndex++)
        {
            AudioClip clip = questionAudios[currentAudioIndex];
            currentState = GameState.Listening;

            ui.ShowMessage($"Playing Question {currentAudioIndex + 1}...");
            audioSource.clip = clip;
            audioSource.Play();


            yield return new WaitForSeconds(clip.length);


            lastReadIndex = currentAudioIndex;

            yield return new WaitForSeconds(1f);
        }


        allAudioPlayed = true;
        StartCoroutine(WaitForCompletion());
    }


    public void SubmitAnswer(int questionIndex, bool isYes)
    {
        if (gameOver) return;

        answered[questionIndex] = true;
        bool correct = correctAnswers[questionIndex];
        correctAnswerGiven[questionIndex] = (isYes == correct);

        if (isTutorial && questionIndex == 0)
        {
            if (!correctAnswerGiven[questionIndex])
            {
                ui.ShowWarning("Incorrect answer for Question 1! Next Question.");
                isTutorial = false;
                paperUI.UnlockAllPrompts();
            }
            else
            {
                ui.ShowMessage("Response for Question 1 recorded. You may now answer the next question.");
                isTutorial = false;
                paperUI.UnlockAllPrompts();
            }
            return;
        }
        if (!correct)
        {
            ui.ShowWarning($"Incorrect answer for Question {questionIndex + 1}!");
        }
        else
        {
            ui.ShowMessage($"Response for Question {questionIndex + 1} recorded.");
        }
    }


    public void CheckForJumpscareOnClose()
    {
        if (gameOver) return;

        for (int i = 0; i < maxQuestions; i++)
        {

            if (answered[i] && !correctAnswerGiven[i] && i <= currentAudioIndex)
            {
                TriggerJumpscare($"Entity detected anomaly on Question {i + 1}...");
                return;
            }
        }
    }


    IEnumerator WaitForCompletion()
    {
        yield return new WaitForSeconds(5f);


        for (int i = 0; i < maxQuestions; i++)
        {
            if (!answered[i])
            {
                TriggerJumpscare("You failed to complete all prompts!");
                yield break;
            }
        }


        for (int i = 0; i < maxQuestions; i++)
        {
            if (!correctAnswerGiven[i])
            {
                TriggerJumpscare("Incorrect responses detected...");
                yield break;
            }
        }

        EndLevel();
    }


    void EndLevel()
    {
        if (gameOver) return;
        gameOver = true;
        currentState = GameState.EndLevel;
        ui.ShowMessage("Audio log ended. Salvage complete. You survived.");
    }

    void TriggerJumpscare(string reason)
    {
        if (gameOver) return;
        gameOver = true;
        currentState = GameState.EndLevel;

        ui.ShowWarning(reason);
        Debug.Log("💀 Jumpscare Triggered: " + reason);
        StartCoroutine(FadeToBlackAndGameOver());

        // add animation Jumpscare here
    }

    IEnumerator FadeToBlackAndGameOver()
    {
        if (Jumpscare)
        {
            Jumpscare.SetActive(true);
        }
        yield return new WaitForSeconds(2f);


        float t = 0f;
        if (blackScreen)
        {
            while (t < 1f)
            {
                t += Time.deltaTime * 0.6f;
                blackScreen.alpha = Mathf.Lerp(0, 1, t);


                if (t < 0.2f && Random.value > 0.5f)
                    blackScreen.alpha = 1f;

                yield return null;
            }
        }


        yield return new WaitForSeconds(1f);


        if (gameOverPanel)
            gameOverPanel.SetActive(true);




        yield return new WaitForSeconds(1f);

        if (retryButton)
        {
            retryButton.gameObject.SetActive(true);
            retryButton.onClick.AddListener(() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex));
        }

        if (returnButton)
        {
            returnButton.gameObject.SetActive(true);
            returnButton.onClick.AddListener(() => SceneManager.LoadScene("MainMenu"));
        }


    }

}
