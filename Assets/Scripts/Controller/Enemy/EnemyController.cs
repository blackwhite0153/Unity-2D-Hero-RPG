using System.Collections;
using UnityEngine;

public class EnemyController : BaseController
{
    // 컴포넌트 참조 변수
    private Animator _animator;                     // 애니메이터 컴포넌트 (애니메이션 제어)
    private Collider2D _collider2D;                 // 2D 충돌 처리
    private Rigidbody2D _rigidbody2D;               // 물리 엔진 적용을 위한 Rigidbody2D
    private BoxCollider2D _boxCollider2D;           // 공격 영역을 담당하는 BoxCollider2D
    private SpriteRenderer _spriteRenderer;         // 스프라이트 렌더링
    private CapsuleCollider2D _capsuleCollider2D;   // 기본 충돌 처리를 담당하는 CapsuleCollider2D

    // 코루틴 참조 변수
    private Coroutine _coAttack;  // 공격 코루틴
    private Coroutine _coDamaged; // 피격 시 코루틴
    private Coroutine _coDeath;   // 사망 시 코루틴

    // WaitForSeconds로 선언한 딜레이 시간
    private WaitForSeconds _destroyTimer = new WaitForSeconds(0.05f);           // 사망 후 비활성화 타이머
    private WaitForSeconds _attackAreaEnableTimer = new WaitForSeconds(0.5f);   // 공격 영역 유지 시간
    private WaitForSeconds _attackDelay = new WaitForSeconds(2.0f);             // 공격 딜레이
    private WaitForSeconds _stunTimer = new WaitForSeconds(0.3f);               // 스턴 시간

    // 적 색상 정보
    private Color _color;         // 현재 적의 색상
    private Color _colorValue;    // 기본 색상 저장용

    // 적 상태 변수
    private bool _isAlive;       // 적의 생존 여부
    private bool _isMove;        // 이동 상태 여부
    private bool _isPlayerAlive; // 플레이어 생존 여부
    private bool _isAttackMode;  // 공격 모드 활성화 여부
    private bool _isAttack;      // 공격 상태 여부
    private bool _isDamaged;     // 피격 상태 여부

    // 적 능력치 변수
    private float _enemyMaxHp;      // 적 최대 체력
    private float _enemyCurrentHp;  // 적 현재 체력
    private float _enemyAtk;        // 적 공격력
    private float _enemySpeed;      // 적 이동 속도
    private float _enemyExp;        // 적 경험치 드랍량
    private int _enemyGold;         // 적 골드 드랍량

    private float _distance;            // 플레이어와의 거리
    private float _stopDistance = 1.1f; // 플레이어와의 최소 간격

    // PlayerController에서 Enemy의 정보를 파악하기 위한 프로퍼티
    public float Atk { get { return _enemyAtk; } }
    public bool IsAlive { get { return _isAlive; } }

    // EnemyHp에서 Enemy의 현재 체력 상태를 파악하기 위한 프로퍼티
    public float CurrentHp { get { return _enemyCurrentHp; } }

    // 초기화
    protected override void Initialize()
    {
        // 필요한 컴포넌트 가져오기
        _animator = GetComponent<Animator>();
        _collider2D = GetComponent<Collider2D>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _boxCollider2D = gameObject.GetComponentInChildren<BoxCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _capsuleCollider2D = GetComponent<CapsuleCollider2D>();

        // 최초 색상 값 저장
        _colorValue = _spriteRenderer.color;

        // 기본 상태 설정
        Settings();
    }

    // 오브젝트 활성화 시 호출
    private void OnEnable()
    {
        // 설정 초기화
        Settings();
    }

    // 물리 업데이트
    private void FixedUpdate()
    {
        // 적이 살아있고
        if (_isAlive)
        {
            // 공격, 피격 상태가 아니라면 이동
            if (!_isAttack && !_isDamaged) Move();
        }
    }

    private void Update()
    {
        PlayerAliveCheck(); // 플레이어 생존 여부 체크
    }

