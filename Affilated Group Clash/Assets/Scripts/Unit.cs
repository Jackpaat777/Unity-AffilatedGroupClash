using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;

public enum UnitType
{
    Warrior,
    Tanker,
    Ranger,
    Buffer,
    Special,
    Base
}

// 변수위치를 바꾸면 ObjectManager 인스펙터에서 프리펩 위치도 바꿔줘야함
public enum UnitDetail
{
    // Bullet A 사용 유닛
    Archer, Sniper, Farmer, Guitar, Singer, Bass, Base,
    // Bullet B 사용 유닛
    Bomb, Stick, Vampire, Punch, Hammer, Devil, Heal, Wizard, AtkUp, AtkspdUp, SpdUp, Piano,
    Sword, Guard, Berserker, GrowUp, Air, Counter, Shield, Drum, Cat,
    CostUp
}

public enum UnitState
{
    Idle,
    Move,
    Attack,
    Hit,
    Die
}
public enum SkillSensor
{
    NONE,
    ATK,
    ATS,
    SPD,
}

public class Unit : MonoBehaviour
{
    [Header("---------------[Stat]")]
    public string unitName;
    public UnitType unitType;
    public UnitDetail unitDetail;
    public int unitCost;
    public int unitMaxHp;
    public int unitHp;
    public int unitAtk;
    public float unitAtkSpeed;
    public float unitRange;
    public float unitSpeed;

    [Header("---------------[Game]")]
    public Slider hpSlider;
    public ParticleSystem dustObject;
    public bool isHeal;         // 힐 관련 변수
    public UnitState unitState; // 유닛의 상태마다 다른 로직을 실행하도록
    float attackTimer;          // 공속용 타이머
    float stopTimer;            // Move용 타이머
    bool isFront;               // Stop에서 Move로 가기 위한 변수
    bool isHit;

    [Header("---------------[Skill]")]
    public SkillSensor skillSensor; // skillSensor를 통해 버프를 제어하므로 버프중첩이 안됨 -> 따로 bool변수로 제작?
    public bool isATSUp;
    public bool isATKUp;
    public bool isSPDUp;
    public bool isAtkDebuff;
    public bool isAtsDebuff;
    public bool isSpdDebuff;
    public bool isNoDamage;
    public bool isBackStep;
    public float atkDebuffTimer;
    public float atsDebuffTimer;
    public float spdDebuffTimer;
    public float backStepTimer;
    int farmerCount;
    int plusAtk;

    public GameObject growUpHand;
    public GameObject barrierSkillEffect;

    GameObject atkSensor;
    GameObject atsSensor;
    GameObject spdSensor;
    GameObject bulSensor;
    Animator anim;
    Collider2D col;

    void Awake()
    {
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
    }

    // 유닛을 클릭했을 때
    void OnMouseDown()
    {
        // Base 타입인 경우
        if (unitType == UnitType.Base)
            return;

        // 유닛 정보 넘기기
        InGameManager.instance.isUnitClick = true;
        InGameManager.instance.unitObj = gameObject;
    }

