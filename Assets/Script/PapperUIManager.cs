using UnityEngine;
using UnityEngine.UI;

public class PaperUIManager : MonoBehaviour
{
    [Header("Paper Elements")]
    public Toggle[] yesToggles;
    public Toggle[] noToggles;
    private bool suppressEvents = false;


    private GameManager gm;
    private PaperAnimator paperAnimator;

    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        paperAnimator = FindObjectOfType<PaperAnimator>();

        for (int i = 0; i < yesToggles.Length; i++)
        {
            yesToggles[i].isOn = false;
            noToggles[i].isOn = false;
            yesToggles[i].interactable = true;
            noToggles[i].interactable = true;
            yesToggles[i].navigation = new Navigation { mode = Navigation.Mode.None };
            noToggles[i].navigation = new Navigation { mode = Navigation.Mode.None };

            int index = i;
            yesToggles[i].onValueChanged.AddListener((val) => OnCheckboxSelected(index, true));
            noToggles[i].onValueChanged.AddListener((val) => OnCheckboxSelected(index, false));
        }
    }

    public void TogglePaperExternally()
    {
        // Tutup noted kalau sedang terbuka
        if (gm != null && gm.notedPage != null && gm.notedPage.IsOpen())
        {
            gm.notedPage.ForceClose();
        }

        paperAnimator.TogglePaper();

        if (!paperAnimator.IsOpen() && gm != null && !gm.isTutorial)
        {
            gm.CheckForJumpscareOnClose();
        }
    }


    public void OnCheckboxSelected(int index, bool isYes)
    {
        if (suppressEvents) return;
        if (!(isYes ? yesToggles[index].isOn : noToggles[index].isOn)) return;

        suppressEvents = true;
        if (isYes) noToggles[index].isOn = false;
        else yesToggles[index].isOn = false;
        suppressEvents = false;

        gm.SubmitAnswer(index, isYes);
    }

    public void LockAllExceptFirst()
    {
        for (int i = 0; i < yesToggles.Length; i++)
        {
            bool first = (i == 0);
            yesToggles[i].interactable = first;
            noToggles[i].interactable = first;

            // Hapus dua baris ini ⛔
            // yesToggles[i].isOn = false;
            // noToggles[i].isOn = false;
        }
    }


    public void UnlockPrompt(int index)
    {
        if (index >= 0 && index < yesToggles.Length)
        {
            yesToggles[index].interactable = true;
            noToggles[index].interactable = true;
        }
    }

    public void UnlockAllPrompts()
    {
        for (int i = 0; i < yesToggles.Length; i++)
        {
            yesToggles[i].interactable = true;
            noToggles[i].interactable = true;
        }
    }

    public void ResetPaper()
    {
        for (int i = 0; i < yesToggles.Length; i++)
        {
            yesToggles[i].isOn = false;
            noToggles[i].isOn = false;
            yesToggles[i].interactable = true;
            noToggles[i].interactable = true;
        }
    }
    public void DisableAllPrompts()
    {
        for (int i = 0; i < yesToggles.Length; i++)
        {
            yesToggles[i].interactable = false;
            noToggles[i].interactable = false;
        }
    }

}