    // 적 기본 설정 초기화
    private void Settings()
    {
        // 코루틴 초기화
        _coAttack = null;
        _coDamaged = null;
        _coDeath = null;

        // 색상 초기화
        _color = _colorValue;

        // Rigidbody2D 설정
        _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        _rigidbody2D.mass = 1.0f;
        _rigidbody2D.linearDamping = 0.5f;
        _rigidbody2D.gravityScale = 1.0f;
        _rigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionY;
        _rigidbody2D.freezeRotation = true;
        _rigidbody2D.linearVelocity = Vector3.zero;

        // Capsule Collider2D 설정
        _capsuleCollider2D.isTrigger = false;
        _capsuleCollider2D.offset = new Vector2(0.0f, 0.63f);
        _capsuleCollider2D.size = new Vector2(0.45f, 1.13f);

        // Box Collider2D 설정
        _boxCollider2D.isTrigger = true;
        _boxCollider2D.offset = new Vector2(-0.57f, 0.75f);
        _boxCollider2D.size = new Vector2(0.35f, 1.5f);
        _boxCollider2D.enabled = false;

        // Sprite Renderer 설정
        _spriteRenderer.color = _color;

        // 상태값 초기화
        _isAlive = true;
        _isMove = false;
        _isPlayerAlive = true;
        _isAttackMode = false;
        _isAttack = false;
        _isDamaged = false;

        // 적 정보 불러오기
        _enemyMaxHp = GameManager.Instance.EnemyInfo.MaxHp;
        _enemyCurrentHp = _enemyMaxHp;
        _enemyAtk = GameManager.Instance.EnemyInfo.Atk;
        _enemySpeed = GameManager.Instance.EnemyInfo.Speed;
        _enemyExp = GameManager.Instance.EnemyInfo.ExpDrop;
        _enemyGold = GameManager.Instance.EnemyInfo.GoldDrop;
    }

    // 이동 처리
    private void Move()
    {
        PlayerController player = ObjectManager.Instance.Player;

        if (player == null) return;

        // 플레이어와의 거리 계산 (Y축 무시)
        _distance = Vector3.Distance(new Vector3(player.transform.position.x, 0.0f, 0.0f),
                                     new Vector3(transform.position.x, 0.0f, 0.0f));

        Vector3 targetPos = Vector3.zero;
        Vector3 targetDir = Vector3.zero;

        if (_isPlayerAlive)
        {
            // 목표 위치 및 방향 설정
            targetPos = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
            targetDir = (targetPos - transform.position).normalized;

            if (_distance > _stopDistance)
            {
                // 공격 모드 해제 및 이동 시작
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
                // 공격 모드 진입
                _isAttackMode = true;
                _animator.SetBool(Define.IsMoveHash, false);
                _animator.SetBool(Define.IsAttackModeHash, true);

                if (_coAttack == null) _coAttack = StartCoroutine(CoAttack());
            }

            // 플레이어 방향을 바라보도록 처리
            _spriteRenderer.flipX = (targetDir.x > 0.0f);
        }
        else
        {
            _isAttackMode = false;
            _animator.SetBool(Define.IsMoveHash, false);
            _animator.SetBool(Define.IsAttackModeHash, false);
        }
    }

    // 공격 처리
    private IEnumerator CoAttack()
    {
        if (_isAttackMode)
        {
            _isAttack = true;

            // 공격 방향에 맞춰 공격 영역 위치 변경
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

    // 피격 처리
    private IEnumerator CoDamaged()
    {
        _isDamaged = true;
        _animator.SetTrigger(Define.Hurt);

        yield return _stunTimer;

        _isDamaged = false;
        _coDamaged = null;
    }

    // 사망 처리
    private IEnumerator CoDeath()
    {
        _rigidbody2D.gravityScale = 0.0f;
        _capsuleCollider2D.isTrigger = true;

        // 사망 상태 전환
        _isAlive = false;
        _isAttackMode = false;
        _isAttack = false;

        _animator.SetTrigger(Define.Death);

        // 투명도가 0이 될 때까지 반복
        while (_color.a > 0)
        {
            // 투명도 감소
            _color.a -= 0.05f;
            // 새로운 색상 적용
            _spriteRenderer.color = new Color(_color.r, _color.g, _color.b, _color.a);

            yield return _destroyTimer;
        }

        // 적 오브젝트 비활성화
        ObjectManager.Instance.Despawn(this);
    }

    // 플레이어 생존 여부 체크
    private void PlayerAliveCheck()
    {
        _isPlayerAlive = GameManager.Instance.PlayerInfo.IsAlive;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 지면에 닿았는지 확인
        if (collision.collider.CompareTag(Define.GroundTag))
        {
            _animator.SetBool(Define.IsGroundHash, true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어 공격을 맞았을 때 처리
        if (collision.CompareTag(Define.PlayerAttackTag) && _isPlayerAlive && _isAlive)
        {
            bool isCritical = GameManager.Instance.Critical >= Random.Range(1.0f, 100.0f) ? true : false;
            float fDamage = GameManager.Instance.Atk;

            // 치명타 피해 적용 데미지
            if (isCritical) fDamage += GameManager.Instance.Atk * GameManager.Instance.CriticalDamage;

            _enemyCurrentHp -= fDamage;

            if (_enemyCurrentHp <= 0.0f)
            {
                if (_coDeath == null) _coDeath = StartCoroutine(CoDeath());

                // 골드 획득
                GameManager.Instance.Gold += _enemyGold;
                // 경험치 획득
                GameManager.Instance.TotalExp += _enemyExp;
            }
            else
            {
                if (_coDamaged == null) _coDamaged = StartCoroutine(CoDamaged());
            }
        }
    }
}