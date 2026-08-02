using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private GameObject pausePanel;
    private bool openedFromPause;
    public bool IsOpen => tutorialPanel.activeSelf;

    
    private void Start()
    {
        if (!PlayerPrefs.HasKey("TutorialSeen"))
        {
            ShowTutorial();
            openedFromPause = false;
            Time.timeScale = 0f; 
            PlayerPrefs.SetInt("TutorialSeen", 1);
        }
    }

    public void ShowTutorial()
    {
        tutorialPanel.SetActive(true);
    }

    public void CloseTutorial()
    {
        tutorialPanel.SetActive(false);

        if (openedFromPause)
        {
            pausePanel.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f; 
        }   

        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);    
    }

    public void OpenTutorialFromPause()
    {
        pausePanel.SetActive(false);
        tutorialPanel.SetActive(true);
        openedFromPause = true;
    }
}
