using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public interface ISubmitMessageTarget : IEventSystemHandler
{
    void OnSubmitMessage(List<int[]> results);
}

public class Experiment : MonoBehaviour, ISubmitMessageTarget
{
    public GameObject blackRoom;
    public GameObject questionnaire;
    public GameObject startButton;

    public Color defaultColor;
    public Color sweetColor;
    public Color bitterColor;
    private List<VolumeParameter<Color>> _visualLevel;
    public Volume volume;
    private ColorAdjustments _visualSource;

    private const AudioClip DefaultAudioCondition = null;
    public AudioClip sweetAudioCondition;
    public AudioClip bitterAudioCondition;
    private List<AudioClip> _audioLevel;
    private AudioSource _audioSource;

    private int participantID;
    private GameObject _referenceCube;
    private GameObject _takeInCanvas;
    private GameObject _tasteCanvas;
    private GameObject _neutralizeCanvas;
    private GameObject _breakCanvas;
    private GameObject _breakButton;
    private GameObject _endCanvas;
    private GameObject _timerCanvas;
    private TextMeshProUGUI _timerText;
    private Image _timerFill;
    private Stopwatch _stopwatch = new();
    private State _state;
    private int _currentCondition;
    private List<int[]> _conditionOrder;
    private StringBuilder _contentOfResults = new();

    private GameObject _settings;
    private int _blackRoomDuration;
    private int _takeInDuration;
    private int _tasteDuration;
    private int _breakFrequency;
    private int _breakDuration;
    private int _startAtCondition;

    private Stopwatch _buttonDown = new();

    // sugar level, visual, auditory
    private int[][] _conditions =
    {
        new[] { 0, 0, 0 }, new[] { 0, 0, 1 }, new[] { 0, 0, 2 },
        new[] { 0, 1, 0 }, new[] { 0, 1, 1 }, new[] { 0, 1, 2 },
        new[] { 0, 2, 0 }, new[] { 0, 2, 1 }, new[] { 0, 2, 2 },
        new[] { 1, 0, 0 }, new[] { 1, 0, 1 }, new[] { 1, 0, 2 },
        new[] { 1, 1, 0 }, new[] { 1, 0, 1 }, new[] { 1, 0, 2 },
        new[] { 1, 2, 0 }, new[] { 1, 2, 1 }, new[] { 1, 2, 2 }
    };

    private enum State
    {
        Idle,
        BlackRoom,
        TakeIn,
        Taste,
        Rate,
        Neutralize,
        Break
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
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
        _neutralizeCanvas = GameObject.Find("Neutralize Canvas");
        _neutralizeCanvas.SetActive(false);
        _breakCanvas = GameObject.Find("Break Canvas");
        _breakButton = GameObject.Find("Break Button");
        _breakButton.SetActive(false);
        _breakCanvas.SetActive(false);
        _endCanvas = GameObject.Find("End Canvas");
        _endCanvas.SetActive(false);
        _settings = GameObject.Find("Settings");
        _state = State.Idle;

        volume.profile.TryGet(out _visualSource);
        _audioSource = GameObject.Find("Audio Source").GetComponent<AudioSource>();

        _visualLevel = new List<VolumeParameter<Color>>{ new(), new(), new() };
        _visualLevel[0].value = defaultColor;
        _visualLevel[1].value = sweetColor;
        _visualLevel[2].value = bitterColor;
        _visualSource.colorFilter.SetValue(_visualLevel[0]);
        
        _audioLevel = new List<AudioClip> { DefaultAudioCondition, sweetAudioCondition, bitterAudioCondition };
    }

    public void StartExperiment()
    {
        participantID = GameObject.Find("ParticipantID").GetComponent<TMP_Dropdown>().value;
        _blackRoomDuration = (int)GameObject.Find("BlackRoomDuration").GetComponent<Slider>().value;
        _takeInDuration = (int)GameObject.Find("TakeInDuration").GetComponent<Slider>().value;
        _tasteDuration = (int)GameObject.Find("TasteDuration").GetComponent<Slider>().value;
        _breakFrequency = (int)GameObject.Find("BreakFrequency").GetComponent<Slider>().value;
        _breakDuration = (int)GameObject.Find("BreakDuration").GetComponent<Slider>().value;
        _startAtCondition = (int)GameObject.Find("StartAtCondition").GetComponent<Slider>().value;
        _settings.SetActive(false);

        startButton.SetActive(false);
        questionnaire.SetActive(false);
        _referenceCube.SetActive(false);
        blackRoom.SetActive(true);
        _timerCanvas.SetActive(true);

        _currentCondition = _startAtCondition;
        _conditionOrder = BalancedLatinSquare();

        _state = State.BlackRoom;
        _stopwatch.Start();
    }

