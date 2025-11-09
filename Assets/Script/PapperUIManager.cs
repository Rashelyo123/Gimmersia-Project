using UnityEngine;
using UnityEngine.UI;

public class PaperUIManager : MonoBehaviour
{
    public GameObject paperPanel;
    public Toggle[] yesToggles;  // array untuk yes (Prompt 1–5)
    public Toggle[] noToggles;   // array untuk no  (Prompt 1–5)

    private int unlockedPrompt = 1; // mulai dari prompt 1
    private GameManager gm;

    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        paperPanel.SetActive(false);

        // Disable semua prompt selain pertama
        for (int i = 1; i < yesToggles.Length; i++)
        {
            yesToggles[i].interactable = false;
            noToggles[i].interactable = false;
        }

        // Tambahkan listener ke setiap checkbox
        for (int i = 0; i < yesToggles.Length; i++)
        {
            int index = i;
            yesToggles[i].onValueChanged.AddListener((val) => OnCheckboxSelected(index, true));
            noToggles[i].onValueChanged.AddListener((val) => OnCheckboxSelected(index, false));
        }
    }

    void Update()
    {
        // Tekan E buat buka/tutup paper
        if (Input.GetKeyDown(KeyCode.E))
        {
            paperPanel.SetActive(!paperPanel.activeSelf);
        }
    }

    void OnCheckboxSelected(int index, bool isYes)
    {
        // Hanya proses jika toggle benar-benar diaktifkan
        if (!(isYes ? yesToggles[index].isOn : noToggles[index].isOn)) return;

        // Pastikan hanya satu pilihan aktif
        if (isYes)
            noToggles[index].isOn = false;
        else
            yesToggles[index].isOn = false;

        // Kirim ke GameManager
        bool answer = isYes;
        gm.SubmitAnswer(answer);

        // Jika benar, unlock prompt berikutnya
        if (gm.currentQuestion == index + 1 && gm.currentQuestion < gm.maxQuestions)
        {
            yesToggles[index + 1].interactable = true;
            noToggles[index + 1].interactable = true;
        }
    }
}
