using System.Collections;
using UnityEngine;

// �÷��̾� ĳ���͸� �����ϴ� Ŭ���� (BaseController�� ���)
public class PlayerController : BaseController
{
    // ������Ʈ ���� ����
    private Animator _animator;                     // �ִϸ����� ������Ʈ (�ִϸ��̼� ����)
    private Rigidbody2D _rigidbody2D;               // ���� ���� ������ ���� Rigidbody2D
    private BoxCollider2D _boxCollider2D;           // ���� ������ ����ϴ� BoxCollider2D
    private SpriteRenderer _spriteRenderer;         // ��������Ʈ ������
    private CapsuleCollider2D _capsuleCollider2D;   // �⺻ �浹 ó���� ����ϴ� CapsuleCollider2D

    private Coroutine _coDamaged;   // �ǰ� �� �ڷ�ƾ
    private WaitForSeconds _stunTimer = new WaitForSeconds(0.3f);   // ���� �ð�

    // �÷��̾��� �̵� ���� ����
    private Vector2 _moveDir;
    private Vector3 _velocity = Vector3.zero;

    // �÷��̾� ���� ����
    private bool _isGround;     // ���� �ִ� ����
    private bool _isMove;       // �̵� ������ ����
    private bool _isRoll;       // ������ ������ ����
    private bool _isAttack;     // ���� ������ ����
    private bool _isBlock;      // ��� ������ ����
    private bool _isJump;       // ���� ������ ����
    private bool _isFall;       // �ϰ� ������ ����
    private bool _isAlive;      // ���� ���� ����
    private bool _isDamaged;    // �ǰ� ���� ����
    private bool _isEnemyAlive; // �� ���� ���� ����

    private int _attackCombo = 0;           // ���� �޺� ����
    private float _comboTimer = 0.0f;       // �޺� ���� �ð�
    private float _comboDuration = 0.6f;    // ���� ������ �Է��� �� �ִ� �ð�
    private float _attackTimer = 0.0f;      // ���� ������ �ð�
    private float _attackDelay = 0.3f;      // ���� ������

    // �̵� ������ ������ �� �ڵ����� ����ȭ (normalized)
    public Vector2 MoveDir
    {
        get { return _moveDir; }
        set { _moveDir = value.normalized; }    // ���� ����ȭ�Ͽ� ������ �ӵ��� �̵�
    }

    // �ʱ�ȭ �Լ� (BaseController�� Initialize �������̵�)
    protected override void Initialize()
    {
        // ������Ʈ ��������
        _animator = GetComponent<Animator>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _boxCollider2D = gameObject.GetComponentInChildren<BoxCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _capsuleCollider2D = GetComponent<CapsuleCollider2D>();

        // �ڷ�ƾ �ʱ�ȭ
        _coDamaged = null;

        // Rigidbody2D ���� (�������� ������ ����)
        _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        _rigidbody2D.mass = 1.0f;
        _rigidbody2D.linearDamping = 0.5f;
        _rigidbody2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        _rigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionX;  // X�� �и� ����
        _rigidbody2D.freezeRotation = true;         // ȸ�� ���� (ĳ���Ͱ� �������� �� ����)
        _rigidbody2D.linearVelocity = Vector3.zero; // �ʱ� �ӵ� 0���� ����

        // Capsule Collider2D ����
        _capsuleCollider2D.isTrigger = false;
        _capsuleCollider2D.offset = new Vector2(-0.03f, 0.67f);
        _capsuleCollider2D.size = new Vector2(0.7f, 1.23f);

        // Box Collider2D ����
        _boxCollider2D.isTrigger = true;
        _boxCollider2D.offset = new Vector2(0.93f, 0.63f);
        _boxCollider2D.size = new Vector2(1.07f, 1.16f);
        _boxCollider2D.enabled = false;

        // ���� ���� ��������
        _isAlive = GameManager.Instance.PlayerInfo.IsAlive;

        // Ű���� �Է��� ����� �� _moveDir �� ������Ʈ
        GameManager.Instance.OnMoveDirChanged += (dir) => { _moveDir = dir; };
    }

    // ���� ������Ʈ (������ �ð� ���ݸ��� ����)
    private void FixedUpdate()
    {
        // ���� ������ ���
        if (_isAlive)
        {
            // ����, ���, �ǰ� ���°� �ƴ� �� �̵�
            if (!_isAttack && !_isBlock && !_isDamaged) Move();
            // �̵� ���� ����
            if (_isMove) ConstrainPosition();

            // ������ �ƴ� ���
            if (!_isGround)
            {
                // ���� ���¶��
                if (_isJump)
                {
                    // ���� ó��
                    Jump();
                    _isJump = false;
                }

                // ���߿����� �ϰ� ó��
                Fall();
            }
        }
    }