    public void Reset()
    {
        blackRoom.SetActive(false);
        questionnaire.SetActive(false);
        _timerCanvas.SetActive(false);
        _takeInCanvas.SetActive(false);
        _tasteCanvas.SetActive(false);
        _breakButton.SetActive(false);
        _breakCanvas.SetActive(false);
        _endCanvas.SetActive(false);
        startButton.SetActive(true);
        _settings.SetActive(true);
        _referenceCube.SetActive(true);
        _stopwatch.Reset();
        _visualSource.colorFilter.SetValue(_visualLevel[0]);
        _audioSource.Stop();
        _state = State.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        OVRInput.Update();
        if (OVRInput.Get(OVRInput.Button.Two) || OVRInput.Get(OVRInput.Button.Four))
        {
            if (_buttonDown.ElapsedMilliseconds >= 3000)
            {
                Reset();
                _buttonDown.Restart();
            }
        }
        else
        {
            _buttonDown.Restart();
        }

        switch (_state)
        {
            case State.Idle: break;
            case State.BlackRoom:
                if (_stopwatch.ElapsedMilliseconds < _blackRoomDuration * 1000)
                {
                    UpdateTimer(_blackRoomDuration);
                    break;
                }

                blackRoom.SetActive(false);
                _state = State.TakeIn;
                _takeInCanvas.SetActive(true);

                _visualSource.colorFilter.SetValue(_visualLevel[_conditionOrder[_currentCondition][1]]);
                if (_conditionOrder[_currentCondition][2] != 0)
                {
                    _audioSource.clip = _audioLevel[_conditionOrder[_currentCondition][2]];
                    _audioSource.loop = true;
                    _audioSource.Play();
                }

                _stopwatch.Restart();
                break;
            case State.TakeIn:
                if (_stopwatch.ElapsedMilliseconds < _takeInDuration * 1000)
                {
                    UpdateTimer(_takeInDuration);
                    break;
                }

                _takeInCanvas.SetActive(false);
                _state = State.Taste;
                _tasteCanvas.SetActive(true);

                _stopwatch.Restart();
                break;
            case State.Taste:
                if (_stopwatch.ElapsedMilliseconds < _tasteDuration * 1000)
                {
                    UpdateTimer(_tasteDuration);
                    break;
                }

                blackRoom.SetActive(true);
                _state = State.Rate;
                questionnaire.SetActive(true);
                _timerCanvas.SetActive(false);
                _tasteCanvas.SetActive(false);

                _visualSource.colorFilter.SetValue(_visualLevel[0]);
                _audioSource.Stop();

                break;
            case State.Break:
                if (_stopwatch.ElapsedMilliseconds < _breakDuration * 1000 * 60)
                {
                    UpdateTimer(_breakDuration * 60);
                    break;
                }

                blackRoom.SetActive(true);
                _breakButton.SetActive(true);
                break;
            case State.Rate: break;
            case State.Neutralize: break;
        }
    }

    public void Continue()
    {
        _breakButton.SetActive(false);
        _breakCanvas.SetActive(false);
        _state = State.BlackRoom;
        _stopwatch.Restart();
    }

    public void ContinueCondition()
    {
        _neutralizeCanvas.SetActive(false);
        _timerCanvas.SetActive(true);
        blackRoom.SetActive(true);
        _state = State.BlackRoom;
        _stopwatch.Restart();
        if (_currentCondition % _breakFrequency != 0) return;
        _state = State.Break;
        blackRoom.SetActive(false);
        _breakCanvas.SetActive(true);
    }

    public void OnSubmitMessage(List<int[]> results)
    {
        SaveToCsv(results);
        questionnaire.SetActive(false);
        blackRoom.SetActive(false);
        _neutralizeCanvas.SetActive(true);
        _state = State.Neutralize;
        _currentCondition += 1;

        if (_currentCondition < _conditionOrder.Count) return;
        // end experiment
        startButton.SetActive(true);
        _referenceCube.SetActive(true);
        _settings.SetActive(true);
    }

    private void SaveToCsv(List<int[]> results)
    {
        var fileName = "participant_" + participantID.ToString("D2") + ".csv";

        var filePath = Path.Combine(Application.persistentDataPath, fileName);


        if (_currentCondition == 0)
        {
            var csvTitleRow = new string[11];
            csvTitleRow[0] = "Sugar";
            csvTitleRow[1] = "Visual";
            csvTitleRow[2] = "Audio";
            csvTitleRow[3] = "Sweet";
            csvTitleRow[4] = "Sour";
            csvTitleRow[5] = "Bitter";
            csvTitleRow[6] = "Salty";
            csvTitleRow[7] = "Sweet Conf";
            csvTitleRow[8] = "Sour Conf";
            csvTitleRow[9] = "Bitter Conf";
            csvTitleRow[10] = "Salty Conf";

            _contentOfResults.AppendLine(string.Join(";", csvTitleRow));
        }

        var csvRow = new string[11];
        csvRow[0] = _conditionOrder[_currentCondition][0].ToString();
        csvRow[1] = _conditionOrder[_currentCondition][1].ToString();
        csvRow[2] = _conditionOrder[_currentCondition][2].ToString();
        csvRow[3] = results[0][0].ToString();
        csvRow[4] = results[1][0].ToString();
        csvRow[5] = results[2][0].ToString();
        csvRow[6] = results[3][0].ToString();
        csvRow[7] = results[0][1].ToString();
        csvRow[8] = results[1][1].ToString();
        csvRow[9] = results[2][1].ToString();
        csvRow[10] = results[3][1].ToString();

        _contentOfResults.Append(string.Join(";", csvRow));
        WriteCsv(filePath, _contentOfResults, _currentCondition == 0);
        _contentOfResults.Clear();
    }

    private void WriteCsv(string filename, StringBuilder content, bool header)
    {
        UnityEngine.Debug.Log("Answers stored in path: " + filename);
        try
        {
            StreamWriter writer = header ? File.CreateText(filename) : File.AppendText(filename);
            writer.WriteLine(content);
            writer.Close();
        }
        catch (IOException e)
        {
            UnityEngine.Debug.LogError(e.Message);
        }
    }

    private void UpdateTimer(int length)
    {
        _timerText.text = (length - _stopwatch.Elapsed.Seconds).ToString();
        _timerFill.fillAmount = 1 - _stopwatch.Elapsed.Seconds / (float)length;
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
            results.Add(_conditions[idx]);
        }

        if (_conditions.GetLength(0) % 2 != 0 && participantID % 2 != 0)
        {
            results.Reverse();
        }

        return results;
    }

    private Texture2D ReturnAsRGBA(Texture2D texture)
    {
        var newTex = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
        newTex.SetPixels(texture.GetPixels());
        newTex.Apply();

        return newTex;
    }
}