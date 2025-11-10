using UnityEngine;
using UnityEngine.UI;

public class PaperUIManager : MonoBehaviour
{
    [Header("Paper Elements")]
    public Toggle[] yesToggles;
    public Toggle[] noToggles;

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
        }


        for (int i = 0; i < yesToggles.Length; i++)
        {
            int index = i;
            yesToggles[i].onValueChanged.AddListener((val) => OnCheckboxSelected(index, true));
            noToggles[i].onValueChanged.AddListener((val) => OnCheckboxSelected(index, false));
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            paperAnimator.TogglePaper();

            // Hanya cek jumpscare kalau BUKAN tutorial
            if (!paperAnimator.IsOpen())
            {
                // Pastikan GameManager ada dan bukan di tutorial
                if (gm != null && !gm.isTutorial)
                {
                    gm.CheckForJumpscareOnClose();
                }
            }
        }

    }


    public void OnCheckboxSelected(int index, bool isYes)
    {

        if (!(isYes ? yesToggles[index].isOn : noToggles[index].isOn)) return;


        if (isYes) noToggles[index].isOn = false;
        else yesToggles[index].isOn = false;


        gm.SubmitAnswer(index, isYes);
    }

    public void LockAllExceptFirst()
    {
        for (int i = 0; i < yesToggles.Length; i++)
        {
            bool first = (i == 0);
            yesToggles[i].interactable = first;
            noToggles[i].interactable = first;
            yesToggles[i].isOn = false;
            noToggles[i].isOn = false;
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
}