    // 재사용 시 초기화 (Awake의 역할)
    void OnEnable()
    {
        // Base 타입인 경우
        if (unitType == UnitType.Base)
            return;

        // 기본 세팅하기
        ResetSettings();

        // 스탯 변화가 있는 경우 초기화
        ResetStat();

        // 센서 사용 유닛은 센서 초기화
        ResetSensor();

        // 중복소환 방지
        SummonOnce();

        // Barrier의 경우
        isNoDamage = false;
        if (unitDetail == UnitDetail.Hammer)
        {
            isNoDamage = true;
            barrierSkillEffect.SetActive(true);
        }
    }
    // ======================================================= OnEnable 정리용 함수
    void ResetSettings()
    {
        // 위치 초기화
        if (gameObject.layer == 8)
            transform.localPosition = Vector3.left * 11;
        else if (gameObject.layer == 9)
            transform.localPosition = Vector3.right * 11;

        // 체력 바
        hpSlider.transform.position = Vector3.up * 2000;

        // 타이머 초기화
        attackTimer = 0; stopTimer = 0; atkDebuffTimer = 0; atsDebuffTimer = 0; spdDebuffTimer = 0; backStepTimer = 0;
        // 값 초기화
        farmerCount = 0;

        // 상태 설정
        unitState = UnitState.Idle;
        isFront = false;
        isHit = false;
        isHeal = false;
        isBackStep = true;
        DoMove();

        // Collider
        col.enabled = true;
    }
    void ResetStat()
    {
        // 난이도에 따라 적 최대체력 변경 (죽거나 게임이 끝나면 다시 원래대로) -> 게임이 끝났는데 적이 살아있을 수가 있나?
        if (gameObject.layer == 9)
        {
            if (Variables.gameLevel == 0)
                unitMaxHp -= 5;
            else if (Variables.gameLevel == 4)
                unitMaxHp += 5;
        }
        
        // 체력 초기화
        unitHp = unitMaxHp;

        // 변한 스탯 값 초기화
        if (isAtkDebuff)
        {
            unitAtk += 5;
            isAtkDebuff = false;
        }
        if (isAtsDebuff)
        {
            unitAtkSpeed /= 2;
            isAtsDebuff = false;
        }
        if (isSpdDebuff)
        {
            if (gameObject.layer == 8)
                unitSpeed += 0.3f;
            else if (gameObject.layer == 9)
                unitSpeed -= 0.3f;
            isSpdDebuff = false;
        }

        // 모든 유닛에 적용
        if (skillSensor == SkillSensor.ATK)
        {
            unitAtk -= 5;
            isATKUp = false;
        }
        else if (skillSensor == SkillSensor.ATS)
        {
            unitAtkSpeed *= 2;
            isATSUp = false;
        }
        else if (skillSensor == SkillSensor.SPD)
        {
            if (gameObject.layer == 8)
                unitSpeed -= 1f;
            else if (gameObject.layer == 9)
                unitSpeed += 1f;
            isATKUp = false;
        }

        // 스탯 값 변화 유닛 초기화
        if (unitDetail == UnitDetail.Bomb)
        {
            if (gameObject.layer == 8)
                unitSpeed = 0.8f;
            else if (gameObject.layer == 9)
                unitSpeed = -0.8f;
        }

        if (unitDetail == UnitDetail.Guitar)
            unitAtkSpeed = 1.5f;

        if (unitDetail == UnitDetail.Berserker)
            unitAtkSpeed = 1.5f;

        if (unitDetail == UnitDetail.GrowUp)
        {
            unitAtk = 10;
            unitAtkSpeed = 1.5f;
            unitMaxHp = 50;
            unitHp = 50;
        }
    }
    void ResetSensor()
    {
        // 센서
        int idx = ((int)unitDetail - (int)UnitDetail.Bomb) + ((int)UnitDetail.Base + 1) * 2;
        if (unitDetail == UnitDetail.AtkUp)
        {
            if (gameObject.layer == 8)
            {
                atkSensor = ObjectManager.instance.GetBullet(idx, transform.position + Vector3.left * 0.5f);
                atkSensor.gameObject.tag = "Atk_UpB";
            }
            else if (gameObject.layer == 9)
            {
                atkSensor = ObjectManager.instance.GetBullet(idx, transform.position + Vector3.right * 0.5f);
                atkSensor.gameObject.tag = "Atk_UpR";
            }
        }
        else if (unitDetail == UnitDetail.AtkspdUp)
        {
            if (gameObject.layer == 8)
            {
                atsSensor = ObjectManager.instance.GetBullet(idx, transform.position + Vector3.left * 0.5f);
                atsSensor.gameObject.tag = "AtkSpd_UpB";
            }
            else if (gameObject.layer == 9)
            {
                atsSensor = ObjectManager.instance.GetBullet(idx, transform.position + Vector3.right * 0.5f);
                atsSensor.gameObject.tag = "AtkSpd_UpR";
            }
        }
        else if (unitDetail == UnitDetail.SpdUp)
        {
            if (gameObject.layer == 8)
            {
                spdSensor = ObjectManager.instance.GetBullet(idx, transform.position + Vector3.left * 0.5f);
                spdSensor.gameObject.tag = "Spd_UpB";
            }
            else if (gameObject.layer == 9)
            {
                spdSensor = ObjectManager.instance.GetBullet(idx, transform.position + Vector3.right * 0.5f);
                spdSensor.gameObject.tag = "Spd_UpR";
            }
        }
        else if (unitDetail == UnitDetail.Piano)
        {
            if (gameObject.layer == 8)
            {
                bulSensor = ObjectManager.instance.GetBullet(idx, transform.position + Vector3.left * 0.5f);
                bulSensor.gameObject.layer = 10;
            }
            else if (gameObject.layer == 9)
            {
                bulSensor = ObjectManager.instance.GetBullet(idx, transform.position + Vector3.right * 0.5f);
                bulSensor.gameObject.layer = 11;
            }
        }
    }
    void SummonOnce()
    {
        // Devil의 경우
        if (unitDetail == UnitDetail.Devil)
        {
            if (gameObject.layer == 8)
                InGameManager.instance.isDevilB = true;
            else if (gameObject.layer == 9)
                InGameManager.instance.isDevilR = true;
        }
        // Cost의 경우
        else if (unitDetail == UnitDetail.CostUp)
        {
            if (gameObject.layer == 8)
                InGameManager.instance.isCostB = true;
            else if (gameObject.layer == 9)
                InGameManager.instance.isCostR = true;
        }
        // Heal의 경우
        else if (unitDetail == UnitDetail.Heal)
        {
            if (gameObject.layer == 8)
                InGameManager.instance.isHealB = true;
            else if (gameObject.layer == 9)
                InGameManager.instance.isHealR = true;
        }
    }

