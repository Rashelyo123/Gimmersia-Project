using UnityEngine;

public enum GameState { StartLevel, Listening, AnswerQuestion, CheckAnswer, EndLevel }

public class GameManager : MonoBehaviour
{
    public GameState currentState = GameState.StartLevel;
    public int currentQuestion = 0;
    public int maxQuestions = 5;
    public bool isTutorial = true;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip[] questionAudios;
    [Header("Answer Settings")]
    public bool[] correctAnswers = new bool[5] { true, false, true, true, false };

    private UI_Manager ui;
    private PaperUIManager paperUI;

    void Start()
    {
        ui = FindObjectOfType<UI_Manager>();
        paperUI = FindObjectOfType<PaperUIManager>();
        StartLevel();
    }

    void StartLevel()
    {
        currentState = GameState.Listening;
        currentQuestion = 1;
        ui.ShowMessage("Audio log started...");


        FindObjectOfType<PaperUIManager>().ResetPaper();

        Invoke("PlayQuestionAudio", 2f);
    }


    void PlayQuestionAudio()
    {
        if (currentQuestion <= maxQuestions)
        {
            audioSource.clip = questionAudios[currentQuestion - 1];
            audioSource.Play();
            currentState = GameState.Listening;
            ui.ShowMessage($"Playing Question {currentQuestion}...");


            Invoke("EnablePaperPrompt", audioSource.clip.length);
        }
    }

    void EnablePaperPrompt()
    {
        currentState = GameState.AnswerQuestion;
        ui.ShowMessage($"Document the result for Prompt {currentQuestion}.");

        FindObjectOfType<PaperUIManager>().UnlockPromptExternally(currentQuestion - 1);
    }

    public void SubmitAnswer(bool isYes)
    {
        currentState = GameState.CheckAnswer;


        bool correct = correctAnswers[currentQuestion - 1];

        if (isTutorial && currentQuestion == 1)
        {

            if (isYes != correct)
                ui.ShowWarning(" Incorrect - Entity detected minor movement.");

            NextQuestion();
            return;
        }

        if (isYes == correct)
        {
            ui.ShowMessage("Response recorded.");
            NextQuestion();
        }
        else
        {
            TriggerJumpscareChance();
        }
    }


    void NextQuestion()
    {
        currentQuestion++;
        if (currentQuestion > maxQuestions)
        {
            EndLevel();
        }
        else
        {
            ui.ShowMessage($"Preparing next audio prompt...");
            Invoke("PlayQuestionAudio", 2f);
        }
    }

    void EndLevel()
    {
        currentState = GameState.EndLevel;
        ui.ShowMessage("Audio log ended. Salvage complete.");
    }

    void TriggerJumpscareChance()
    {
        ui.ShowWarning(" Incorrect response!");
        // float chance = Random.value;

        // if (chance < 0.4f)
        // {
        //     ui.ShowWarning("⚠ Entity becomes hostile!");
        //     // TODO: Trigger jumpscare animation or sound
        // }
        // else
        // {
        //     ui.ShowWarning("Entity moved closer...");
        //     NextQuestion();
        // }
    }
}
