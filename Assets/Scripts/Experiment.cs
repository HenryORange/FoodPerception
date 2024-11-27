using System.Diagnostics;
using UnityEngine;

public class Experiment : MonoBehaviour
{
    public int participantID;
    public GameObject blackRoom;
    public GameObject questionnaire;
    public GameObject startButton;
    private Stopwatch _stopwatch = new Stopwatch();
    private int _state = 0;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        blackRoom.SetActive(false);
        questionnaire.SetActive(false);
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
                if (_stopwatch.ElapsedMilliseconds < 1000) return;
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
