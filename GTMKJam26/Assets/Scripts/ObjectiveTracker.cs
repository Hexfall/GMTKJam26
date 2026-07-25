using System;
using TMPro;
using UnityEngine;

public class ObjectiveTracker : MonoBehaviour
{
    [SerializeField][Range(0, 1)] private float typewriterTime = .035f;
    private float timeSinceLast = 0;
    private TextMeshProUGUI  objectiveText;
    private string CurrentText
    {
        get => objectiveText.text;
        set => objectiveText.text = value;
    }
    
    private string _text = "";
    public string Text
    {
        get => _text;
        set
        {
            _text = value;
            CurrentText = "";
            timeSinceLast = 0;
        }
    }

    private void Awake()
    {
        objectiveText = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        timeSinceLast += Time.deltaTime;
        if (timeSinceLast > typewriterTime)
        {
            timeSinceLast = 0;
            UpdateText();
        }
    }

    private void UpdateText()
    {
        if (CurrentText.Length == Text.Length)
            return;
        CurrentText = Text.Substring(0, CurrentText.Length + 1);
    }
}
