using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public TextMeshProUGUI recordingText;
    public TextMeshProUGUI playText;
    public TextMeshProUGUI debugMessageText;
    public Image recordingButton;
    public Image playButton;

    public Color redColor;
    public Color greenColor;
    public Color whiteColor;

    public GameObject playButtonObject;
    public GameObject firstTimeLoadObject;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void SwitchColorRed()
    {
        playButtonObject.SetActive(false);
        recordingButton.color = redColor;
        recordingText.text = "Recording!";
    }

    public void SwitchColorGreen()
    {
        playButton.color = greenColor;
        playText.text = "Playing";
    }

    public void PlayButtonBack()
    {
        playButtonObject.SetActive(true);
        recordingButton.color = whiteColor;
        recordingText.text = "Record Mouse Input";
    }

    public void StopEverything()
    {
        playButtonObject.SetActive(true);
        recordingText.text = "Record Mouse Input";
        playText.text = "Play Movements";
        recordingButton.color = whiteColor;
        playButton.color = whiteColor;
    }

    public void DebugMessage(string  msg)
    {
        debugMessageText.text = msg;
    }
}
