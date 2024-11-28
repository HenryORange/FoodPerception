using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Experiment : MonoBehaviour
{
    public int participantID;
    public GameObject blackRoom;
    public GameObject questionnaire;
    public GameObject startButton;
    private TextMeshProUGUI _timerText;
    private Image _timerFill;
    private Stopwatch _stopwatch = new Stopwatch();
    private int _state = 0;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        blackRoom.SetActive(false);
        questionnaire.SetActive(false);
        _timerText = GameObject.Find("TimerText").GetComponent<TextMeshProUGUI>();
        _timerFill = GameObject.Find("TimerFill").GetComponent<Image>();
    }

    public void StartExperiment()
    {
        blackRoom.SetActive(true);
        startButton.SetActive(false);
        questionnaire.SetActive(false);
        _stopwatch.Start();
    }
    
    // Update is called once per frame
    void Update()
    {
        print(_stopwatch.ElapsedMilliseconds);
        switch (_state)
        {
            case 0:
                if (_stopwatch.ElapsedMilliseconds < 15000)
                {
                    _timerText.text = (15-_stopwatch.Elapsed.Seconds).ToString();
                    _timerFill.fillAmount = (float)(1 - (_stopwatch.Elapsed.Seconds / 15.0));
                    break;
                }
                blackRoom.SetActive(false);
                _state = 1;

                _stopwatch.Restart();
                break;
            case 1:
                if (_stopwatch.ElapsedMilliseconds < 1000) return;
                _stopwatch.Restart();
                _state = 2;
                break;
            case 2:
                if (_stopwatch.ElapsedMilliseconds < 1000) return;
                blackRoom.SetActive(true);
                _state = 0;
                _stopwatch.Restart();
                break;
        }
    }
}
