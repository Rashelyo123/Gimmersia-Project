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
    public AudioClip[] questionAudios; // Assign 5 clips di Inspector

    private UI_Manager ui;               // Untuk menampilkan teks notifikasi
    private PaperUIManager paperUI;     // Untuk interaksi checkbox

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

            // Setelah audio selesai → izinkan pemain menjawab lewat kertas
            Invoke("EnablePaperPrompt", audioSource.clip.length);
        }
    }

    void EnablePaperPrompt()
    {
        currentState = GameState.AnswerQuestion;
        ui.ShowMessage($"Document the result for Prompt {currentQuestion}.");
        // PaperUIManager sudah handle checkbox aktif/nonaktif
    }

    public void SubmitAnswer(bool isYes)
    {
        currentState = GameState.CheckAnswer;

        // Cek jawaban (contohnya: Prompt 1 aman, sisanya punya risiko)
        if (isTutorial && currentQuestion == 1)
        {
            if (!isYes)
                ui.ShowWarning("⚠ Incorrect - Entity detected minor movement.");

            NextQuestion();
            return;
        }

        // Misal untuk prototype — anggap jawaban benar itu 'yes'
        bool isCorrect = isYes;

        if (isCorrect)
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
        float chance = Random.value;

        if (chance < 0.4f)
        {
            ui.ShowWarning("⚠ Entity becomes hostile!");
            // TODO: Trigger jumpscare animation or sound
        }
        else
        {
            ui.ShowWarning("Entity moved closer...");
            NextQuestion();
        }
    }
}
