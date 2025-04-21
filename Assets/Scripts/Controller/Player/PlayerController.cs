using System.Collections;
using UnityEngine;

// 플레이어 캐릭터를 제어하는 클래스 (BaseController를 상속)
public class PlayerController : BaseController
{
    // 컴포넌트 참조 변수
    private Animator _animator;                     // 애니메이터 컴포넌트 (애니메이션 제어)
    private Rigidbody2D _rigidbody2D;               // 물리 엔진 적용을 위한 Rigidbody2D
    private BoxCollider2D _boxCollider2D;           // 공격 영역을 담당하는 BoxCollider2D
    private SpriteRenderer _spriteRenderer;         // 스프라이트 렌더링
    private CapsuleCollider2D _capsuleCollider2D;   // 기본 충돌 처리를 담당하는 CapsuleCollider2D

    private Coroutine _coDamaged;   // 피격 시 코루틴
    private WaitForSeconds _stunTimer = new WaitForSeconds(0.3f);   // 스턴 시간

    // 플레이어의 이동 방향 벡터
    private Vector2 _moveDir;
    private Vector3 _velocity = Vector3.zero;

    // 플레이어 상태 변수
    private bool _isGround;     // 땅에 있는 상태
    private bool _isMove;       // 이동 중인지 여부
    private bool _isRoll;       // 구르기 중인지 여부
    private bool _isAttack;     // 공격 중인지 여부
    private bool _isBlock;      // 방어 중인지 여부
    private bool _isJump;       // 점프 중인지 여부
    private bool _isFall;       // 하강 중인지 여부
    private bool _isAlive;      // 생존 상태 여부
    private bool _isDamaged;    // 피격 상태 여부
    private bool _isEnemyAlive; // 적 생존 상태 여부

    private int _attackCombo = 0;           // 현재 콤보 상태
    private float _comboTimer = 0.0f;       // 콤보 유지 시간
    private float _comboDuration = 0.6f;    // 다음 공격을 입력할 수 있는 시간
    private float _attackTimer = 0.0f;      // 공격 딜레이 시간
    private float _attackDelay = 0.3f;      // 공격 딜레이

    // 이동 방향을 설정할 때 자동으로 정규화 (normalized)
    public Vector2 MoveDir
    {
        get { return _moveDir; }
        set { _moveDir = value.normalized; }    // 벡터 정규화하여 일정한 속도로 이동
    }

    // 초기화 함수 (BaseController의 Initialize 오버라이드)
    protected override void Initialize()
    {
        // 컴포넌트 가져오기
        _animator = GetComponent<Animator>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _boxCollider2D = gameObject.GetComponentInChildren<BoxCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _capsuleCollider2D = GetComponent<CapsuleCollider2D>();

        // 코루틴 초기화
        _coDamaged = null;

        // Rigidbody2D 설정 (물리적인 움직임 조정)
        _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        _rigidbody2D.mass = 1.0f;
        _rigidbody2D.linearDamping = 0.5f;
        _rigidbody2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        _rigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionX;  // X축 밀림 방지
        _rigidbody2D.freezeRotation = true;         // 회전 방지 (캐릭터가 기울어지는 것 방지)
        _rigidbody2D.linearVelocity = Vector3.zero; // 초기 속도 0으로 설정

        // Capsule Collider2D 설정
        _capsuleCollider2D.isTrigger = false;
        _capsuleCollider2D.offset = new Vector2(-0.03f, 0.67f);
        _capsuleCollider2D.size = new Vector2(0.7f, 1.23f);

        // Box Collider2D 설정
        _boxCollider2D.isTrigger = true;
        _boxCollider2D.offset = new Vector2(0.93f, 0.63f);
        _boxCollider2D.size = new Vector2(1.07f, 1.16f);
        _boxCollider2D.enabled = false;

        // 생존 여부 가져오기
        _isAlive = GameManager.Instance.PlayerInfo.IsAlive;

        // 키보드 입력이 변경될 때 _moveDir 값 업데이트
        GameManager.Instance.OnMoveDirChanged += (dir) => { _moveDir = dir; };
    }

    // 물리 업데이트 (일정한 시간 간격마다 실행)
    private void FixedUpdate()
    {
        // 생존 상태일 경우
        if (_isAlive)
        {
            // 공격, 방어, 피격 상태가 아닐 시 이동
            if (!_isAttack && !_isBlock && !_isDamaged) Move();
            // 이동 영역 제한
            if (_isMove) ConstrainPosition();

            // 지면이 아닐 경우
            if (!_isGround)
            {
                // 점프 상태라면
                if (_isJump)
                {
                    // 점프 처리
                    Jump();
                    _isJump = false;
                }

                // 공중에서의 하강 처리
                Fall();
            }
        }
    }

    private void Update()
    {
        // 생존 상태일 경우
        if (_isAlive)
        {
            // 지면 상태일 경우
            if (_isGround)
            {
                // 구르기 가능
                Roll();

                // 구르기 상태가 아니라면
                if (!_isRoll)
                {
                    // 공격, 점프 상태 체크
                    AtackStateCheck();
                    JumpStateCheck();

                    // 공격, 방어 가능
                    Attack();
                    Block();
                }
            }
        }
    }

    // 매 프레임마다 피해를 준 적 목록 초기화
    private void LateUpdate()
    {
        ObjectManager.Instance.DamagedEnemies.Clear();
    }