    void Update()
    {
        // 게임 종료
        if (!InGameManager.instance.isGameLive)
            return;

        // Base 타입인 경우
        if (unitType == UnitType.Base)
            return;

        // 상대팀 Devil의 공격
        DevilHit();
        // BackStep
        BackStepTimer();
        // 버프 유닛 (일정시간마다 공격모션)
        BuffUnit();
        // 디버프 상태에 걸렸을 때
        DebuffTimer();
        // 버프 센서 -> 센서 안에 들어와 있으면 버프 적용
        BuffSensor();
        // 유닛 이동
        UnitMovement();
        HpSlider();
    }
    // ======================================================= Update 정리용 함수
    void DevilHit()
    {
        if ((InGameManager.instance.isDevilBAttack && gameObject.layer == 9) || (InGameManager.instance.isDevilRAttack && gameObject.layer == 8))
        {
            // 이펙트 위치조정 벡터
            Vector3 vec = Vector3.zero;
            if (gameObject.layer == 8)
                vec = new Vector3(-0.5f, 1.5f);
            else if (gameObject.layer == 9)
                vec = new Vector3(0.5f, 1.5f);

            // 이펙트
            int idx = (int)UnitDetail.Devil - (int)UnitDetail.Bomb + ((int)UnitDetail.Base + 1) * 2;
            ObjectManager.instance.GetBullet(idx, transform.position + vec);
            // Hit >> 데미지 (5f)
            DoHit(5);
        }
    }
    void BackStepTimer()
    {
        if (!isBackStep)
        {
            backStepTimer += Time.deltaTime;
            if (backStepTimer > 4f)
            {
                isBackStep = true;
                backStepTimer = 0;
            }
        }
    }
    void BuffUnit()
    {
        // 특수유닛
        if (unitDetail == UnitDetail.CostUp || unitDetail == UnitDetail.Devil || unitDetail == UnitDetail.AtkUp ||
            unitDetail == UnitDetail.AtkspdUp || unitDetail == UnitDetail.SpdUp || unitDetail == UnitDetail.Piano)
        {
            // 센서 위치 조정
            if (unitDetail == UnitDetail.AtkUp)
            {
                if (gameObject.layer == 8)
                    atkSensor.transform.position = transform.position + Vector3.left * 0.5f;
                else if (gameObject.layer == 9)
                    atkSensor.transform.position = transform.position + Vector3.right * 0.5f;
            }
            else if (unitDetail == UnitDetail.AtkspdUp)
            {
                if (gameObject.layer == 8)
                    atsSensor.transform.position = transform.position + Vector3.left * 0.5f;
                else if (gameObject.layer == 9)
                    atsSensor.transform.position = transform.position + Vector3.right * 0.5f;
            }
            else if (unitDetail == UnitDetail.SpdUp)
            {
                if (gameObject.layer == 8)
                    spdSensor.transform.position = transform.position + Vector3.left * 0.5f;
                else if (gameObject.layer == 9)
                    spdSensor.transform.position = transform.position + Vector3.right * 0.5f;
            }
            else if (unitDetail == UnitDetail.Piano)
            {
                if (gameObject.layer == 8)
                    bulSensor.transform.position = transform.position + Vector3.left * 0.5f;
                else if (gameObject.layer == 9)
                    bulSensor.transform.position = transform.position + Vector3.right * 0.5f;
            }

            // Attack Animation
            attackTimer += Time.deltaTime;
            if (attackTimer > unitAtkSpeed)
            {
                // Stop -> doMove 애니메이터로 넘어가기 위해 멈추기 필요
                DoStop();

                // CostUp
                if (unitDetail == UnitDetail.CostUp)
                {
                    if (gameObject.layer == 8 && InGameManager.instance.blueCost < 10)
                        InGameManager.instance.blueCost += 1;
                    else if (gameObject.layer == 9 && InGameManager.instance.redCost < 10)
                        InGameManager.instance.redCost += 1;

                    // Sound
                    SoundManager.instance.SfxPlay("Coin");
                }
                else if (unitDetail == UnitDetail.Devil)
                {
                    SoundManager.instance.SfxPlay("Devil");
                }

                // Animation
                anim.SetTrigger("doAttack");
                attackTimer = 0;
            }
        }
    }
    void DebuffTimer()
    {
        // 공격 디버프 (2초뒤에 다시 올림)
        if (isAtkDebuff)
        {
            atkDebuffTimer += Time.deltaTime;
            if (atkDebuffTimer > 2f)
            {
                unitAtk += 5;
                isAtkDebuff = false;
            }
        }
        // 공속 디버프 (2초뒤에 다시 올림)
        if (isAtsDebuff)
        {
            atsDebuffTimer += Time.deltaTime;
            if (atsDebuffTimer > 2f)
            {
                unitAtkSpeed /= 2;
                isAtsDebuff = false;
            }
        }
        // 이속 디버프 (2초뒤에 다시 올림)
        if (isSpdDebuff)
        {
            spdDebuffTimer += Time.deltaTime;
            if (spdDebuffTimer > 2f)
            {
                if (gameObject.layer == 8)
                    unitSpeed += 0.3f;
                else if (gameObject.layer == 9)
                    unitSpeed -= 0.3f;
                isSpdDebuff = false;
            }
        }
    }
    void BuffSensor()
    {
        // 공격력 버프
        if (skillSensor == SkillSensor.ATK)
        {
            // 버프 발동 (한번만 실행) -> 실행이후 isATKUp가 true가 되어 더이상 공격력증가 로직을 실행하지 않음
            if (!isATKUp)
            {
                // 공격력 증가
                unitAtk += 5;
                isATKUp = true;
            }
        }
        // 공속 버프
        if (skillSensor == SkillSensor.ATS)
        {
            if (!isATSUp)
            {
                unitAtkSpeed /= 2;
                unitAtkSpeed = unitAtkSpeed < 0.3f ? 0.3f : unitAtkSpeed;
                isATSUp = true;
            }
        }
        // 스피드 버프
        if (skillSensor == SkillSensor.SPD)
        {
            if (!isSPDUp)
            {
                if (gameObject.layer == 8)
                    unitSpeed += 1f;
                else if (gameObject.layer == 9)
                    unitSpeed -= 1f;
                isSPDUp = true;
            }
        }
        // 센서 밖
        if (skillSensor == SkillSensor.NONE)
        {
            // 버프 중인 경우 버프 해제 (한번만 실행)
            if (isATKUp)
            {
                unitAtk -= 5;
                isATKUp = false;
            }
            else if (isATSUp)
            {
                unitAtkSpeed *= 2;
                isATSUp = false;
            }
            else if (isSPDUp)
            {
                if (gameObject.layer == 8)
                    unitSpeed -= 1f;
                else if (gameObject.layer == 9)
                    unitSpeed += 1f;
                isSPDUp = false;
            }
        }
    }
    void UnitMovement()
    {
        // 유닛 이동 구현
        if (unitState == UnitState.Move)
        {
            Vector3 nextMove = Vector3.right * unitSpeed * Time.deltaTime;
            transform.Translate(nextMove);

            // attackTimer 초기화 -> 코스트업 제외 모든 유닛은 멈춰있을 때만 공격 가능
            if (unitDetail != UnitDetail.CostUp)
                attackTimer = 0;
        }
        else if (unitState == UnitState.Idle || unitState == UnitState.Hit)
        {
            // 앞에 아무도 없을 때
            if (!isFront)
            {
                // Bomb와 Cat은 피격시 멈추지 않음
                if (unitDetail == UnitDetail.Bomb || unitDetail == UnitDetail.Cat)
                {
                    DoMove();
                }
                // 일정시간 뒤에 다시 움직임
                stopTimer += Time.deltaTime;
                if (stopTimer > 0.5f)
                {
                    DoMove();
                    stopTimer = 0;
                }
            }
        }
    }
    void HpSlider()
    {
        // 슬라이더 체력 표시
        hpSlider.value = (float)unitHp / unitMaxHp;

        // 슬라이더 이동
        Vector3 vec = Vector3.zero;
        if (gameObject.layer == 8)
            vec = new Vector3(-0.5f, 1.3f);
        else if (gameObject.layer == 9)
            vec = new Vector3(0.5f, 1.3f);
        hpSlider.transform.position = Camera.main.WorldToScreenPoint(transform.position + vec);
    }

