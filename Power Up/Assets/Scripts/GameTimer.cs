using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameTimer : Singleton<GameTimer>
{
    private float _startTime;
    private float _currentTime;
    private TextMeshProUGUI _timerText;
    void Start()
    {
        _startTime = Time.time;
        _timerText = GetComponent<TextMeshProUGUI>();
        if (_timerText == null)
            _timerText = gameObject.AddComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        _currentTime += Time.deltaTime;
        if (_timerText != null)
        {
            int minutes = (int)_currentTime / 60;
            int seconds = (int)_currentTime % 60;
            _timerText.text = (minutes.ToString("F0") + ":" + seconds.ToString("00"));
        }
    }
}
