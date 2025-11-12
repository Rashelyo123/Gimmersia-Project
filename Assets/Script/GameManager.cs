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
    public int maxQuestions = 4;
    [Header("Recording Notes")]
    public GameObject notedPageUI;
    public GameObject[] nextNotedPage;


    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip TutorialAudio;
    public AudioClip recordNotedAudio;
    public AudioClip playerNotAnswerAudio;
    public AudioClip endLevelAudio;
    public AudioClip[] questionAudios;

    [Header("UI Elements")]
    [SerializeField] private Button retryButton;
    [SerializeField] private Button returnButton;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject winPanel;
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

    [Header("Animators")]
    public PaperAnimator paperAnimator;
    [Header("References")]
    public NotedPage notedPage;

    [Header("Ghost Settings")]
    public Animator ghostAnimator;




    private bool[] answered;
    private bool[] correctAnswerGiven;
    private int currentAudioIndex = 0;
    private int lastReadIndex = -1;
    private bool gameOver = false;

    private bool tutorialFinished = false;
    private bool paperOpened = false;

    void Start()
    {
        ui = FindObjectOfType<UI_Manager>();
        paperUI = FindObjectOfType<PaperUIManager>();

        answered = new bool[maxQuestions];
        correctAnswerGiven = new bool[maxQuestions];

        if (isTutorial)
            paperUI.DisableAllPrompts();
        else
            paperUI.UnlockAllPrompts();

        StartCoroutine(PlayTutorial());
    }

    void Update()
    {
        if (gameOver) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {

            paperUI.TogglePaperExternally();

            if (isTutorial && !paperOpened)
            {
                paperOpened = true;
                // ui.ShowMessage("Paper opened. All checkboxes disabled...");


            }
        }

    }


    private IEnumerator PlayTutorial()
    {
        if (isTutorial)
        {


            audioSource.clip = TutorialAudio;
            audioSource.Play();
            //  ui.ShowMessage("VA Tutorial playing...");

            // Tunggu sampai audio tutorial selesai
            yield return new WaitForSeconds(TutorialAudio.length);
            audioSource.clip = recordNotedAudio;
            audioSource.Play();
            notedPageUI.SetActive(true);
            yield return new WaitForSeconds(recordNotedAudio.length);

            audioSource.Stop();
            //  ui.ShowMessage("Tutorial finished. Starting first question...");

            // Mulai pertanyaan pertama otomatis
            paperUI.LockAllExceptFirst();
            StartCoroutine(StartFirstQuestion());
        }
    }



    private IEnumerator StartFirstQuestion()
    {
        paperUI.DisableAllPrompts();

        AudioClip clip = questionAudios[0];
        audioSource.clip = clip;
        audioSource.Play();
        //ui.ShowMessage("VA Question 1 playing...");

        yield return new WaitForSeconds(clip.length);
        AudioClip recordClip = recordNotedAudio;
        audioSource.clip = recordClip;
        audioSource.Play();
        yield return new WaitForSeconds(recordClip.length);
        nextNotedPage[0].SetActive(true);

        lastReadIndex = 0;
        paperUI.UnlockPrompt(0);
    }


    private IEnumerator PlayAllQuestions()
    {
        // ui.ShowMessage("Audio log started...");

        for (currentAudioIndex = 1; currentAudioIndex < maxQuestions; currentAudioIndex++)
        {
            AudioClip clip = questionAudios[currentAudioIndex];
            currentState = GameState.Listening;
            //  ui.ShowMessage($"Playing Question {currentAudioIndex + 1}...");
            audioSource.clip = clip;
            audioSource.Play();
            yield return new WaitForSeconds(clip.length);
            lastReadIndex = currentAudioIndex;
            if (recordNotedAudio != null)
            {
                if (currentAudioIndex < nextNotedPage.Length)
                {
                    nextNotedPage[currentAudioIndex].SetActive(true);
                }



                audioSource.clip = recordNotedAudio;
                audioSource.Play();
                //ui.ShowMessage("Record note has been updated...");
                yield return new WaitForSeconds(recordNotedAudio.length);
            }
            yield return new WaitForSeconds(1f);
        }

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
                //    ui.ShowWarning("Incorrect - Entity detected minor movement.");
                StartCoroutine(CompleteTutorial());
            }
            else
            {
                // ui.ShowMessage("Tutorial complete. You may now proceed.");
                StartCoroutine(CompleteTutorial());
            }
            return;
        }

        if (!correct)
            // ui.ShowWarning($"Incorrect answer for Question {questionIndex + 1}!");
            Debug.LogWarning($"Incorrect answer for Question {questionIndex + 1}!");
        else
            // ui.ShowMessage($"Response for Question {questionIndex + 1} recorded.");
            Debug.Log($"Response for Question {questionIndex + 1} recorded.");
    }

    private IEnumerator CompleteTutorial()
    {
        yield return new WaitForSeconds(2f);
        // ui.ShowMessage("Tutorial complete. Proceeding to next questions...");
        isTutorial = false;
        paperUI.UnlockAllPrompts();
        StartCoroutine(PlayAllQuestions());
    }

    public void CheckForJumpscareOnClose()
    {
        if (gameOver || isTutorial) return;

        for (int i = 0; i < maxQuestions; i++)
        {
            if (i == 0) continue;
            if (answered[i] && !correctAnswerGiven[i] && i <= currentAudioIndex)
            {
                TriggerJumpscare();
                return;
            }
        }
    }

    private IEnumerator WaitForCompletion()
    {
        yield return new WaitForSeconds(5f);

        for (int i = 0; i < maxQuestions; i++)
        {
            if (!answered[i])
            {
                audioSource.clip = playerNotAnswerAudio;
                audioSource.Play();
                yield return new WaitForSeconds(playerNotAnswerAudio.length);
                TriggerJumpscare();
                yield break;
            }
        }

        for (int i = 1; i < maxQuestions; i++)
        {
            if (!correctAnswerGiven[i])
            {
                TriggerJumpscare();
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
        StartCoroutine(WinGame());

        // ui.ShowMessage("Audio log ended. Salvage complete. You survived.");
    }

    private IEnumerator WinGame()
    {
        AudioClip clip = endLevelAudio;
        audioSource.clip = clip;
        audioSource.Play();
        yield return new WaitForSeconds(clip.length);
        StartCoroutine(fadeToBlackAndWin());

    }
    void TriggerJumpscare()
    {
        if (gameOver) return;

        gameOver = true;
        currentState = GameState.EndLevel;


        StopCoroutine("PlayAllQuestions");
        StopCoroutine("WaitForCompletion");
        StopCoroutine("StartFirstQuestion");


        audioSource.Stop();
        audioSource.enabled = false;

        //ui.ShowWarning(reason);


        StartCoroutine(FadeToBlackAndGameOver());
    }


    private IEnumerator FadeToBlackAndGameOver()
    {
        if (Jumpscare)
        {
            yield return new WaitForSeconds(1f);
            ghostAnimator.SetTrigger("Jumpscare");
            yield return new WaitForSeconds(0.3f);
            Jumpscare.SetActive(true);
        }

        if (videoPlayer)
        {
            videoPlayer.Play();
            yield return new WaitForSeconds((float)videoPlayer.clip.length);
        }
        else
        {
            yield return new WaitForSeconds(2.5f);
        }

        blackScreen.SetActive(true);
        yield return new WaitForSeconds(1f);

        if (gameOverPanel)
            gameOverPanel.SetActive(true);

        yield return new WaitForSeconds(1f);

        if (retryButton)
        {
            retryButton.gameObject.SetActive(true);
            retryButton.onClick.RemoveAllListeners();
            retryButton.onClick.AddListener(() =>
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex)
            );
        }

        if (returnButton)
        {
            returnButton.gameObject.SetActive(true);
            returnButton.onClick.RemoveAllListeners();
            returnButton.onClick.AddListener(() =>
                SceneManager.LoadScene("MainMenu")
            );
        }
    }
    private IEnumerator fadeToBlackAndWin()
    {
        blackScreen.SetActive(true);
        yield return new WaitForSeconds(1f);

        if (winPanel)
            winPanel.SetActive(true);

        yield return new WaitForSeconds(1f);

        if (returnButton)
        {
            returnButton.gameObject.SetActive(true);
            returnButton.onClick.RemoveAllListeners();
            returnButton.onClick.AddListener(() =>
                SceneManager.LoadScene("MainMenu")
            );
        }
    }
}
