using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI_LobbyScene : UI_Base
{
    // ���� Lobby UI, �� ���� UI
    public GameObject LobbyTitle;
    public GameObject LobbyButton;
    public GameObject MapSelect;

    // ���� Lobby UI
    public Button StartButton;
    public Button InformationButton;

    // �� ���� UI
    public Button Forest;
    public Button DarkForest;
    public Button CancelButton;

    protected override void Initialize()
    {
        base.Initialize();

        // �÷��̾� ���� ���·� ����
        GameManager.Instance.PlayerInfo.IsAlive = true;

        // Lobby UI ����
        LobbyButton.SetActive(true);
        MapSelect.SetActive(false);

        StartButton.onClick.AddListener(OnStartButtonClick);
        InformationButton.onClick.AddListener(OnInformationButtonClick);

        Forest.onClick.AddListener(OnForestClick);
        DarkForest.onClick.AddListener(OnDarkForestClick);
        CancelButton.onClick.AddListener(OnCancelButtonClick);
    }

    // ���� ���� ����
    private void OnStartButtonClick()
    {
        if (LobbyButton.activeSelf)
        {
            LobbyTitle.SetActive(false);
            LobbyButton.SetActive(false);
        }
        if (!MapSelect.activeSelf) MapSelect.SetActive(true);
    }

    // �÷��̾� �� �� ����â ����
    private void OnInformationButtonClick()
    {
        SceneManager.LoadScene(Define.InformationScene);
    }

    // Forest �� ����
    private void OnForestClick()
    {
        var gameManager = GameManager.Instance;

        // Forest �� Ȱ��ȭ �� DarkForest ��Ȱ��ȭ
        gameManager.SelectMap.Forest = true;
        gameManager.SelectMap.DarkForest = false;

        // ���� �� �̵�
        SceneManager.LoadScene(Define.GameScene);
    }

    // DarkForest �� ����
    private void OnDarkForestClick()
    {
        var gameManager = GameManager.Instance;

        // DarkForest �� Ȱ��ȭ �� Forest ��Ȱ��ȭ
        gameManager.SelectMap.Forest = false;
        gameManager.SelectMap.DarkForest = true;

        // ���� �� �̵�
        SceneManager.LoadScene(Define.GameScene);
    }

    // �� ���� ���
    private void OnCancelButtonClick()
    {
        // �� ���� UI ��Ȱ��ȭ �� �κ� ��ư UI Ȱ��ȭ
        if (MapSelect.activeSelf) MapSelect.SetActive(false);
        if (!LobbyButton.activeSelf)
        {
            LobbyTitle.SetActive(true);
            LobbyButton.SetActive(true);
        }
    }
}