using System.Collections;
using UnityEngine;

public class EnemyController : BaseController
{
    // ������Ʈ ���� ����
    private Animator _animator;                     // �ִϸ����� ������Ʈ (�ִϸ��̼� ����)
    private Collider2D _collider2D;                 // 2D �浹 ó��
    private Rigidbody2D _rigidbody2D;               // ���� ���� ������ ���� Rigidbody2D
    private BoxCollider2D _boxCollider2D;           // ���� ������ ����ϴ� BoxCollider2D
    private SpriteRenderer _spriteRenderer;         // ��������Ʈ ������
    private CapsuleCollider2D _capsuleCollider2D;   // �⺻ �浹 ó���� ����ϴ� CapsuleCollider2D

    // �ڷ�ƾ ���� ����
    private Coroutine _coAttack;  // ���� �ڷ�ƾ
    private Coroutine _coDamaged; // �ǰ� �� �ڷ�ƾ
    private Coroutine _coDeath;   // ��� �� �ڷ�ƾ

    // WaitForSeconds�� ������ ������ �ð�
    private WaitForSeconds _destroyTimer = new WaitForSeconds(0.05f);           // ��� �� ��Ȱ��ȭ Ÿ�̸�
    private WaitForSeconds _attackAreaEnableTimer = new WaitForSeconds(0.5f);   // ���� ���� ���� �ð�
    private WaitForSeconds _attackDelay = new WaitForSeconds(2.0f);             // ���� ������
    private WaitForSeconds _stunTimer = new WaitForSeconds(0.3f);               // ���� �ð�

    // �� ���� ����
    private Color _color;         // ���� ���� ����
    private Color _colorValue;    // �⺻ ���� �����

    // �� ���� ����
    private bool _isAlive;       // ���� ���� ����
    private bool _isMove;        // �̵� ���� ����
    private bool _isPlayerAlive; // �÷��̾� ���� ����
    private bool _isAttackMode;  // ���� ��� Ȱ��ȭ ����
    private bool _isAttack;      // ���� ���� ����
    private bool _isDamaged;     // �ǰ� ���� ����

    // �� �ɷ�ġ ����
    private float _enemyMaxHp;      // �� �ִ� ü��
    private float _enemyCurrentHp;  // �� ���� ü��
    private float _enemyAtk;        // �� ���ݷ�
    private float _enemySpeed;      // �� �̵� �ӵ�
    private float _enemyExp;        // �� ����ġ �����
    private int _enemyGold;         // �� ��� �����

    private float _distance;            // �÷��̾���� �Ÿ�
    private float _stopDistance = 1.1f; // �÷��̾���� �ּ� ����

    // PlayerController���� Enemy�� ������ �ľ��ϱ� ���� ������Ƽ
    public float Atk { get { return _enemyAtk; } }
    public bool IsAlive { get { return _isAlive; } }

    // EnemyHp���� Enemy�� ���� ü�� ���¸� �ľ��ϱ� ���� ������Ƽ
    public float CurrentHp { get { return _enemyCurrentHp; } }

    // �ʱ�ȭ
    protected override void Initialize()
    {
        // �ʿ��� ������Ʈ ��������
        _animator = GetComponent<Animator>();
        _collider2D = GetComponent<Collider2D>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _boxCollider2D = gameObject.GetComponentInChildren<BoxCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _capsuleCollider2D = GetComponent<CapsuleCollider2D>();

        // ���� ���� �� ����
        _colorValue = _spriteRenderer.color;

        // �⺻ ���� ����
        Settings();
    }

    // ������Ʈ Ȱ��ȭ �� ȣ��
    private void OnEnable()
    {
        // ���� �ʱ�ȭ
        Settings();
    }

    // ���� ������Ʈ
    private void FixedUpdate()
    {
        // ���� ����ְ�
        if (_isAlive)
        {
            // ����, �ǰ� ���°� �ƴ϶�� �̵�
            if (!_isAttack && !_isDamaged) Move();
        }
    }

