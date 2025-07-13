using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FTUEUI : MonoBehaviour
{
    private const string PLAYER_PREFS_FTUE = "FTUE";

    [SerializeField] private Button contineButton;

    private void Awake()
    {
        contineButton.onClick.AddListener(() =>
        {
            Hide();
        });
    }

    private void Start()
    {
        if (PlayerPrefs.GetInt(PLAYER_PREFS_FTUE, 0) == 1)
        {
            Hide();
        }
    }
    
    public void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);

        PlayerPrefs.SetInt(PLAYER_PREFS_FTUE, 1);
        PlayerPrefs.Save();
    }
}