    void FixedUpdate()
    {
        // 게임 종료
        if (!InGameManager.instance.isGameLive)
            return;

        // RayCast를 사용하기 때문에(RigidBody) FixedUpdate에서 호출 
        ScanEnemy();
    }

    // ======================================================= 타겟 감지 함수
    void ScanEnemy()
    {
        if (unitState == UnitState.Die)
            return;

        Vector2 dir = Vector2.zero;
        string enemyLayer = "";

        if (gameObject.layer == 8)      // Blue 팀 유닛
        {
            dir = Vector2.right;
            enemyLayer = "Red";
        }
        else if (gameObject.layer == 9) // Red 팀 유닛
        {
            dir = Vector2.left;
            enemyLayer = "Blue";
        }

        // RayCast
        RaycastHit2D rayHit = Physics2D.Raycast(transform.position, dir, unitRange, LayerMask.GetMask(enemyLayer));

        // 적군이 감지됨
        if (rayHit.collider != null)
        {
            // 적 오브젝트 가져오기
            Unit enemyLogic = rayHit.collider.gameObject.GetComponent<Unit>();

            // 광역 유닛의 경우 (Wizard는 Transform 변수 필요) (Stick은 Warrior지만 다른 함수 적용)
            if (unitDetail == UnitDetail.Stick || unitDetail == UnitDetail.Bomb || unitDetail == UnitDetail.Wizard)
                WideAttack(enemyLogic.transform);

            // 근접 유닛의 경우 (Unit 변수 필요)
            else if (unitType == UnitType.Warrior || unitType == UnitType.Tanker)
                DirectAttack(enemyLogic);

            // 원거리 유닛의 경우 + Base의 경우
            else if (unitType == UnitType.Ranger || unitType == UnitType.Base)
                ShotAttack();

            // Cat의 경우
            else if (unitDetail == UnitDetail.Cat)
                CatAttack(enemyLogic);

            // 버퍼의 경우
            else if (unitType == UnitType.Buffer || unitDetail == UnitDetail.Devil)
            {
                // 멈춤 (공격X)
                DoStop();
                isFront = true;
            }
        }
        else
        {
            isFront = false;
        }

        // Bomb 레이
        if (unitDetail == UnitDetail.Bomb)
        {
            RaycastHit2D bombRayHit = Physics2D.Raycast(transform.position, dir, 4f, LayerMask.GetMask(enemyLayer));
            if (bombRayHit.collider != null)
            {
                anim.SetTrigger("doAttack");
                if (gameObject.layer == 8)
                    unitSpeed = 5f;
                else if (gameObject.layer == 9)
                    unitSpeed = -5f;
            }
        }
        // Archer 레이
        else if (unitDetail == UnitDetail.Archer)
        {
            RaycastHit2D bombRayHit = Physics2D.Raycast(transform.position, dir, 1f, LayerMask.GetMask(enemyLayer));
            if (bombRayHit.collider != null)
            {
                // 백스탭이 가능한 상태에서만 (2초 쿨타임)
                if (isBackStep)
                {
                    // 백스탭 실행 (제한거리 있음)
                    if (gameObject.layer == 8 && transform.position.x > -10f)
                        transform.Translate(Vector3.left * 1f);
                    else if (gameObject.layer == 9 && transform.position.x < 10f)
                        transform.Translate(Vector3.right * 1f);
                    // 2초간 백스탭 불가능
                    isBackStep = false;
                }
            }
        }
    }

