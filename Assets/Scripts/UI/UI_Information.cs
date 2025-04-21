using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class UI_Information : UI_Base
{
    public Button PlayerInformationButton;
    public Button EnemyInformationButton;
    public Button ExitButton;

    public GameObject PlayerInformation;
    public GameObject EnemyInformation;

    protected override void Initialize()
    {
        base.Initialize();

        PlayerInformationButton.onClick.AddListener(OnPlayerInformationButtonClick);
        EnemyInformationButton.onClick.AddListener(OnEnemyInformationButtonClick);
        ExitButton.onClick.AddListener(OnExitButtonClick);
    }

    private void OnPlayerInformationButtonClick()
    {
        if (EnemyInformation.activeSelf) EnemyInformation.SetActive(false);
        if (!PlayerInformation.activeSelf) PlayerInformation.SetActive(true);
    }

    private void OnEnemyInformationButtonClick()
    {
        if (PlayerInformation.activeSelf) PlayerInformation.SetActive(false);
        if (!EnemyInformation.activeSelf) EnemyInformation.SetActive(true);
    }

    private void OnExitButtonClick()
    {
        SceneManager.LoadScene(Define.LobbyScene);
    }
}