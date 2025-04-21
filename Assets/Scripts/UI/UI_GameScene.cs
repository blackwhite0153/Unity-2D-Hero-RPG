using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class UI_GameScene : UI_Base
{
    // 현재 보유 골드를 표시할 텍스트
    public TMP_Text GoldText;

    // 맵 UI
    public GameObject Forest;
    public GameObject DarkForest;

    // 일시정지 UI
    public GameObject Pause;

    // 버튼
    public Button PauseButton;      // 일시정지 버튼
    public Button PauseExitButton;  // 일시정지 해제 및 로비로 나가기 버튼
    public Button ReStartButton;    // 게임 재시작 버튼
    public Button ExitButton;       // 게임 종료 버튼

    // UI 초기화
    protected override void Initialize()
    {
        base.Initialize();

        // 맵 UI 비활성화
        Forest.SetActive(false);
        DarkForest.SetActive(false);

        // 일시정지 UI를 비활성화
        Pause.SetActive(false);

        // 버튼 클릭 이벤트에 해당하는 메서드 연결
        PauseButton.onClick.AddListener(OnPauseButtonClick);
        PauseExitButton.onClick.AddListener(OnPauseExitButtonClick);
        ReStartButton.onClick.AddListener(OnReStartButtonClick);
        ExitButton.onClick.AddListener(OnExitButtonClick);
    }

    private void Start()
    {
        var gameManager = GameManager.Instance;

        // 맵 선택 여부에 따라 맵 UI 활성화
        if (gameManager.SelectMap.Forest) Forest.SetActive(true);
        if (gameManager.SelectMap.DarkForest) DarkForest.SetActive(true);
    }

    // 프레임마다 골드 텍스트를 갱신
    private void Update()
    {
        TextDisplay();
    }

    // 게임의 시간 흐름을 제어 (일시정지 및 해제)
    private void TimeScale()
    {
        if (Time.timeScale == 1.0f) Time.timeScale = 0.0f;  // 게임 정지
        else Time.timeScale = 1.0f; // 게임 진행
    }

    // 일시정지 버튼 클릭 시 실행
    private void OnPauseButtonClick()
    {
        TimeScale();

        // 일시정지 UI의 활성화 여부를 변경
        if (Pause.activeSelf) Pause.SetActive(false);
        else Pause.SetActive(true);
    }


    // 일시정지 해제 및 로비로 이동하는 버튼 클릭 시 실행
    private void OnPauseExitButtonClick()
    {
        var gamaManager = GameManager.Instance;

        TimeScale();    // 게임 진행 상태로 변경

        // 플레이어가 살아있지 않은 경우, 다시 살아나도록 설정
        if (!gamaManager.PlayerInfo.IsAlive) gamaManager.PlayerInfo.IsAlive = true;

        // 비동기 씬 로드
        StartCoroutine(LoadMyAsyncScene(Define.LobbyScene));
    }

    // 게임을 재시작하는 버튼 클릭 시 실행
    private void OnReStartButtonClick()
    {
        var gamaManager = GameManager.Instance;

        // 플레이어가 살아있지 않은 경우, 다시 살아나도록 설정
        if (!gamaManager.PlayerInfo.IsAlive) gamaManager.PlayerInfo.IsAlive = true;

        // 현재 씬을 다시 로드하여 게임을 초기 상태로 재시작
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // 게임 종료 버튼 클릭 시 실행
    private void OnExitButtonClick()
    {
        var gamaManager = GameManager.Instance;

        // 플레이어가 살아있지 않은 경우, 다시 살아나도록 설정
        if (!gamaManager.PlayerInfo.IsAlive) gamaManager.PlayerInfo.IsAlive = true;

        // 비동기 씬 로드
        StartCoroutine(LoadMyAsyncScene(Define.LobbyScene));
    }

    // 현재 보유 골드를 UI에 표시
    private void TextDisplay()
    {
        var gamaManager = GameManager.Instance;

        GoldText.text = $"Gold : {gamaManager.Gold} $";
    }

    // 부드러운 씬 전환을 위한 비동기 씬 전환 코루틴
    private IEnumerator LoadMyAsyncScene(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}