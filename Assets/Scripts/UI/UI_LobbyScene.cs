using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI_LobbyScene : UI_Base
{
    // 기존 Lobby UI, 맵 선택 UI
    public GameObject LobbyTitle;
    public GameObject LobbyButton;
    public GameObject MapSelect;

    // 기존 Lobby UI
    public Button StartButton;
    public Button InformationButton;

    // 맵 선택 UI
    public Button Forest;
    public Button DarkForest;
    public Button CancelButton;

    protected override void Initialize()
    {
        base.Initialize();

        // 플레이어 생존 상태로 설정
        GameManager.Instance.PlayerInfo.IsAlive = true;

        // Lobby UI 세팅
        LobbyButton.SetActive(true);
        MapSelect.SetActive(false);

        StartButton.onClick.AddListener(OnStartButtonClick);
        InformationButton.onClick.AddListener(OnInformationButtonClick);

        Forest.onClick.AddListener(OnForestClick);
        DarkForest.onClick.AddListener(OnDarkForestClick);
        CancelButton.onClick.AddListener(OnCancelButtonClick);
    }

    // 게임 시작 선택
    private void OnStartButtonClick()
    {
        if (LobbyButton.activeSelf)
        {
            LobbyTitle.SetActive(false);
            LobbyButton.SetActive(false);
        }
        if (!MapSelect.activeSelf) MapSelect.SetActive(true);
    }

    // 플레이어 및 적 정보창 선택
    private void OnInformationButtonClick()
    {
        SceneManager.LoadScene(Define.InformationScene);
    }

    // Forest 맵 선택
    private void OnForestClick()
    {
        var gameManager = GameManager.Instance;

        // Forest 맵 활성화 및 DarkForest 비활성화
        gameManager.SelectMap.Forest = true;
        gameManager.SelectMap.DarkForest = false;

        // 게임 씬 이동
        SceneManager.LoadScene(Define.GameScene);
    }

    // DarkForest 맵 선택
    private void OnDarkForestClick()
    {
        var gameManager = GameManager.Instance;

        // DarkForest 맵 활성화 및 Forest 비활성화
        gameManager.SelectMap.Forest = false;
        gameManager.SelectMap.DarkForest = true;

        // 게임 씬 이동
        SceneManager.LoadScene(Define.GameScene);
    }

    // 맵 선택 취소
    private void OnCancelButtonClick()
    {
        // 맵 선택 UI 비활성화 후 로비 버튼 UI 활성화
        if (MapSelect.activeSelf) MapSelect.SetActive(false);
        if (!LobbyButton.activeSelf)
        {
            LobbyTitle.SetActive(true);
            LobbyButton.SetActive(true);
        }
    }
}