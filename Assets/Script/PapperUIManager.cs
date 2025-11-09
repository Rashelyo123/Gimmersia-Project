using UnityEngine;
using UnityEngine.UI;

public class PaperUIManager : MonoBehaviour
{
    [Header("Paper Elements")]
    public Toggle[] yesToggles;   // Toggle YES untuk setiap prompt
    public Toggle[] noToggles;    // Toggle NO untuk setiap prompt

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


            if (!paperAnimator.IsOpen())
            {
                gm.CheckForJumpscareOnClose();
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