    // ======================================================= 공격 함수
    void CatAttack(Unit enemyLogic)
    {
        // Cat은 근접상대 무시
        if (unitDetail == UnitDetail.Cat)
        {
            // Base앞에선 멈춤
            if (enemyLogic.unitDetail == UnitDetail.Base)
            {
                // 멈춤
                isFront = true;
                DoStop();
            }

            // 공격속도에 따라서 어택
            attackTimer += Time.deltaTime;
            if (attackTimer < unitAtkSpeed)
                return;

            // Attack
            enemyLogic.DoHit(unitAtk);
            anim.SetTrigger("doAttack");
            attackTimer = 0;

            // Sound
            SoundManager.instance.SfxPlay("Sword");
        }
    }
    void DirectAttack(Unit enemyLogic)
    {
        // Cat은 근접상대 무시
        if (unitDetail == UnitDetail.Cat)
        {
            // 근접상대인 경우 return
            if (enemyLogic.unitType == UnitType.Warrior || enemyLogic.unitType == UnitType.Tanker ||
                enemyLogic.unitDetail == UnitDetail.Stick || enemyLogic.unitDetail == UnitDetail.Bomb)
                return;
        }

        // 멈춤
        isFront = true;
        DoStop();

        // 공격속도에 따라서 어택
        attackTimer += Time.deltaTime;
        if (attackTimer < unitAtkSpeed)
            return;

        // Skill
        int idx = (int)unitDetail - (int)UnitDetail.Bomb;
        // Vampire는 공격 시 체력 회복
        if (unitDetail == UnitDetail.Vampire)
        {
            // 회복 이펙트
            ObjectManager.instance.GetBullet(idx + ((int)UnitDetail.Base + 1) * 2, transform.position + Vector3.up * 0.5f);
            unitHp += unitAtk;
            unitHp = unitHp > unitMaxHp ? unitMaxHp : unitHp;
        }
        // Punch는 상대 체력이 50이하면 즉사시킴
        else if (unitDetail == UnitDetail.Punch)
        {
            // Base는 해당안됨
            if (enemyLogic.unitDetail != UnitDetail.Base)
            {
                // 즉사
                if (enemyLogic.unitHp <= 30)
                {
                    enemyLogic.DoHit(30);
                    // 이펙트
                    if (gameObject.layer == 8)
                    {
                        Vector3 vec = new Vector3(0.5f, 0.5f);
                        ObjectManager.instance.GetBullet(idx + ((int)UnitDetail.Base + 1) * 2, transform.position + vec);
                    }
                    else if (gameObject.layer == 9)
                    {
                        Vector3 vec = new Vector3(-0.5f, 0.5f);
                        ObjectManager.instance.GetBullet(idx + ((int)UnitDetail.Base + 1) * 2, transform.position + vec);
                    }
                }
            }
        }
        // Grow는 적을 죽일수록 스탯 강화
        else if (unitDetail == UnitDetail.GrowUp)
        {
            Animator anim = growUpHand.GetComponent<Animator>();
            if (gameObject.layer == 8)
                anim.SetTrigger("doAttackB");
            else if (gameObject.layer == 9)
                anim.SetTrigger("doAttackR");

            // Base는 해당안됨
            if (enemyLogic.unitDetail != UnitDetail.Base)
            {
                // 적의 HP가 내 공격력이하만큼 있으면 (죽을 경우 -> 상대방이 힐 유닛이면? 힐되기 전에 죽었을 듯)
                if (enemyLogic.unitHp <= unitAtk)
                {
                    unitAtk += 1;
                    unitAtkSpeed -= 0.05f;
                    unitAtkSpeed = unitAtkSpeed < 0.3f ? 0.3f : unitAtkSpeed;
                    unitMaxHp += 5;
                }
            }
        }
        // Air는 공격 시 상대 공속 절반
        else if (unitDetail == UnitDetail.Air && !enemyLogic.isAtsDebuff)
        {
            if (enemyLogic.unitDetail != UnitDetail.Base)
            {
                enemyLogic.unitAtkSpeed *= 2;
                enemyLogic.isAtsDebuff = true;
                enemyLogic.atsDebuffTimer = 0;
            }
        }
        // Hammer는 공격 시 베리어 어택 True
        else if (unitDetail == UnitDetail.Hammer)
        {
            // 이펙트
            if (gameObject.layer == 8)
                ObjectManager.instance.GetBullet(idx + ((int)UnitDetail.Base + 1) * 2, transform.position + Vector3.right * 0.5f);
            else if (gameObject.layer == 9)
                ObjectManager.instance.GetBullet(idx + ((int)UnitDetail.Base + 1) * 2, transform.position + Vector3.left * 0.5f);
            barrierSkillEffect.SetActive(false);
            isNoDamage = false;
        }
        // Drum은 적을 밀침
        else if (unitDetail == UnitDetail.Drum)
        {
            // 적 밀치기 (최댓값 존재)
            if (gameObject.layer == 8 && enemyLogic.transform.position.x < 10f)
                enemyLogic.transform.Translate(Vector3.right * 0.5f);
            else if (gameObject.layer == 9 && enemyLogic.transform.position.x > -10f)
                enemyLogic.transform.Translate(Vector3.left * 0.5f);
        }

        // 근접유닛이 Counter를 때리면 자신도 절반 데미지
        if (enemyLogic.unitDetail == UnitDetail.Counter)
            DoHit(unitAtk / 2);

        // Attack
        enemyLogic.DoHit(unitAtk);
        anim.SetTrigger("doAttack");
        attackTimer = 0;


        // Sound
        if (unitDetail == UnitDetail.Punch || unitDetail == UnitDetail.Hammer)
            SoundManager.instance.SfxPlay("Crash");
        else if (unitDetail == UnitDetail.Berserker)
            SoundManager.instance.SfxPlay("Berserker");
        else if (unitDetail == UnitDetail.GrowUp)
            SoundManager.instance.SfxPlay("Grow");
        else if (unitType == UnitType.Warrior || unitDetail == UnitDetail.Cat)
            SoundManager.instance.SfxPlay("Sword");
        else
            SoundManager.instance.SfxPlay("Guard");
    }
    void ShotAttack()
    {
        // 멈춤
        isFront = true;
        DoStop();

        // 공격속도에 따라서 실행
        attackTimer += Time.deltaTime;
        if (attackTimer < unitAtkSpeed)
            return;


        // Bullet
        GameObject bullet = null;
        int idx = (int)unitDetail;
        Vector3 vec = Vector3.zero;

        if (gameObject.layer == 8)          // 블루 팀 유닛일 경우
        {
            // Base의 경우
            if (unitDetail == UnitDetail.Base)
                vec = new Vector3(0.5f, 0);

            // 자신을 기준으로 총알 발사
            bullet = ObjectManager.instance.GetBullet(idx, transform.position + Vector3.up * 0.2f + vec);
        }
        else if (gameObject.layer == 9)     // 레드 팀 유닛일 경우
        {
            if (unitDetail == UnitDetail.Base)
                vec = new Vector3(-0.5f, 0);

            bullet = ObjectManager.instance.GetBullet(idx + ((int)UnitDetail.Base + 1), transform.position + Vector3.up * 0.2f + vec); // => Bullet A 사용 유닛 개수 추가
        }

        // Skill
        if (unitDetail == UnitDetail.Guitar)
        {
            unitAtkSpeed -= 0.05f;
            unitAtkSpeed = unitAtkSpeed < 0.3f ? 0.3f : unitAtkSpeed;
        }
        if (unitDetail == UnitDetail.Farmer)
        {
            farmerCount++;
            if (farmerCount == 3)
            {
                SpriteRenderer spriteRen = bullet.GetComponent<SpriteRenderer>();
                spriteRen.color = Color.red;
                plusAtk = 5;
                farmerCount = 0;
            }
            else
            {
                SpriteRenderer spriteRen = bullet.GetComponent<SpriteRenderer>();
                spriteRen.color = Color.white;
                plusAtk = 0;
            }
        }

        // Bullet에 값 넘겨주기
        BulletSetting(bullet, unitDetail, gameObject.layer, unitAtk + plusAtk);

        // Attack
        if (unitType != UnitType.Base)
            anim.SetTrigger("doAttack");
        attackTimer = 0;

        // Sound
        if (unitDetail == UnitDetail.Base)
            SoundManager.instance.SfxPlay("Base");
        else if (unitDetail == UnitDetail.Sniper)
            SoundManager.instance.SfxPlay("Gun");
        else
            SoundManager.instance.SfxPlay("Archer");
    }
    void WideAttack(Transform targetTrans)
    {
        // 이펙트 가져오기
        GameObject bullet = null;

        int idx = ((int)unitDetail - (int)UnitDetail.Bomb) + ((int)UnitDetail.Base + 1) * 2;
        if (unitDetail == UnitDetail.Bomb)
        {
            // 자폭
            DoHit(unitMaxHp);
            // 이펙트
            bullet = ObjectManager.instance.GetBullet(idx, transform.position); // 12 => Bullet A 사용 유닛 * 2

            // Sound
            SoundManager.instance.SfxPlay("Bomb");
        }
        else // 공속에 영향을 받는 유닛들
        {
            // 멈춤
            isFront = true;
            DoStop();

            // 공격속도에 따라서 실행
            attackTimer += Time.deltaTime;
            if (attackTimer < unitAtkSpeed)
                return;

            // 자신을 기준으로 총알 발사
            if (unitDetail == UnitDetail.Stick)
            {
                if (gameObject.layer == 8)
                    bullet = ObjectManager.instance.GetBullet(idx, transform.position + Vector3.right * 0.5f);
                else if (gameObject.layer == 9)
                    bullet = ObjectManager.instance.GetBullet(idx, transform.position + Vector3.left * 0.5f);

                // Sound
                SoundManager.instance.SfxPlay("Crash");
            }
            // 타겟를 기준으로 총알 발사
            else if (unitDetail == UnitDetail.Wizard)
            {
                if (gameObject.layer == 8)
                    bullet = ObjectManager.instance.GetBullet(idx, targetTrans.position + Vector3.right * 0.5f);
                else if (gameObject.layer == 9)
                    bullet = ObjectManager.instance.GetBullet(idx, targetTrans.position + Vector3.left * 0.5f);

                // Sound
                SoundManager.instance.SfxPlay("Magic");
            }

            // Attack
            if (unitType != UnitType.Base)
                anim.SetTrigger("doAttack");
            attackTimer = 0;
        }

        // Bullet에 값 넘겨주기
        BulletSetting(bullet, unitDetail, gameObject.layer, unitAtk);
    }
    void BulletSetting(GameObject bulletObj, UnitDetail uDetail, int uLayer, int uDmg)
    {
        Bullet bulletLogic = bulletObj.GetComponent<Bullet>();

        bulletLogic.unitDetail = uDetail;
        bulletLogic.layer = uLayer;
        bulletLogic.dmg = uDmg;
    }

