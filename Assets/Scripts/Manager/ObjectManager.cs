using System;
using System.Collections.Generic;
using UnityEngine;

// ���� �� ������Ʈ�� �����ϴ� �̱��� Ŭ����
public class ObjectManager : Singleton<ObjectManager>
{
    // �÷��̾� ��Ʈ�ѷ� �ν��Ͻ� (private ����)
    private PlayerController _player;

    // ���ҽ� (������) ���� ����
    private GameObject _heroKnightResource;
    private GameObject _heavyBanditResource;
    private GameObject _lightBanditResource;

    // �÷��̾� ��Ʈ�ѷ��� ���� public �б� ���� ������Ƽ
    public PlayerController Player { get => _player; }

    // �� ��ü���� �����ϴ� HashSet
    public HashSet<HeavyBanditController> HeavyBandit { get; set; } = new HashSet<HeavyBanditController>();
    public HashSet<LightBanditController> LightBandit { get; set; } = new HashSet<LightBanditController>();

    // ���ظ� �� ���� �����ϴ� HashSet
    public HashSet<EnemyController> DamagedEnemies = new HashSet<EnemyController>();

    // �̱��� �ʱ�ȭ (��ӵ� Singleton���� �⺻ �ʱ�ȭ ����)
    protected override void Initialize()
    {
        // Singleton �ʱ�ȭ ȣ��
        base.Initialize();
    }

    // ��� ���� ������Ʈ ���ҽ��� �ε��ϴ� �Լ�
    public void ResourceAllLoad()
    {
        // Resources.Load<T>(path)�� ����Ͽ� ������ �ε�
        _heroKnightResource = Resources.Load<GameObject>(Define.HeroKnightPath);
        _heavyBanditResource = Resources.Load<GameObject>(Define.HeavyBanditPath);
        _lightBanditResource = Resources.Load<GameObject>(Define.LightBanditPath);
    }

    // ���׸� Ÿ���� ����� Spawn �Լ�
    public T Spawn<T>(Vector3 spawnPos) where T : BaseController
    {
        // ���׸� Ÿ�� T�� �޾Ƽ� Ÿ���� ����
        Type type = typeof(T);

        if (type == typeof(PlayerController))
        {
            // �÷��̾� ĳ���� ����
            GameObject obj = Instantiate(_heroKnightResource, spawnPos, Quaternion.identity);   // �÷��̾� ������Ʈ ����
            PlayerController playerController = obj.GetOrAddComponent<PlayerController>();      // PlayerController ������Ʈ �߰�
            _player = playerController; // �÷��̾� ������ �Ҵ�

            // ���׸� Ÿ�� T�� ĳ�����Ͽ� ��ȯ
            return playerController as T;
        }
        else if (type == typeof(HeavyBanditController))
        {
            // HeavyBandit ���� ����
            GameObject obj = Instantiate(_heavyBanditResource, spawnPos, Quaternion.identity);              // �÷��̾� ������Ʈ ����
            HeavyBanditController heavyBanditController = obj.GetOrAddComponent<HeavyBanditController>();   // HeavyBanditController ������Ʈ �߰�
            HeavyBandit.Add(heavyBanditController); // ������ HeavyBanditController�� HashSet�� �߰�

            // ���׸� Ÿ�� T�� ĳ�����Ͽ� ��ȯ
            return heavyBanditController as T;
        }
        else if (type == typeof(LightBanditController))
        {
            // LightBandit ���� ����
            GameObject obj = Instantiate(_lightBanditResource, spawnPos, Quaternion.identity);              // �÷��̾� ������Ʈ ����
            LightBanditController lightBanditController = obj.GetOrAddComponent<LightBanditController>();   // LightBanditController ������Ʈ �߰�
            LightBandit.Add(lightBanditController); // ������ LightBanditController�� HashSet�� �߰�

            // ���׸� Ÿ�� T�� ĳ�����Ͽ� ��ȯ
            return lightBanditController as T;
        }

        // �ش��ϴ� Ÿ���� ������ null ��ȯ
        return null;
    }

    // ������Ʈ�� ��Ȱ��ȭ�ϴ� �Լ�
    public void Despawn<T>(T obj) where T : BaseController
    {
        // ������Ʈ Ǯ�� ����� ���� ��Ȱ��ȭ
        obj.gameObject.SetActive(false);
    }

    // ��ü �������� ������Ʈ�� �����ϴ� �Լ�
    protected override void Clear()
    {
        base.Clear();

        DamagedEnemies.Clear(); // DamagedEnemies HashSet �ʱ�ȭ (��� DamagedEnemies ����)
        HeavyBandit.Clear();    // HeavyBandit HashSet �ʱ�ȭ (��� HeavyBanditController ����)
        LightBandit.Clear();    // LightBandit HashSet �ʱ�ȭ (��� LightBanditController ����)
        _player = null;         // �÷��̾� ���� �ʱ�ȭ (�÷��̾� ����)

        // Resources�� �ε��� �� �� �޸𸮿��� ������ �ʴ� ���ҽ� ����
        Resources.UnloadUnusedAssets();
    }
}