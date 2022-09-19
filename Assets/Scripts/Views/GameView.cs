using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class GameView : MonoBehaviour
{
    public event Action GameStarted;

    [SerializeField] private Image _winPanel;
    [SerializeField] private Image _losePanel;
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _exitButton;

    private void Awake()
    {
        _winPanel.gameObject.SetActive(false);
        _losePanel.gameObject.SetActive(false);
        _startButton.gameObject.SetActive(true);

        _exitButton.onClick.AddListener(OnExit);
        _startButton.onClick.AddListener(OnStart);
    }



    private void OnExit()
    {
        Application.Quit();

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }


    public void ShowWinHUD()
    {
        _winPanel.gameObject.SetActive(true);
        _startButton.gameObject.SetActive(true);
    }

    public void ShowLoseHUD()
    {
        _losePanel.gameObject.SetActive(true);
        _startButton.gameObject.SetActive(true);
    }

    private void OnStart()
    {
        _winPanel.gameObject.SetActive(false);
        _losePanel.gameObject.SetActive(false);

        _startButton.gameObject.SetActive(false);
        GameStarted?.Invoke();
    }

    private void OnDestroy()
    {
        _startButton.onClick.RemoveListener(OnStart);
        _exitButton.onClick.RemoveListener(OnExit);
    }
}