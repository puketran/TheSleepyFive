using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button startButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button creditsButton;

    private void Awake()
    {
        optionsButton.onClick.AddListener(() =>
        {

        });
        startButton.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.GameScene);
        });
        exitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
        creditsButton.onClick.AddListener(() =>
        {
            CreditsUI.Instance.Show();
        });
    }
}
