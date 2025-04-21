using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerExp : MonoBehaviour
{
    private int _level;
    private float _maxExp;
    private float _currentExp;

    public Slider ExpBar;
    public TMP_Text LevelText;
    public TMP_Text ExperienceText;


    private void Update()
    {
        UpdateState();
        DisplayExp();
    }

    private void UpdateState()
    {
        _level = GameManager.Instance.Level;
        _maxExp = GameManager.Instance.MaxExp;
        _currentExp = GameManager.Instance.TotalExp;

        ExpBar.maxValue = _maxExp;
        ExpBar.value = _currentExp;
    }

    private void DisplayExp()
    {
        LevelText.text = $"Lv. {_level}";
        ExperienceText.text = $"{((_currentExp / _maxExp) * 100).ToString("F1")}% ({_currentExp.ToString("F1")} / {_maxExp.ToString("F1")})";
    }
}