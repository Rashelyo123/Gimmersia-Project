using UnityEngine;
using UnityEngine.UI;

public class PaperUIManager : MonoBehaviour
{
    [Header("Paper Elements")]

    public Toggle[] yesToggles;
    public Toggle[] noToggles;

    private int unlockedPrompt = 0;
    private GameManager gm;

    void Start()
    {
        gm = FindObjectOfType<GameManager>();


        // Reset semua toggle
        for (int i = 0; i < yesToggles.Length; i++)
        {
            yesToggles[i].isOn = false;
            noToggles[i].isOn = false;
            yesToggles[i].interactable = false;
            noToggles[i].interactable = false;
        }

        UnlockPrompt(0); // hanya prompt pertama aktif

        // listener
        for (int i = 0; i < yesToggles.Length; i++)
        {
            int index = i;
            yesToggles[i].onValueChanged.AddListener((val) => OnCheckboxSelected(index, true));
            noToggles[i].onValueChanged.AddListener((val) => OnCheckboxSelected(index, false));
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            FindObjectOfType<PaperAnimator>().TogglePaper();
        }

    }

    public void UnlockPrompt(int index)
    {
        if (index < yesToggles.Length)
        {
            yesToggles[index].interactable = true;
            noToggles[index].interactable = true;
        }
    }

    public void LockPrompt(int index)
    {
        if (index < yesToggles.Length)
        {
            yesToggles[index].interactable = false;
            noToggles[index].interactable = false;
        }
    }

    public void OnCheckboxSelected(int index, bool isYes)
    {
        if (!(isYes ? yesToggles[index].isOn : noToggles[index].isOn)) return;

        // pastikan cuma satu pilihan
        if (isYes) noToggles[index].isOn = false;
        else yesToggles[index].isOn = false;

        // hanya boleh jawab prompt yang sedang aktif
        if (index != unlockedPrompt) return;

        // kirim jawaban ke GameManager
        gm.SubmitAnswer(isYes);

        // kunci prompt yang barusan dijawab
        LockPrompt(index);
    }

    public void ResetPaper()
    {
        unlockedPrompt = 0;
        for (int i = 0; i < yesToggles.Length; i++)
        {
            yesToggles[i].isOn = false;
            noToggles[i].isOn = false;
            yesToggles[i].interactable = false;
            noToggles[i].interactable = false;
        }
        UnlockPrompt(0);
    }

    // Fungsi baru – dipanggil dari GameManager saat audio pertanyaan baru selesai
    public void UnlockPromptExternally(int index)
    {
        unlockedPrompt = index;
        UnlockPrompt(index);
    }
}