    private void Update()
    {
        PlayerAliveCheck(); // �÷��̾� ���� ���� üũ
    }

    // �� �⺻ ���� �ʱ�ȭ
    private void Settings()
    {
        // �ڷ�ƾ �ʱ�ȭ
        _coAttack = null;
        _coDamaged = null;
        _coDeath = null;

        // ���� �ʱ�ȭ
        _color = _colorValue;

        // Rigidbody2D ����
        _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        _rigidbody2D.mass = 1.0f;
        _rigidbody2D.linearDamping = 0.5f;
        _rigidbody2D.gravityScale = 1.0f;
        _rigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionY;
        _rigidbody2D.freezeRotation = true;
        _rigidbody2D.linearVelocity = Vector3.zero;

        // Capsule Collider2D ����
        _capsuleCollider2D.isTrigger = false;
        _capsuleCollider2D.offset = new Vector2(0.0f, 0.63f);
        _capsuleCollider2D.size = new Vector2(0.45f, 1.13f);

        // Box Collider2D ����
        _boxCollider2D.isTrigger = true;
        _boxCollider2D.offset = new Vector2(-0.57f, 0.75f);
        _boxCollider2D.size = new Vector2(0.35f, 1.5f);
        _boxCollider2D.enabled = false;

        // Sprite Renderer ����
        _spriteRenderer.color = _color;

        // ���°� �ʱ�ȭ
        _isAlive = true;
        _isMove = false;
        _isPlayerAlive = true;
        _isAttackMode = false;
        _isAttack = false;
        _isDamaged = false;

        // �� ���� �ҷ�����
        _enemyMaxHp = GameManager.Instance.EnemyInfo.MaxHp;
        _enemyCurrentHp = _enemyMaxHp;
        _enemyAtk = GameManager.Instance.EnemyInfo.Atk;
        _enemySpeed = GameManager.Instance.EnemyInfo.Speed;
        _enemyExp = GameManager.Instance.EnemyInfo.ExpDrop;
        _enemyGold = GameManager.Instance.EnemyInfo.GoldDrop;
    }

    // �̵� ó��
    private void Move()
    {
        PlayerController player = ObjectManager.Instance.Player;

        if (player == null) return;

        // �÷��̾���� �Ÿ� ��� (Y�� ����)
        _distance = Vector3.Distance(new Vector3(player.transform.position.x, 0.0f, 0.0f),
                                     new Vector3(transform.position.x, 0.0f, 0.0f));

        Vector3 targetPos = Vector3.zero;
        Vector3 targetDir = Vector3.zero;

        if (_isPlayerAlive)
        {
            // ��ǥ ��ġ �� ���� ����
            targetPos = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
            targetDir = (targetPos - transform.position).normalized;

            if (_distance > _stopDistance)
            {
                // ���� ��� ���� �� �̵� ����
                if (_isAttackMode)
                {
                    _isAttack = false;
                    _isAttackMode = false;
                    _animator.SetBool(Define.IsAttackModeHash, false);
                }

                transform.position += new Vector3(targetDir.x, 0.0f, 0.0f) * _enemySpeed * Time.deltaTime;

                _isMove = (Vector2)targetDir != Vector2.zero;
                _animator.SetBool(Define.IsMoveHash, _isMove);
            }
            else
            {
                // ���� ��� ����
                _isAttackMode = true;
                _animator.SetBool(Define.IsMoveHash, false);
                _animator.SetBool(Define.IsAttackModeHash, true);

                if (_coAttack == null) _coAttack = StartCoroutine(CoAttack());
            }

            // �÷��̾� ������ �ٶ󺸵��� ó��
            _spriteRenderer.flipX = (targetDir.x > 0.0f);
        }
        else
        {
            _isAttackMode = false;
            _animator.SetBool(Define.IsMoveHash, false);
            _animator.SetBool(Define.IsAttackModeHash, false);
        }
    }

