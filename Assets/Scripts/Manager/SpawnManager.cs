using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    // �� ������ �����ϴ� �ڷ�ƾ ����
    private Coroutine _coSpawningPool;
    // ���� ���� ����
    private WaitForSeconds _spawnInterval = new WaitForSeconds(5.0f);

    void Start()
    {
        // ���� ������Ʈ ���ҽ� �ε�
        ObjectManager.Instance.ResourceAllLoad();
        // ������ ��ġ�� �÷��̾� ����
        ObjectManager.Instance.Spawn<PlayerController>(new Vector3(-6.75f, -3.8f, 2.0f));

        // ���� �ڷ�ƾ�� ���� ���� �ƴϸ� ����
        if (_coSpawningPool == null)
        {
            _coSpawningPool = StartCoroutine(CoSpawningPool());
            _coSpawningPool = null;
        }
    }

    // Enemy�� �ֱ������� �����ϴ� �ڷ�ƾ
    private IEnumerator CoSpawningPool()
    {
        // �÷��̾ ���� ���¶�� ���� �ݺ�
        while (GameManager.Instance.PlayerInfo.IsAlive)
        {
            // ������ �� ���� ����
            int randomSpawn = Random.Range(0, 2);

            // ������ ��ġ�� �� ���� ����
            if (randomSpawn == 0) PoolManager.Instance.GetObject<HeavyBanditController>(new Vector3(10.0f, -3.81f, 2.0f));
            else if (randomSpawn == 1) PoolManager.Instance.GetObject<LightBanditController>(new Vector3(10.0f, -3.81f, 2.0f));

            // ���� ����
            yield return _spawnInterval;
        }
    }
}