    private void Update()
    {
        // ���� ������ ���
        if (_isAlive)
        {
            // ���� ������ ���
            if (_isGround)
            {
                // ������ ����
                Roll();

                // ������ ���°� �ƴ϶��
                if (!_isRoll)
                {
                    // ����, ���� ���� üũ
                    AtackStateCheck();
                    JumpStateCheck();

                    // ����, ��� ����
                    Attack();
                    Block();
                }
            }
        }
    }

    // �� �����Ӹ��� ���ظ� �� �� ��� �ʱ�ȭ
    private void LateUpdate()
    {
        ObjectManager.Instance.DamagedEnemies.Clear();
    }

    // �̵� ó��
    private void Move()
    {
        float moveSpeed = GameManager.Instance.PlayerInfo.Speed;    // �̵� �ӵ� ��������
        float horizontal = Input.GetAxisRaw(Define.Horizontal);     // ���� �Է� �ޱ�
        _moveDir = new Vector2(horizontal, 0.0f);                   // �̵� ���� ����

        // �̵� ����
        this.transform.Translate(_moveDir * moveSpeed * Time.deltaTime);
        _isMove = _moveDir != Vector2.zero;     // �̵� ���� �Ǵ�

        // �̵� ���⿡ ���� ĳ���� ���� ���� (�¿� ����)
        if (_isMove) _spriteRenderer.flipX = _moveDir.x < 0.0f;

        // ����, �ϰ� ���� �ƴ� ��� �ִϸ��̼� ����
        _animator.SetBool(Define.IsMoveHash, !_isJump && !_isFall && _isMove);
    }

    // ���� ó��
    private void Jump()
    {
        // �������� �� ���� ��Ȱ��ȭ (Ȱ��ȭ �������� Move�� �Ұ���)
        if (_isAttack) _isAttack = false;

        // ������ ��������
        float jumpPower = GameManager.Instance.PlayerInfo.JumpPower;

        // ������ ����
        _rigidbody2D.AddForce(Vector2.up * (jumpPower * _rigidbody2D.mass), ForceMode2D.Impulse);

        // ���� �ִϸ��̼� ���� ����
        _animator.SetBool(Define.IsGroundHash, false);
        _animator.SetBool(Define.IsJumpHash, true);
    }

