using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class UI_GameScene : UI_Base
{
    // ���� ���� ��带 ǥ���� �ؽ�Ʈ
    public TMP_Text GoldText;

    // �� UI
    public GameObject Forest;
    public GameObject DarkForest;

    // �Ͻ����� UI
    public GameObject Pause;

    // ��ư
    public Button PauseButton;      // �Ͻ����� ��ư
    public Button PauseExitButton;  // �Ͻ����� ���� �� �κ�� ������ ��ư
    public Button ReStartButton;    // ���� ����� ��ư
    public Button ExitButton;       // ���� ���� ��ư

    // UI �ʱ�ȭ
    protected override void Initialize()
    {
        base.Initialize();

        // �� UI ��Ȱ��ȭ
        Forest.SetActive(false);
        DarkForest.SetActive(false);

        // �Ͻ����� UI�� ��Ȱ��ȭ
        Pause.SetActive(false);

        // ��ư Ŭ�� �̺�Ʈ�� �ش��ϴ� �޼��� ����
        PauseButton.onClick.AddListener(OnPauseButtonClick);
        PauseExitButton.onClick.AddListener(OnPauseExitButtonClick);
        ReStartButton.onClick.AddListener(OnReStartButtonClick);
        ExitButton.onClick.AddListener(OnExitButtonClick);
    }

    private void Start()
    {
        var gameManager = GameManager.Instance;

        // �� ���� ���ο� ���� �� UI Ȱ��ȭ
        if (gameManager.SelectMap.Forest) Forest.SetActive(true);
        if (gameManager.SelectMap.DarkForest) DarkForest.SetActive(true);
    }

    // �����Ӹ��� ��� �ؽ�Ʈ�� ����
    private void Update()
    {
        TextDisplay();
    }

    // ������ �ð� �帧�� ���� (�Ͻ����� �� ����)
    private void TimeScale()
    {
        if (Time.timeScale == 1.0f) Time.timeScale = 0.0f;  // ���� ����
        else Time.timeScale = 1.0f; // ���� ����
    }

    // �Ͻ����� ��ư Ŭ�� �� ����
    private void OnPauseButtonClick()
    {
        TimeScale();

        // �Ͻ����� UI�� Ȱ��ȭ ���θ� ����
        if (Pause.activeSelf) Pause.SetActive(false);
        else Pause.SetActive(true);
    }


    // �Ͻ����� ���� �� �κ�� �̵��ϴ� ��ư Ŭ�� �� ����
    private void OnPauseExitButtonClick()
    {
        var gamaManager = GameManager.Instance;

        TimeScale();    // ���� ���� ���·� ����

        // �÷��̾ ������� ���� ���, �ٽ� ��Ƴ����� ����
        if (!gamaManager.PlayerInfo.IsAlive) gamaManager.PlayerInfo.IsAlive = true;

        // �񵿱� �� �ε�
        StartCoroutine(LoadMyAsyncScene(Define.LobbyScene));
    }

    // ������ ������ϴ� ��ư Ŭ�� �� ����
    private void OnReStartButtonClick()
    {
        var gamaManager = GameManager.Instance;

        // �÷��̾ ������� ���� ���, �ٽ� ��Ƴ����� ����
        if (!gamaManager.PlayerInfo.IsAlive) gamaManager.PlayerInfo.IsAlive = true;

        // ���� ���� �ٽ� �ε��Ͽ� ������ �ʱ� ���·� �����
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // ���� ���� ��ư Ŭ�� �� ����
    private void OnExitButtonClick()
    {
        var gamaManager = GameManager.Instance;

        // �÷��̾ ������� ���� ���, �ٽ� ��Ƴ����� ����
        if (!gamaManager.PlayerInfo.IsAlive) gamaManager.PlayerInfo.IsAlive = true;

        // �񵿱� �� �ε�
        StartCoroutine(LoadMyAsyncScene(Define.LobbyScene));
    }

    // ���� ���� ��带 UI�� ǥ��
    private void TextDisplay()
    {
        var gamaManager = GameManager.Instance;

        GoldText.text = $"Gold : {gamaManager.Gold} $";
    }

    // �ε巯�� �� ��ȯ�� ���� �񵿱� �� ��ȯ �ڷ�ƾ
    private IEnumerator LoadMyAsyncScene(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}