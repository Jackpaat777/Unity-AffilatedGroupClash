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

// ������ġ�� �ٲٸ� ObjectManager �ν����Ϳ��� ������ ��ġ�� �ٲ������
public enum UnitDetail
{
    // Bullet A ��� ����
    Archer, Sniper, Farmer, Guitar, Singer, Bass, Base,
    // Bullet B ��� ����
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
    public bool isHeal;         // �� ���� ����
    public UnitState unitState; // ������ ���¸��� �ٸ� ������ �����ϵ���
    float attackTimer;          // ���ӿ� Ÿ�̸�
    float stopTimer;            // Move�� Ÿ�̸�
    bool isFront;               // Stop���� Move�� ���� ���� ����
    bool isHit;

    [Header("---------------[Skill]")]
    public SkillSensor skillSensor; // skillSensor�� ���� ������ �����ϹǷ� ������ø�� �ȵ� -> ���� bool������ ����?
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

    // ������ Ŭ������ ��
    void OnMouseDown()
    {
        // Base Ÿ���� ���
        if (unitType == UnitType.Base)
            return;

        // ���� ���� �ѱ��
        InGameManager.instance.isUnitClick = true;
        InGameManager.instance.unitObj = gameObject;
    }

    // ���� �� �ʱ�ȭ (Awake�� ����)
    void OnEnable()
    {
        // Base Ÿ���� ���
        if (unitType == UnitType.Base)
            return;

        // �⺻ �����ϱ�
        ResetSettings();

        // ���� ��ȭ�� �ִ� ��� �ʱ�ȭ
        ResetStat();

        // ���� ��� ������ ���� �ʱ�ȭ
        ResetSensor();

        // �ߺ���ȯ ����
        SummonOnce();

        // Barrier�� ���
        isNoDamage = false;
        if (unitDetail == UnitDetail.Hammer)
        {
            isNoDamage = true;
            barrierSkillEffect.SetActive(true);
        }
    }
    // ======================================================= OnEnable ������ �Լ�
    void ResetSettings()
    {
        // ��ġ �ʱ�ȭ
        if (gameObject.layer == 8)
            transform.localPosition = Vector3.left * 11;
        else if (gameObject.layer == 9)
            transform.localPosition = Vector3.right * 11;

        // ü�� ��
        hpSlider.transform.position = Vector3.up * 2000;

        // Ÿ�̸� �ʱ�ȭ
        attackTimer = 0; stopTimer = 0; atkDebuffTimer = 0; atsDebuffTimer = 0; spdDebuffTimer = 0; backStepTimer = 0;
        // �� �ʱ�ȭ
        farmerCount = 0;

        // ���� ����
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
        // ���̵��� ���� �� �ִ�ü�� ���� (�װų� ������ ������ �ٽ� �������) -> ������ �����µ� ���� ������� ���� �ֳ�?
        if (gameObject.layer == 9)
        {
            if (Variables.gameLevel == 0)
                unitMaxHp -= 5;
            else if (Variables.gameLevel == 4)
                unitMaxHp += 5;
        }
        
        // ü�� �ʱ�ȭ
        unitHp = unitMaxHp;

        // ���� ���� �� �ʱ�ȭ
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

        // ��� ���ֿ� ����
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

        // ���� �� ��ȭ ���� �ʱ�ȭ
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
        // ����
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
        // Devil�� ���
        if (unitDetail == UnitDetail.Devil)
        {
            if (gameObject.layer == 8)
                InGameManager.instance.isDevilB = true;
            else if (gameObject.layer == 9)
                InGameManager.instance.isDevilR = true;
        }
        // Cost�� ���
        else if (unitDetail == UnitDetail.CostUp)
        {
            if (gameObject.layer == 8)
                InGameManager.instance.isCostB = true;
            else if (gameObject.layer == 9)
                InGameManager.instance.isCostR = true;
        }
        // Heal�� ���
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
        // ���� ����
        if (!InGameManager.instance.isGameLive)
            return;

        // Base Ÿ���� ���
        if (unitType == UnitType.Base)
            return;

        // ����� Devil�� ����
        DevilHit();
        // BackStep
        BackStepTimer();
        // ���� ���� (�����ð����� ���ݸ��)
        BuffUnit();
        // ����� ���¿� �ɷ��� ��
        DebuffTimer();
        // ���� ���� -> ���� �ȿ� ���� ������ ���� ����
        BuffSensor();
        // ���� �̵�
        UnitMovement();
        HpSlider();
    }
    // ======================================================= Update ������ �Լ�
    void DevilHit()
    {
        if ((InGameManager.instance.isDevilBAttack && gameObject.layer == 9) || (InGameManager.instance.isDevilRAttack && gameObject.layer == 8))
        {
            // ����Ʈ ��ġ���� ����
            Vector3 vec = Vector3.zero;
            if (gameObject.layer == 8)
                vec = new Vector3(-0.5f, 1.5f);
            else if (gameObject.layer == 9)
                vec = new Vector3(0.5f, 1.5f);

            // ����Ʈ
            int idx = (int)UnitDetail.Devil - (int)UnitDetail.Bomb + ((int)UnitDetail.Base + 1) * 2;
            ObjectManager.instance.GetBullet(idx, transform.position + vec);
            // Hit >> ������ (5f)
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
        // Ư������
        if (unitDetail == UnitDetail.CostUp || unitDetail == UnitDetail.Devil || unitDetail == UnitDetail.AtkUp ||
            unitDetail == UnitDetail.AtkspdUp || unitDetail == UnitDetail.SpdUp || unitDetail == UnitDetail.Piano)
        {
            // ���� ��ġ ����
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
                // Stop -> doMove �ִϸ����ͷ� �Ѿ�� ���� ���߱� �ʿ�
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
        // ���� ����� (2�ʵڿ� �ٽ� �ø�)
        if (isAtkDebuff)
        {
            atkDebuffTimer += Time.deltaTime;
            if (atkDebuffTimer > 2f)
            {
                unitAtk += 5;
                isAtkDebuff = false;
            }
        }
        // ���� ����� (2�ʵڿ� �ٽ� �ø�)
        if (isAtsDebuff)
        {
            atsDebuffTimer += Time.deltaTime;
            if (atsDebuffTimer > 2f)
            {
                unitAtkSpeed /= 2;
                isAtsDebuff = false;
            }
        }
        // �̼� ����� (2�ʵڿ� �ٽ� �ø�)
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
        // ���ݷ� ����
        if (skillSensor == SkillSensor.ATK)
        {
            // ���� �ߵ� (�ѹ��� ����) -> �������� isATKUp�� true�� �Ǿ� ���̻� ���ݷ����� ������ �������� ����
            if (!isATKUp)
            {
                // ���ݷ� ����
                unitAtk += 5;
                isATKUp = true;
            }
        }
        // ���� ����
        if (skillSensor == SkillSensor.ATS)
        {
            if (!isATSUp)
            {
                unitAtkSpeed /= 2;
                unitAtkSpeed = unitAtkSpeed < 0.3f ? 0.3f : unitAtkSpeed;
                isATSUp = true;
            }
        }
        // ���ǵ� ����
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
        // ���� ��
        if (skillSensor == SkillSensor.NONE)
        {
            // ���� ���� ��� ���� ���� (�ѹ��� ����)
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
        // ���� �̵� ����
        if (unitState == UnitState.Move)
        {
            Vector3 nextMove = Vector3.right * unitSpeed * Time.deltaTime;
            transform.Translate(nextMove);

            // attackTimer �ʱ�ȭ -> �ڽ�Ʈ�� ���� ��� ������ �������� ���� ���� ����
            if (unitDetail != UnitDetail.CostUp)
                attackTimer = 0;
        }
        else if (unitState == UnitState.Idle || unitState == UnitState.Hit)
        {
            // �տ� �ƹ��� ���� ��
            if (!isFront)
            {
                // Bomb�� Cat�� �ǰݽ� ������ ����
                if (unitDetail == UnitDetail.Bomb || unitDetail == UnitDetail.Cat)
                {
                    DoMove();
                }
                // �����ð� �ڿ� �ٽ� ������
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
        // �����̴� ü�� ǥ��
        hpSlider.value = (float)unitHp / unitMaxHp;

        // �����̴� �̵�
        Vector3 vec = Vector3.zero;
        if (gameObject.layer == 8)
            vec = new Vector3(-0.5f, 1.3f);
        else if (gameObject.layer == 9)
            vec = new Vector3(0.5f, 1.3f);
        hpSlider.transform.position = Camera.main.WorldToScreenPoint(transform.position + vec);
    }

    void FixedUpdate()
    {
        // ���� ����
        if (!InGameManager.instance.isGameLive)
            return;

        // RayCast�� ����ϱ� ������(RigidBody) FixedUpdate���� ȣ�� 
        ScanEnemy();
    }

    // ======================================================= Ÿ�� ���� �Լ�
    void ScanEnemy()
    {
        if (unitState == UnitState.Die)
            return;

        Vector2 dir = Vector2.zero;
        string enemyLayer = "";

        if (gameObject.layer == 8)      // Blue �� ����
        {
            dir = Vector2.right;
            enemyLayer = "Red";
        }
        else if (gameObject.layer == 9) // Red �� ����
        {
            dir = Vector2.left;
            enemyLayer = "Blue";
        }

        // RayCast
        RaycastHit2D rayHit = Physics2D.Raycast(transform.position, dir, unitRange, LayerMask.GetMask(enemyLayer));

        // ������ ������
        if (rayHit.collider != null)
        {
            // �� ������Ʈ ��������
            Unit enemyLogic = rayHit.collider.gameObject.GetComponent<Unit>();

            // ���� ������ ��� (Wizard�� Transform ���� �ʿ�) (Stick�� Warrior���� �ٸ� �Լ� ����)
            if (unitDetail == UnitDetail.Stick || unitDetail == UnitDetail.Bomb || unitDetail == UnitDetail.Wizard)
                WideAttack(enemyLogic.transform);

            // ���� ������ ��� (Unit ���� �ʿ�)
            else if (unitType == UnitType.Warrior || unitType == UnitType.Tanker)
                DirectAttack(enemyLogic);

            // ���Ÿ� ������ ��� + Base�� ���
            else if (unitType == UnitType.Ranger || unitType == UnitType.Base)
                ShotAttack();

            // Cat�� ���
            else if (unitDetail == UnitDetail.Cat)
                CatAttack(enemyLogic);

            // ������ ���
            else if (unitType == UnitType.Buffer || unitDetail == UnitDetail.Devil)
            {
                // ���� (����X)
                DoStop();
                isFront = true;
            }
        }
        else
        {
            isFront = false;
        }

        // Bomb ����
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
        // Archer ����
        else if (unitDetail == UnitDetail.Archer)
        {
            RaycastHit2D bombRayHit = Physics2D.Raycast(transform.position, dir, 1f, LayerMask.GetMask(enemyLayer));
            if (bombRayHit.collider != null)
            {
                // �齺���� ������ ���¿����� (2�� ��Ÿ��)
                if (isBackStep)
                {
                    // �齺�� ���� (���ѰŸ� ����)
                    if (gameObject.layer == 8 && transform.position.x > -10f)
                        transform.Translate(Vector3.left * 1f);
                    else if (gameObject.layer == 9 && transform.position.x < 10f)
                        transform.Translate(Vector3.right * 1f);
                    // 2�ʰ� �齺�� �Ұ���
                    isBackStep = false;
                }
            }
        }
    }

    // ======================================================= ���� �Լ�
    void CatAttack(Unit enemyLogic)
    {
        // Cat�� ������� ����
        if (unitDetail == UnitDetail.Cat)
        {
            // Base�տ��� ����
            if (enemyLogic.unitDetail == UnitDetail.Base)
            {
                // ����
                isFront = true;
                DoStop();
            }

            // ���ݼӵ��� ���� ����
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
        // Cat�� ������� ����
        if (unitDetail == UnitDetail.Cat)
        {
            // ��������� ��� return
            if (enemyLogic.unitType == UnitType.Warrior || enemyLogic.unitType == UnitType.Tanker ||
                enemyLogic.unitDetail == UnitDetail.Stick || enemyLogic.unitDetail == UnitDetail.Bomb)
                return;
        }

        // ����
        isFront = true;
        DoStop();

        // ���ݼӵ��� ���� ����
        attackTimer += Time.deltaTime;
        if (attackTimer < unitAtkSpeed)
            return;

        // Skill
        int idx = (int)unitDetail - (int)UnitDetail.Bomb;
        // Vampire�� ���� �� ü�� ȸ��
        if (unitDetail == UnitDetail.Vampire)
        {
            // ȸ�� ����Ʈ
            ObjectManager.instance.GetBullet(idx + ((int)UnitDetail.Base + 1) * 2, transform.position + Vector3.up * 0.5f);
            unitHp += unitAtk;
            unitHp = unitHp > unitMaxHp ? unitMaxHp : unitHp;
        }
        // Punch�� ��� ü���� 50���ϸ� ����Ŵ
        else if (unitDetail == UnitDetail.Punch)
        {
            // Base�� �ش�ȵ�
            if (enemyLogic.unitDetail != UnitDetail.Base)
            {
                // ���
                if (enemyLogic.unitHp <= 30)
                {
                    enemyLogic.DoHit(30);
                    // ����Ʈ
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
        // Grow�� ���� ���ϼ��� ���� ��ȭ
        else if (unitDetail == UnitDetail.GrowUp)
        {
            Animator anim = growUpHand.GetComponent<Animator>();
            if (gameObject.layer == 8)
                anim.SetTrigger("doAttackB");
            else if (gameObject.layer == 9)
                anim.SetTrigger("doAttackR");

            // Base�� �ش�ȵ�
            if (enemyLogic.unitDetail != UnitDetail.Base)
            {
                // ���� HP�� �� ���ݷ����ϸ�ŭ ������ (���� ��� -> ������ �� �����̸�? ���Ǳ� ���� �׾��� ��)
                if (enemyLogic.unitHp <= unitAtk)
                {
                    unitAtk += 1;
                    unitAtkSpeed -= 0.05f;
                    unitAtkSpeed = unitAtkSpeed < 0.3f ? 0.3f : unitAtkSpeed;
                    unitMaxHp += 5;
                }
            }
        }
        // Air�� ���� �� ��� ���� ����
        else if (unitDetail == UnitDetail.Air && !enemyLogic.isAtsDebuff)
        {
            if (enemyLogic.unitDetail != UnitDetail.Base)
            {
                enemyLogic.unitAtkSpeed *= 2;
                enemyLogic.isAtsDebuff = true;
                enemyLogic.atsDebuffTimer = 0;
            }
        }
        // Hammer�� ���� �� ������ ���� True
        else if (unitDetail == UnitDetail.Hammer)
        {
            // ����Ʈ
            if (gameObject.layer == 8)
                ObjectManager.instance.GetBullet(idx + ((int)UnitDetail.Base + 1) * 2, transform.position + Vector3.right * 0.5f);
            else if (gameObject.layer == 9)
                ObjectManager.instance.GetBullet(idx + ((int)UnitDetail.Base + 1) * 2, transform.position + Vector3.left * 0.5f);
            barrierSkillEffect.SetActive(false);
            isNoDamage = false;
        }
        // Drum�� ���� ��ħ
        else if (unitDetail == UnitDetail.Drum)
        {
            // �� ��ġ�� (�ִ� ����)
            if (gameObject.layer == 8 && enemyLogic.transform.position.x < 10f)
                enemyLogic.transform.Translate(Vector3.right * 0.5f);
            else if (gameObject.layer == 9 && enemyLogic.transform.position.x > -10f)
                enemyLogic.transform.Translate(Vector3.left * 0.5f);
        }

        // ���������� Counter�� ������ �ڽŵ� ���� ������
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
        // ����
        isFront = true;
        DoStop();

        // ���ݼӵ��� ���� ����
        attackTimer += Time.deltaTime;
        if (attackTimer < unitAtkSpeed)
            return;


        // Bullet
        GameObject bullet = null;
        int idx = (int)unitDetail;
        Vector3 vec = Vector3.zero;

        if (gameObject.layer == 8)          // ��� �� ������ ���
        {
            // Base�� ���
            if (unitDetail == UnitDetail.Base)
                vec = new Vector3(0.5f, 0);

            // �ڽ��� �������� �Ѿ� �߻�
            bullet = ObjectManager.instance.GetBullet(idx, transform.position + Vector3.up * 0.2f + vec);
        }
        else if (gameObject.layer == 9)     // ���� �� ������ ���
        {
            if (unitDetail == UnitDetail.Base)
                vec = new Vector3(-0.5f, 0);

            bullet = ObjectManager.instance.GetBullet(idx + ((int)UnitDetail.Base + 1), transform.position + Vector3.up * 0.2f + vec); // => Bullet A ��� ���� ���� �߰�
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

        // Bullet�� �� �Ѱ��ֱ�
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
        // ����Ʈ ��������
        GameObject bullet = null;

        int idx = ((int)unitDetail - (int)UnitDetail.Bomb) + ((int)UnitDetail.Base + 1) * 2;
        if (unitDetail == UnitDetail.Bomb)
        {
            // ����
            DoHit(unitMaxHp);
            // ����Ʈ
            bullet = ObjectManager.instance.GetBullet(idx, transform.position); // 12 => Bullet A ��� ���� * 2

            // Sound
            SoundManager.instance.SfxPlay("Bomb");
        }
        else // ���ӿ� ������ �޴� ���ֵ�
        {
            // ����
            isFront = true;
            DoStop();

            // ���ݼӵ��� ���� ����
            attackTimer += Time.deltaTime;
            if (attackTimer < unitAtkSpeed)
                return;

            // �ڽ��� �������� �Ѿ� �߻�
            if (unitDetail == UnitDetail.Stick)
            {
                if (gameObject.layer == 8)
                    bullet = ObjectManager.instance.GetBullet(idx, transform.position + Vector3.right * 0.5f);
                else if (gameObject.layer == 9)
                    bullet = ObjectManager.instance.GetBullet(idx, transform.position + Vector3.left * 0.5f);

                // Sound
                SoundManager.instance.SfxPlay("Crash");
            }
            // Ÿ�ٸ� �������� �Ѿ� �߻�
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

        // Bullet�� �� �Ѱ��ֱ�
        BulletSetting(bullet, unitDetail, gameObject.layer, unitAtk);
    }
    void BulletSetting(GameObject bulletObj, UnitDetail uDetail, int uLayer, int uDmg)
    {
        Bullet bulletLogic = bulletObj.GetComponent<Bullet>();

        bulletLogic.unitDetail = uDetail;
        bulletLogic.layer = uLayer;
        bulletLogic.dmg = uDmg;
    }

    // ======================================================= ���� �Լ�
    public void DoBuff(Unit allyLogic)
    {
        // Bullet
        int idx = ((int)unitDetail - (int)UnitDetail.Bomb) + ((int)UnitDetail.Base + 1) * 2;

        if (gameObject.layer == 8)          // ��� �� ������ ���
        {
            // Ÿ���� �������� �߻�
            if (unitDetail == UnitDetail.Heal)
            {
                allyLogic.unitHp += 15;
                allyLogic.unitHp = allyLogic.unitHp > allyLogic.unitMaxHp ? allyLogic.unitMaxHp : allyLogic.unitHp;
                Vector3 vec = new Vector3(-0.5f, 0.5f);
                ObjectManager.instance.GetBullet(idx, allyLogic.transform.position + vec);
            }
        }
        else if (gameObject.layer == 9)     // ���� �� ������ ���
        {
            if (unitDetail == UnitDetail.Heal)
            {
                allyLogic.unitHp += 15;
                allyLogic.unitHp = allyLogic.unitHp > allyLogic.unitMaxHp ? allyLogic.unitMaxHp : allyLogic.unitHp;
                Vector3 vec = new Vector3(0.5f, 0.5f);
                ObjectManager.instance.GetBullet(idx, allyLogic.transform.position + vec);
            }
        }

        // ���� �ߵ�
        anim.SetTrigger("doAttack");

        if (isHit)
        {
            anim.SetTrigger("doHit");
            isHit = false;
        }

        // Sound
        SoundManager.instance.SfxPlay("Heal");
    }

    // ======================================================= ���� �Լ�
    void DoMove()
    {
        // �̹� �����̰� ������ ȣ�� X
        if (unitState == UnitState.Move)
            return;

        // Guitar�� �����̸� ���� �ʱ�ȭ
        if (unitDetail == UnitDetail.Guitar)
            unitAtkSpeed = 1.5f;

        unitState = UnitState.Move;
        anim.SetTrigger("doMove");
        if (dustObject != null)
            dustObject.Play();
    }
    void DoStop()
    {
        // �ȿ����̰� �ִٸ� ȣ�� X
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
        // Base Ÿ���� ���
        if (unitType == UnitType.Base)
        {
            InGameManager.instance.BaseHit(damage, gameObject.layer);
            return;
        }

        // Barrier�� ���������� ���ٸ� ��������
        if (isNoDamage)
            return;

        if (unitDetail == UnitDetail.Berserker)
        {
            // ����ü�� 10 �� 0.05�� ���� ����
            unitAtkSpeed -= damage * 0.005f;
            unitAtkSpeed = unitAtkSpeed < 0.3f ? 0.3f : unitAtkSpeed;
        }
        else if (unitDetail == UnitDetail.Shield)
        {
            // 3������ ����
            damage -= 3;
            damage = damage < 0 ? 0 : damage;
        }

        // Hit ����
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
        // ü�� �������
        if (gameObject.layer == 9)
        {
            if (Variables.gameLevel == 0)
                unitMaxHp += 5;
            else if (Variables.gameLevel == 4)
                unitMaxHp -= 5;
        }

        // Sensor ����
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

    // ======================================================= Ʈ���� �۵� (�ַ� ��������)
    void OnTriggerStay2D(Collider2D collision)
    {
        // Base Ÿ���� ���
        if (unitType == UnitType.Base)
            return;

        // ��������
        // Blue �� ������ Blue Buff Sensor�� ����� ��� || Red �� ������ Red Buff Sensor�� ����� ���
        if ((gameObject.layer == 8 && collision.gameObject.tag == "AtkSpd_UpB") || (gameObject.layer == 9 && collision.gameObject.tag == "AtkSpd_UpR"))
        {
            // ���� ����(���� ����)
            if (unitDetail != UnitDetail.AtkspdUp && unitDetail != UnitDetail.Devil)
                skillSensor = SkillSensor.ATS;
        }
        // ���� ����
        if ((gameObject.layer == 8 && collision.gameObject.tag == "Atk_UpB") || (gameObject.layer == 9 && collision.gameObject.tag == "Atk_UpR"))
        {
            if (unitDetail != UnitDetail.AtkUp)
                skillSensor = SkillSensor.ATK;
        }
        // �̼� ����
        if ((gameObject.layer == 8 && collision.gameObject.tag == "Spd_UpB") || (gameObject.layer == 9 && collision.gameObject.tag == "Spd_UpR"))
        {
            if (unitDetail != UnitDetail.SpdUp)
                skillSensor = SkillSensor.SPD;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Base Ÿ���� ���
        if (unitType == UnitType.Base)
            return;

        // ���� �Ʊ� �������� ������ ��
        if ((gameObject.layer == 8 && collision.gameObject.tag == "AtkSpd_UpB") || (gameObject.layer == 9 && collision.gameObject.tag == "AtkSpd_UpR") ||
            (gameObject.layer == 8 && collision.gameObject.tag == "Atk_UpB") || (gameObject.layer == 9 && collision.gameObject.tag == "Atk_UpR") ||
            (gameObject.layer == 8 && collision.gameObject.tag == "Spd_UpB") || (gameObject.layer == 9 && collision.gameObject.tag == "Spd_UpR"))
        {
            // ���� ����
            skillSensor = SkillSensor.NONE;
        }
    }
}
