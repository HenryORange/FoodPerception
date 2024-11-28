using System;
using System.Collections.Generic;
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
    private GameObject _referenceCube;
    private GameObject _takeInCanvas;
    private GameObject _tasteCanvas;
    private GameObject _timerCanvas;
    private TextMeshProUGUI _timerText;
    private Image _timerFill;
    private Stopwatch _stopwatch = new Stopwatch();
    private State _state;

    // sugar level, visual, auditory
    private int[,] _conditions = 
    {
        {0,0,0}, {0,0,1}, {0,0,2},
        {0,1,0}, {0,1,1}, {0,1,2},
        {0,2,0}, {0,2,1}, {0,2,2},
        {1,0,0}, {1,0,1}, {1,0,2},
        {1,1,0}, {1,0,1}, {1,0,2},
        {1,2,0}, {1,2,1}, {1,2,2}
    };
    
    private enum State
    {
        BlackRoom,
        TakeIn,
        Taste,
        Rate
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        blackRoom.SetActive(false);
        questionnaire.SetActive(false);
        _timerText = GameObject.Find("Timer Text").GetComponent<TextMeshProUGUI>();
        _timerFill = GameObject.Find("Timer Fill").GetComponent<Image>();
        _referenceCube = GameObject.Find("Reference Cube");
        _timerCanvas = GameObject.Find("Timer Canvas");
        _timerCanvas.SetActive(false);
        _takeInCanvas = GameObject.Find("Take In Canvas");
        _takeInCanvas.SetActive(false);
        _tasteCanvas = GameObject.Find("Taste Canvas");
        _tasteCanvas.SetActive(false);
    }

    public void StartExperiment()
    {
        startButton.SetActive(false);
        questionnaire.SetActive(false);
        _referenceCube.SetActive(false);
        blackRoom.SetActive(true);
        _timerCanvas.SetActive(true);
        _state = State.BlackRoom;
        _stopwatch.Start();
    }
    
    // Update is called once per frame
    void Update()
    {
        print(_stopwatch.ElapsedMilliseconds);
        switch (_state)
        {
            case State.BlackRoom:
                if (_stopwatch.ElapsedMilliseconds < 1000)
                {
                    UpdateTimer(15);
                    break;
                }
                blackRoom.SetActive(false);
                _state = State.TakeIn;
                _takeInCanvas.SetActive(true);

                _stopwatch.Restart();
                break;
            case State.TakeIn:
                if (_stopwatch.ElapsedMilliseconds < 1000)
                {
                    UpdateTimer(10);
                    break;
                }

                _takeInCanvas.SetActive(false);
                _state = State.Taste;
                _tasteCanvas.SetActive(true);
                
                _stopwatch.Restart();
                break;
            case State.Taste:
                if (_stopwatch.ElapsedMilliseconds < 1000)
                {
                    UpdateTimer(15);
                    break;
                }
                blackRoom.SetActive(true);
                _state = State.Rate;
                questionnaire.SetActive(true);
                _timerCanvas.SetActive(false);
                _tasteCanvas.SetActive(false);
                break;
        }
    }

    private void UpdateTimer(int length)
    {
        _timerText.text = (length-_stopwatch.Elapsed.Seconds).ToString();
        _timerFill.fillAmount = 1 - _stopwatch.Elapsed.Seconds / (float) length;
    }

    private List<int[]> BalancedLatinSquare()
    {
        var results = new List<int[]>();
        var j = 0;
        var k = 0;
        for (var i = 0; i < _conditions.GetLength(0); i++)
        {
            var val = 0;
            if (i < 2 || i % 2 != 0)
            {
                val = j++;
            }
            else
            {
                val = _conditions.GetLength(0) - k - 1;
                ++k;
            }
            var idx = (val + participantID) % _conditions.GetLength(0);
            results.Add((int[])_conditions.GetValue(idx));
        }

        if (_conditions.GetLength(0) % 2 != 0 && participantID % 2 != 0)
        {
            results.Reverse();
        }
        return results;
    }
}
