using UnityEngine;

public class PaperAnimator : MonoBehaviour
{
    private bool isOpen = false;
    public Animator paperAnimator;



    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            TogglePaper();
            Debug.Log("Toggled Paper");
        }


    }

    public void TogglePaper()
    {
        isOpen = !isOpen;
        paperAnimator.SetBool("isOpen", isOpen);
    }

    public bool IsOpen()
    {
        return isOpen;
    }
}