    // ======================================================= 버프 함수
    public void DoBuff(Unit allyLogic)
    {
        // Bullet
        int idx = ((int)unitDetail - (int)UnitDetail.Bomb) + ((int)UnitDetail.Base + 1) * 2;

        if (gameObject.layer == 8)          // 블루 팀 유닛일 경우
        {
            // 타겟을 기준으로 발사
            if (unitDetail == UnitDetail.Heal)
            {
                allyLogic.unitHp += 15;
                allyLogic.unitHp = allyLogic.unitHp > allyLogic.unitMaxHp ? allyLogic.unitMaxHp : allyLogic.unitHp;
                Vector3 vec = new Vector3(-0.5f, 0.5f);
                ObjectManager.instance.GetBullet(idx, allyLogic.transform.position + vec);
            }
        }
        else if (gameObject.layer == 9)     // 레드 팀 유닛일 경우
        {
            if (unitDetail == UnitDetail.Heal)
            {
                allyLogic.unitHp += 15;
                allyLogic.unitHp = allyLogic.unitHp > allyLogic.unitMaxHp ? allyLogic.unitMaxHp : allyLogic.unitHp;
                Vector3 vec = new Vector3(0.5f, 0.5f);
                ObjectManager.instance.GetBullet(idx, allyLogic.transform.position + vec);
            }
        }

        // 버프 발동
        anim.SetTrigger("doAttack");

        if (isHit)
        {
            anim.SetTrigger("doHit");
            isHit = false;
        }

        // Sound
        SoundManager.instance.SfxPlay("Heal");
    }