    // ������ ó��
    private void Roll()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _animator.SetTrigger(Define.Roll);
        }
        _isRoll = ActionStateCheck(Define.Roll);
    }

    // �ϰ� ó��
    private void Fall()
    {
        // �ϰ� ������ Ȯ��
        if (_rigidbody2D.linearVelocity.y <= 0.0f)
        {
            _isJump = false;
            _isFall = true;

            _animator.SetBool(Define.IsJumpHash, false);
            _animator.SetBool(Define.IsFallHash, true);
        }
    }

    // ���� ó��
    private void Attack()
    {
        if (_comboTimer > 0.0f)
        {
            _attackTimer -= Time.deltaTime;
            _comboTimer -= Time.deltaTime;

            // �ð� �ʱ�ȭ OR �޺� ���� ��� ���� ��� �ʱ�ȭ
            if (_comboTimer <= 0.0f || _isBlock)
            {
                _boxCollider2D.enabled = false;
                _attackCombo = 0;
            }
        }

        if (Input.GetMouseButtonDown(0) && _attackTimer <= 0.0f)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 playerPos = transform.position;

            Vector2 direction = (mousePos - playerPos).normalized;

            // ���콺 Ŭ�� ���⿡ ���� ���� ����
            _spriteRenderer.flipX = direction.x < 0.0f;

            // ���� ���⿡ ���� ���� ���� ��ġ ����
            _boxCollider2D.offset = (_spriteRenderer.flipX) ? new Vector2((_boxCollider2D.offset.x < 0.0f) ?
                                                              _boxCollider2D.offset.x : -_boxCollider2D.offset.x,
                                                              _boxCollider2D.offset.y) :
                                                              new Vector2((_boxCollider2D.offset.x > 0.0f) ?
                                                              _boxCollider2D.offset.x : -_boxCollider2D.offset.x,
                                                              _boxCollider2D.offset.y);
            // ���� ���� Ȱ��ȭ
            _boxCollider2D.enabled = true;

            if (_attackCombo == 0 && _attackTimer <= 0.0f)
            {
                _animator.SetTrigger(Define.Attack1);
                _attackCombo = 1;
            }
            else if (_attackCombo == 1 && _comboTimer > 0.0f && _attackTimer <= 0.0f)
            {
                _animator.SetTrigger(Define.Attack2);
                _attackCombo = 2;
            }
            else if (_attackCombo == 2 && _comboTimer > 0.0f && _attackTimer <= 0.0f)
            {
                _animator.SetTrigger(Define.Attack3);
                _attackCombo = 0;
            }

            // ���� ������ �ʱ�ȭ
            _attackTimer = _attackDelay;
            // �޺� Ÿ�̸� �ʱ�ȭ
            _comboTimer = _comboDuration;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (_boxCollider2D.enabled) _boxCollider2D.enabled = false;
        }
    }

    // ��� ó��
    private void Block()
    {
        if (Input.GetMouseButtonDown(1))
        {
            _isBlock = true;
            _animator.SetTrigger(Define.Block);
            _animator.SetBool(Define.IsIdleBlockHash, true);
        }
        else if (Input.GetMouseButtonUp(1)) 
        {
            _animator.SetBool(Define.IsIdleBlockHash, false);
            Invoke("UnBlock", 0.2f);
        }
    }

    private IEnumerator CoDamaged()
    {
        // �ǰ� �ִϸ��̼� ���
        if (!_isBlock)
        {
            _isDamaged = true;
            _animator.SetTrigger(Define.Hurt);

            yield return _stunTimer;

            _isDamaged = false;
            _coDamaged = null;
        }
    }

    private bool ActionStateCheck(string action)
    {
        bool state = false;

        // ���� �ִϸ��̼��� üũ�ϰ��� �ϴ� �ִϸ��̼����� Ȯ��
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName(action))
        {
            // ���ϴ� �ִϸ��̼��̶�� �÷��� ������ üũ
            float animTime = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

            // �ִϸ��̼� �÷��� ��
            if (animTime > 0.0f && animTime < 1.0f) state = true;
            else state = false;
        }
        else if (!_animator.GetCurrentAnimatorStateInfo(0).IsName(action))
        {
            state = false;
        }

        return state;
    }

    // ���� ���� üũ
    private void AtackStateCheck()
    {
        // ���� �ִϸ��̼��� üũ�ϰ��� �ϴ� �ִϸ��̼����� Ȯ��
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName(Define.Attack1) ||
            _animator.GetCurrentAnimatorStateInfo(0).IsName(Define.Attack2) ||
            _animator.GetCurrentAnimatorStateInfo(0).IsName(Define.Attack3))
        {
            // ���ϴ� �ִϸ��̼��̶�� �÷��� ������ üũ
            float animTime = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

            // �ִϸ��̼� �÷��� ��
            if (animTime > 0 && animTime < 1.0f)
            {
                _isAttack = true;
            }
        }
        else if (!_animator.GetCurrentAnimatorStateInfo(0).IsName(Define.Attack1) ||
                 !_animator.GetCurrentAnimatorStateInfo(0).IsName(Define.Attack2) ||
                 !_animator.GetCurrentAnimatorStateInfo(0).IsName(Define.Attack3))
        {
            _isAttack = false;
        }
    }

    // ���� ���� üũ
    private void JumpStateCheck()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _isJump = true;
            _isGround = false;
        }
    }

    private void UnBlock()
    {
        _isBlock = false;
    }

    private void ConstrainPosition()
    {
        Vector2 currentPos = transform.position;
        currentPos.x = Mathf.Clamp(transform.position.x, -8.33f, 8.33f);
        transform.position = currentPos;
        transform.position = Vector3.SmoothDamp(transform.position, currentPos, ref _velocity, 0.8f);
    }

    // �浹 ����
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag(Define.GroundTag) && _rigidbody2D.linearVelocity.y <= 0.0f)
        {
            _isJump = false;
            _isFall = false;
            _isGround = true;

            _animator.SetBool(Define.IsJumpHash, false);
            _animator.SetBool(Define.IsFallHash, false);
            _animator.SetBool(Define.IsGroundHash, true);
        }
    }

    // ���� �浹 �� ���� ó��
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Define.EnemyAttackTag) && _isAlive && !_isRoll)
        {
            // �浹�� �� ������Ʈ (���� ������ �θ� ������Ʈ) ��������
            EnemyController enemy = collision.GetComponentInParent<EnemyController>();

            // ���� null�� �ƴϸ�, ���� �����̰�, �ߺ� ������ �ƴ϶��
            if (enemy != null && enemy.IsAlive && !ObjectManager.Instance.DamagedEnemies.Contains(enemy))
            {
                // ���� �����ӿ��� �̹� ���ظ� �� ������ ���
                ObjectManager.Instance.DamagedEnemies.Add(enemy);

                // �� ������Ʈ�� ���ݷ� ��ŭ ü�� ����
                if (_isBlock) GameManager.Instance.CurrentHp -= (enemy.Atk / 2);
                else GameManager.Instance.CurrentHp -= enemy.Atk;

                // ü���� 0 ������ ���
                if (GameManager.Instance.CurrentHp <= 0.0f)
                {
                    // ��� ���·� ��ȯ
                    _isAlive = false;
                    GameManager.Instance.PlayerInfo.IsAlive = _isAlive;
                    _animator.SetTrigger(Define.Death);

                    // GameManager�� GameOverSetting ����
                    GameManager.Instance.GameOverSetting();
                }
                // ü���� ���� ���
                else
                {
                    if (_coDamaged == null) _coDamaged = StartCoroutine(CoDamaged());
                }
            }
        }
    }
}