    // ���� ó��
    private IEnumerator CoAttack()
    {
        if (_isAttackMode)
        {
            _isAttack = true;

            // ���� ���⿡ ���� ���� ���� ��ġ ����
            _boxCollider2D.offset = (_spriteRenderer.flipX) ? new Vector2((_boxCollider2D.offset.x > 0.0f) ?
                                                              _boxCollider2D.offset.x : -_boxCollider2D.offset.x,
                                                              _boxCollider2D.offset.y) :
                                                              new Vector2((_boxCollider2D.offset.x < 0.0f) ?
                                                              _boxCollider2D.offset.x : -_boxCollider2D.offset.x,
                                                              _boxCollider2D.offset.y);

            _animator.SetBool(Define.IsAttackModeHash, _isAttack ? false : true);
            if (!_animator.GetBool(Define.IsAttackModeHash)) _animator.SetTrigger(Define.Attack);

            yield return _attackAreaEnableTimer;

            if (!_isDamaged) _boxCollider2D.enabled = true;

            yield return _attackAreaEnableTimer;

            _isAttack = false;
            _animator.SetBool(Define.IsAttackModeHash, true);
            if (_boxCollider2D.enabled) _boxCollider2D.enabled = false;

            yield return _attackDelay;

            StartCoroutine(CoAttack());
        }
        else
        {
            _coAttack = null;
        }
    }

    // �ǰ� ó��
    private IEnumerator CoDamaged()
    {
        _isDamaged = true;
        _animator.SetTrigger(Define.Hurt);

        yield return _stunTimer;

        _isDamaged = false;
        _coDamaged = null;
    }

    // ��� ó��
    private IEnumerator CoDeath()
    {
        _rigidbody2D.gravityScale = 0.0f;
        _capsuleCollider2D.isTrigger = true;

        // ��� ���� ��ȯ
        _isAlive = false;
        _isAttackMode = false;
        _isAttack = false;

        _animator.SetTrigger(Define.Death);

        // ������ 0�� �� ������ �ݺ�
        while (_color.a > 0)
        {
            // ���� ����
            _color.a -= 0.05f;
            // ���ο� ���� ����
            _spriteRenderer.color = new Color(_color.r, _color.g, _color.b, _color.a);

            yield return _destroyTimer;
        }

        // �� ������Ʈ ��Ȱ��ȭ
        ObjectManager.Instance.Despawn(this);
    }

    // �÷��̾� ���� ���� üũ
    private void PlayerAliveCheck()
    {
        _isPlayerAlive = GameManager.Instance.PlayerInfo.IsAlive;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // ���鿡 ��Ҵ��� Ȯ��
        if (collision.collider.CompareTag(Define.GroundTag))
        {
            _animator.SetBool(Define.IsGroundHash, true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �÷��̾� ������ �¾��� �� ó��
        if (collision.CompareTag(Define.PlayerAttackTag) && _isPlayerAlive && _isAlive)
        {
            bool isCritical = GameManager.Instance.Critical >= Random.Range(1.0f, 100.0f) ? true : false;
            float fDamage = GameManager.Instance.Atk;

            // ġ��Ÿ ���� ���� ������
            if (isCritical) fDamage += GameManager.Instance.Atk * GameManager.Instance.CriticalDamage;

            _enemyCurrentHp -= fDamage;

            if (_enemyCurrentHp <= 0.0f)
            {
                if (_coDeath == null) _coDeath = StartCoroutine(CoDeath());

                // ��� ȹ��
                GameManager.Instance.Gold += _enemyGold;
                // ����ġ ȹ��
                GameManager.Instance.TotalExp += _enemyExp;
            }
            else
            {
                if (_coDamaged == null) _coDamaged = StartCoroutine(CoDamaged());
            }
        }
    }
}