    // ======================================================= 상태 함수
    void DoMove()
    {
        // 이미 움직이고 있으면 호출 X
        if (unitState == UnitState.Move)
            return;

        // Guitar는 움직이면 공속 초기화
        if (unitDetail == UnitDetail.Guitar)
            unitAtkSpeed = 1.5f;

        unitState = UnitState.Move;
        anim.SetTrigger("doMove");
        if (dustObject != null)
            dustObject.Play();
    }
    void DoStop()
    {
        // 안움직이고 있다면 호출 X
        if (unitState != UnitState.Move)
            return;

        unitState = UnitState.Idle;
        anim.SetTrigger("doStop");
        if (dustObject != null)
            dustObject.Stop();

        stopTimer = 0;
    }
    public void DoHit(int damage)
    {
        // Base 타입인 경우
        if (unitType == UnitType.Base)
        {
            InGameManager.instance.BaseHit(damage, gameObject.layer);
            return;
        }

        // Barrier가 공격한적이 없다면 맞지않음
        if (isNoDamage)
            return;

        if (unitDetail == UnitDetail.Berserker)
        {
            // 깎인체력 10 당 0.05씩 공속 증가
            unitAtkSpeed -= damage * 0.005f;
            unitAtkSpeed = unitAtkSpeed < 0.3f ? 0.3f : unitAtkSpeed;
        }
        else if (unitDetail == UnitDetail.Shield)
        {
            // 3데미지 감소
            damage -= 3;
            damage = damage < 0 ? 0 : damage;
        }

        // Hit 로직
        unitState = UnitState.Hit;
        unitHp -= damage;
        isHit = true;
        anim.SetTrigger("doHit");

        stopTimer = 0;

        if (dustObject != null)
            dustObject.Stop();

        if (unitHp <= 0)
        {
            unitHp = 0;
            DoDie();
            return;
        }
    }
    void DoDie()
    {
        // 체력 원래대로
        if (gameObject.layer == 9)
        {
            if (Variables.gameLevel == 0)
                unitMaxHp += 5;
            else if (Variables.gameLevel == 4)
                unitMaxHp -= 5;
        }

        // Sensor 끄기
        if (unitDetail == UnitDetail.AtkUp)
            atkSensor.SetActive(false);
        else if (unitDetail == UnitDetail.AtkspdUp)
            atsSensor.SetActive(false);
        else if (unitDetail == UnitDetail.SpdUp)
            spdSensor.SetActive(false);
        else if (unitDetail == UnitDetail.Piano)
            bulSensor.SetActive(false);

        // Devil
        if (unitDetail == UnitDetail.Devil)
        {
            if (gameObject.layer == 8)
            {
                InGameManager.instance.isDevilB = false;
                InGameManager.instance.isDevilBAttack = false;
                InGameManager.instance.devilBTimer = 0;
            }
            else if(gameObject.layer == 9)
            {
                InGameManager.instance.isDevilR = false;
                InGameManager.instance.isDevilRAttack = false;
                InGameManager.instance.devilRTimer = 0;
            }
        }
        // Cost
        if (unitDetail == UnitDetail.CostUp)
        {
            if (gameObject.layer == 8)
                InGameManager.instance.isCostB = false;
            else if (gameObject.layer == 9)
                InGameManager.instance.isCostR = false;
        }
        // Heal
        if (unitDetail == UnitDetail.Heal)
        {
            if (gameObject.layer == 8)
                InGameManager.instance.isHealB = false;
            else if (gameObject.layer == 9)
                InGameManager.instance.isHealR = false;
        }

        col.enabled = false;
        unitState = UnitState.Die;
        gameObject.SetActive(false);
    }

