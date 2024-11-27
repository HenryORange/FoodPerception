using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubmitController : MonoBehaviour
{
    public List<ToggleGroup> questions;
    private List<GameObject> _unansweredQuestions;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private bool CheckAllAnswered()
    {
        var answered = true;
        foreach (var questionGroup in questions)
        {
            if (!questionGroup.AnyTogglesOn()) answered = false;
        }

        return answered;
    }

    public void next()
    {
        if (CheckAllAnswered())
        {
            foreach(var group in questions) group.SetAllTogglesOff();
        }
        else
        {
            
        }
    }
}
