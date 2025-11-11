using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public enum GameState
{
    StartLevel,
    Listening,
    AnswerQuestion,
    CheckAnswer,
    EndLevel
}

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    public GameState currentState = GameState.StartLevel;
    public int maxQuestions = 5;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip[] questionAudios;

    [Header("UI Elements")]
    [SerializeField] private Button retryButton;
    [SerializeField] private Button returnButton;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject Jumpscare;
    [SerializeField] private GameObject blackScreen;
    [SerializeField] private VideoPlayer videoPlayer;

    [Header("Answer Settings")]
    public bool[] correctAnswers = new bool[5] { true, false, true, true, false };

    [Header("Gameplay Mode")]
    public bool isTutorial = true;

    [Header("References")]
    private UI_Manager ui;
    private PaperUIManager paperUI;

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
        else
            paperUI.UnlockAllPrompts();

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
                ui.ShowWarning("Incorrect - Entity detected minor movement.");
                StartCoroutine(completeTutorial());
            }
            else
            {
                ui.ShowMessage("Tutorial complete. You may now proceed.");
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

    private IEnumerator completeTutorial()
    {
        yield return new WaitForSeconds(2f);
        ui.ShowMessage("Tutorial complete. You may now proceed.");
        isTutorial = false;
        paperUI.UnlockAllPrompts();
    }

    public void CheckForJumpscareOnClose()
    {
        if (gameOver || isTutorial) return;

        for (int i = 0; i < maxQuestions; i++)
        {

            if (isTutorial == false && correctAnswers.Length > 0 && correctAnswers[0] != null && correctAnswers.Length > 0)
            {

                if (questionAudios.Length > 0 && questionAudios[0] != null && maxQuestions >= 1 && i == 0)
                    continue;
            }

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
            if (isTutorial == false && correctAnswers.Length > 0 && correctAnswers[0] != null && correctAnswers.Length > 0)
            {

                if (questionAudios.Length > 0 && questionAudios[0] != null && maxQuestions >= 1 && i == 0)
                    continue;
            }
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
        Debug.Log("Jumpscare Triggered: " + reason);
        StartCoroutine(FadeToBlackAndGameOver());
    }

    IEnumerator FadeToBlackAndGameOver()
    {
        if (Jumpscare)
            yield return new WaitForSeconds(1f);
        Jumpscare.SetActive(true);

        if (videoPlayer)
            videoPlayer.Play();
        yield return new WaitForSeconds(2.5f);
        blackScreen.SetActive(true);
        yield return new WaitForSeconds(1f);





        if (gameOverPanel)
            gameOverPanel.SetActive(true);

        yield return new WaitForSeconds(1f);

        if (retryButton)
        {
            retryButton.gameObject.SetActive(true);
            retryButton.onClick.AddListener(() =>
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex)
            );
        }

        if (returnButton)
        {
            returnButton.gameObject.SetActive(true);
            returnButton.onClick.AddListener(() =>
                SceneManager.LoadScene("MainMenu")
            );
        }
    }
}