    // 이동 처리
    private void Move()
    {
        float moveSpeed = GameManager.Instance.PlayerInfo.Speed;    // 이동 속도 가져오기
        float horizontal = Input.GetAxisRaw(Define.Horizontal);     // 수평 입력 받기
        _moveDir = new Vector2(horizontal, 0.0f);                   // 이동 방향 설정

        // 이동 적용
        this.transform.Translate(_moveDir * moveSpeed * Time.deltaTime);
        _isMove = _moveDir != Vector2.zero;     // 이동 여부 판단

        // 이동 방향에 따라 캐릭터 방향 변경 (좌우 반전)
        if (_isMove) _spriteRenderer.flipX = _moveDir.x < 0.0f;

        // 점프, 하강 중이 아닐 경우 애니메이션 설정
        _animator.SetBool(Define.IsMoveHash, !_isJump && !_isFall && _isMove);
    }

    // 점프 처리
    private void Jump()
    {
        // 점프했을 때 공격 비활성화 (활성화 되있으면 Move가 불가능)
        if (_isAttack) _isAttack = false;

        // 점프력 가져오기
        float jumpPower = GameManager.Instance.PlayerInfo.JumpPower;

        // 점프력 적용
        _rigidbody2D.AddForce(Vector2.up * (jumpPower * _rigidbody2D.mass), ForceMode2D.Impulse);

        // 점프 애니메이션 상태 변경
        _animator.SetBool(Define.IsGroundHash, false);
        _animator.SetBool(Define.IsJumpHash, true);
    }

    // 구르기 처리
    private void Roll()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _animator.SetTrigger(Define.Roll);
        }
        _isRoll = ActionStateCheck(Define.Roll);
    }

    // 하강 처리
    private void Fall()
    {
        // 하강 중인지 확인
        if (_rigidbody2D.linearVelocity.y <= 0.0f)
        {
            _isJump = false;
            _isFall = true;

            _animator.SetBool(Define.IsJumpHash, false);
            _animator.SetBool(Define.IsFallHash, true);
        }
    }

    // 공격 처리
    private void Attack()
    {
        if (_comboTimer > 0.0f)
        {
            _attackTimer -= Time.deltaTime;
            _comboTimer -= Time.deltaTime;

            // 시간 초기화 OR 콤보 도중 방어 했을 경우 초기화
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

            // 마우스 클릭 방향에 맞춰 반전 설정
            _spriteRenderer.flipX = direction.x < 0.0f;

            // 공격 방향에 맞춰 공격 영역 위치 변경
            _boxCollider2D.offset = (_spriteRenderer.flipX) ? new Vector2((_boxCollider2D.offset.x < 0.0f) ?
                                                              _boxCollider2D.offset.x : -_boxCollider2D.offset.x,
                                                              _boxCollider2D.offset.y) :
                                                              new Vector2((_boxCollider2D.offset.x > 0.0f) ?
                                                              _boxCollider2D.offset.x : -_boxCollider2D.offset.x,
                                                              _boxCollider2D.offset.y);
            // 공격 영역 활성화
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

            // 공격 딜레이 초기화
            _attackTimer = _attackDelay;
            // 콤보 타이머 초기화
            _comboTimer = _comboDuration;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (_boxCollider2D.enabled) _boxCollider2D.enabled = false;
        }
    }

    // 방어 처리
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
        // 피격 애니메이션 재생
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

        // 현재 애니메이션이 체크하고자 하는 애니메이션인지 확인
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName(action))
        {
            // 원하는 애니메이션이라면 플레이 중인지 체크
            float animTime = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

            // 애니메이션 플레이 중
            if (animTime > 0.0f && animTime < 1.0f) state = true;
            else state = false;
        }
        else if (!_animator.GetCurrentAnimatorStateInfo(0).IsName(action))
        {
            state = false;
        }

        return state;
    }

    // 공격 상태 체크
    private void AtackStateCheck()
    {
        // 현재 애니메이션이 체크하고자 하는 애니메이션인지 확인
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName(Define.Attack1) ||
            _animator.GetCurrentAnimatorStateInfo(0).IsName(Define.Attack2) ||
            _animator.GetCurrentAnimatorStateInfo(0).IsName(Define.Attack3))
        {
            // 원하는 애니메이션이라면 플레이 중인지 체크
            float animTime = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

            // 애니메이션 플레이 중
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

    // 점프 상태 체크
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

    // 충돌 감지
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

    // 적과 충돌 시 피해 처리
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Define.EnemyAttackTag) && _isAlive && !_isRoll)
        {
            // 충돌한 적 오브젝트 (공격 영역의 부모 오브젝트) 가져오기
            EnemyController enemy = collision.GetComponentInParent<EnemyController>();

            // 적이 null이 아니며, 생존 상태이고, 중복 공격이 아니라면
            if (enemy != null && enemy.IsAlive && !ObjectManager.Instance.DamagedEnemies.Contains(enemy))
            {
                // 현재 프레임에서 이미 피해를 준 적으로 등록
                ObjectManager.Instance.DamagedEnemies.Add(enemy);

                // 적 오브젝트의 공격력 만큼 체력 감소
                if (_isBlock) GameManager.Instance.CurrentHp -= (enemy.Atk / 2);
                else GameManager.Instance.CurrentHp -= enemy.Atk;

                // 체력이 0 이하일 경우
                if (GameManager.Instance.CurrentHp <= 0.0f)
                {
                    // 사망 상태로 전환
                    _isAlive = false;
                    GameManager.Instance.PlayerInfo.IsAlive = _isAlive;
                    _animator.SetTrigger(Define.Death);

                    // GameManager의 GameOverSetting 실행
                    GameManager.Instance.GameOverSetting();
                }
                // 체력이 있을 경우
                else
                {
                    if (_coDamaged == null) _coDamaged = StartCoroutine(CoDamaged());
                }
            }
        }
    }
}