    // ======================================================= 트리거 작동 (주로 버프센서)
    void OnTriggerStay2D(Collider2D collision)
    {
        // Base 타입인 경우
        if (unitType == UnitType.Base)
            return;

        // 공속증가
        // Blue 팀 유닛이 Blue Buff Sensor와 닿았을 경우 || Red 팀 유닛이 Red Buff Sensor와 닿았을 경우
        if ((gameObject.layer == 8 && collision.gameObject.tag == "AtkSpd_UpB") || (gameObject.layer == 9 && collision.gameObject.tag == "AtkSpd_UpR"))
        {
            // 버프 적용(버퍼 제외)
            if (unitDetail != UnitDetail.AtkspdUp && unitDetail != UnitDetail.Devil)
                skillSensor = SkillSensor.ATS;
        }
        // 공격 증가
        if ((gameObject.layer == 8 && collision.gameObject.tag == "Atk_UpB") || (gameObject.layer == 9 && collision.gameObject.tag == "Atk_UpR"))
        {
            if (unitDetail != UnitDetail.AtkUp)
                skillSensor = SkillSensor.ATK;
        }
        // 이속 증가
        if ((gameObject.layer == 8 && collision.gameObject.tag == "Spd_UpB") || (gameObject.layer == 9 && collision.gameObject.tag == "Spd_UpR"))
        {
            if (unitDetail != UnitDetail.SpdUp)
                skillSensor = SkillSensor.SPD;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Base 타입인 경우
        if (unitType == UnitType.Base)
            return;

        // 내가 아군 센서에서 나갔을 때
        if ((gameObject.layer == 8 && collision.gameObject.tag == "AtkSpd_UpB") || (gameObject.layer == 9 && collision.gameObject.tag == "AtkSpd_UpR") ||
            (gameObject.layer == 8 && collision.gameObject.tag == "Atk_UpB") || (gameObject.layer == 9 && collision.gameObject.tag == "Atk_UpR") ||
            (gameObject.layer == 8 && collision.gameObject.tag == "Spd_UpB") || (gameObject.layer == 9 && collision.gameObject.tag == "Spd_UpR"))
        {
            // 버프 해제
            skillSensor = SkillSensor.NONE;
        }
    }
}
