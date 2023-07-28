using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public enum UnitType
{
    Warrior,
    Tanker,
    Ranger,
    Buffer,
    Special
}

// ������ġ�� �ٲٸ� ObjectManager �ν����Ϳ��� ������ ��ġ�� �ٲ������
public enum UnitDetail
{
    // Bullet A ��� ����
    Archer, Sniper, Farmer, Guitar, Singer, Bass,
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
    public ParticleSystem dustObject;
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
    public float atkDebuffTimer;
    public float atsDebuffTimer;
    public float spdDebuffTimer;
    public float backStepTimer;
    public float devilTimer;
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
        // ���� ���� �ѱ��
        GameManager.instance.isUnitClick = true;
        GameManager.instance.unitObj = gameObject;
    }

    // ���� �� �ʱ�ȭ (Awake�� ����)
    void OnEnable()
    {
        // �⺻ �����ϱ�
        ResetSettings();

        // ���� ��ȭ�� �ִ� ��� �ʱ�ȭ
        ResetStat();

        // ���� ��� ������ ���� �ʱ�ȭ
        ResetSensor();

        // Devil�� ���
        if (unitDetail == UnitDetail.Devil)
            GameManager.instance.isDevil = true;
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

        // Ÿ�̸� �ʱ�ȭ
        attackTimer = 0; stopTimer = 0; devilTimer = 0; atkDebuffTimer = 0; atsDebuffTimer = 0; spdDebuffTimer = 0; backStepTimer = 0;
        // �� �ʱ�ȭ
        farmerCount = 0;

        // ���� ����
        unitState = UnitState.Idle;
        isFront = false;
        isHit = false;
        DoMove();

        // ü�� �ʱ�ȭ
        unitHp = unitMaxHp;

        // Collider
        col.enabled = true;
    }
    void ResetStat()
    {
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
                unitSpeed += 0.4f;
            else if (gameObject.layer == 9)
                unitSpeed -= 0.4f;
            isSpdDebuff = false;
        }

        // ��� ���ֿ� ����
        if (skillSensor == SkillSensor.ATK)
        {
            unitAtk -= 10;
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
            unitAtk = 30;
            unitAtkSpeed = 1.5f;
            unitMaxHp = 100;
            unitHp = 100;
        }
    }
    void ResetSensor()
    {
        // ����
        int idx = ((int)unitDetail - (int)UnitDetail.Bomb) + ((int)UnitDetail.Bass + 1) * 2;
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

    void Update()
    {
        // ���� ���� (�����ð����� ���ݸ��)
        BuffUnit();
        // ����� ���¿� �ɷ��� ��
        DebuffTimer();
        // ���� ���� -> ���� �ȿ� ���� ������ ���� ����
        BuffSensor();
        // ���� �̵�
        UnitMovement();
    }
    // ======================================================= Update ������ �Լ�
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
            // ���� �ѱ�
            if (unitDetail == UnitDetail.Devil)
            {
                if (gameObject.layer == 8)
                    GameManager.instance.allSensor.tag = "DevilB";
                else if (gameObject.layer == 9)
                    GameManager.instance.allSensor.tag = "DevilR";
                GameManager.instance.allSensor.SetActive(true);
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
                    if (gameObject.layer == 8 && GameManager.instance.blueCost < 10)
                        GameManager.instance.blueCost += 1;
                    else if (gameObject.layer == 9 && GameManager.instance.redCost < 10)
                        GameManager.instance.redCost += 1;
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
                    unitSpeed += 0.4f;
                else if (gameObject.layer == 9)
                    unitSpeed -= 0.4f;
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
                unitAtk += 10;
                isATKUp = true;
            }
        }
        // ���� ����
        if (skillSensor == SkillSensor.ATS)
        {
            if (!isATSUp)
            {
                unitAtkSpeed /= 2;
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
                unitAtk -= 10;
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
        }
        else if (unitState == UnitState.Idle || unitState == UnitState.Hit)
        {
            // �տ� �ƹ��� ���� ��
            if (!isFront)
            {
                // Cat�� �ǰݽ� ������ ����
                if (unitDetail == UnitDetail.Cat)
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

    void FixedUpdate()
    {
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
            else if (unitType == UnitType.Warrior || unitType == UnitType.Tanker || unitDetail == UnitDetail.Cat)
                DirectAttack(enemyLogic);

            // ���Ÿ� ������ ���
            else if (unitType == UnitType.Ranger)
                ShotAttack();

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
                unitSpeed *= 1.1f;
            }
        }
        // Archer ����
        else if (unitDetail == UnitDetail.Archer)
        {
            RaycastHit2D bombRayHit = Physics2D.Raycast(transform.position, dir, 1f, LayerMask.GetMask(enemyLayer));
            if (bombRayHit.collider != null)
            {
                // �齺�� ���� (���ѰŸ� ����)
                if (gameObject.layer == 8 && transform.position.x > -10f)
                    transform.Translate(Vector3.left * 1f);
                else if (gameObject.layer == 9 && transform.position.x < 10f)
                    transform.Translate(Vector3.right * 1f);
            }
        }
    }

    // ======================================================= ���� �Լ�
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
            ObjectManager.instance.GetBullet(idx + ((int)UnitDetail.Bass + 1) * 2, transform.position + Vector3.up * 0.5f);
            unitHp += unitAtk;
            unitHp = unitHp > unitMaxHp ? unitMaxHp : unitHp;
        }
        // Punch�� ��� ü���� 5���ϸ� ����Ŵ
        else if (unitDetail == UnitDetail.Punch)
        {
            // ���
            if (enemyLogic.unitHp <= 50)
            {
                enemyLogic.DoHit(50);
                // ����Ʈ
                if (gameObject.layer == 8)
                {
                    Vector3 vec = new Vector3(0.5f, 0.5f);
                    ObjectManager.instance.GetBullet(idx + ((int)UnitDetail.Bass + 1) * 2, transform.position + vec);
                }
                else if (gameObject.layer == 9)
                {
                    Vector3 vec = new Vector3(-0.5f, 0.5f);
                    ObjectManager.instance.GetBullet(idx + ((int)UnitDetail.Bass + 1) * 2, transform.position + vec);
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
            // ���� HP�� �� ���ݷ����ϸ�ŭ ������ (���� ��� -> ������ �� �����̸�? ���Ǳ� ���� �׾��� ��)
            if (enemyLogic.unitHp <= unitAtk)
            {
                unitAtk += 5;
                unitAtkSpeed -= 0.1f;
                unitAtkSpeed = unitAtkSpeed < 0.3f ? 0.3f : unitAtkSpeed;
                unitMaxHp += 10;
            }
        }
        // Air�� ���� �� ��� ���� ����
        else if (unitDetail == UnitDetail.Air && !enemyLogic.isAtsDebuff)
        {
            enemyLogic.unitAtkSpeed *= 2;
            enemyLogic.isAtsDebuff = true;
            enemyLogic.atsDebuffTimer = 0;
        }
        // Hammer�� ���� �� ������ ���� True
        else if (unitDetail == UnitDetail.Hammer)
        {
            // ����Ʈ
            if (gameObject.layer == 8)
                ObjectManager.instance.GetBullet(idx + ((int)UnitDetail.Bass + 1) * 2, transform.position + Vector3.right * 0.5f);
            else if (gameObject.layer == 9)
                ObjectManager.instance.GetBullet(idx + ((int)UnitDetail.Bass + 1) * 2, transform.position + Vector3.left * 0.5f);
            barrierSkillEffect.SetActive(false);
            isNoDamage = false;
        }
        // Drum�� ���� ��ħ
        else if (unitDetail == UnitDetail.Drum)
        {
            // �� ��ġ��
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
        if (gameObject.layer == 8)          // ��� �� ������ ���
        {
            // �ڽ��� �������� �Ѿ� �߻�
            bullet = ObjectManager.instance.GetBullet(idx, transform.position + Vector3.up * 0.2f);
        }
        else if (gameObject.layer == 9)     // ���� �� ������ ���
        {
            bullet = ObjectManager.instance.GetBullet(idx + ((int)UnitDetail.Bass + 1), transform.position + Vector3.up * 0.2f); // => Bullet A ��� ���� ���� �߰�
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
        anim.SetTrigger("doAttack");
        attackTimer = 0;
    }
    void WideAttack(Transform targetTrans)
    {
        // ����Ʈ ��������
        GameObject bullet = null;

        int idx = ((int)unitDetail - (int)UnitDetail.Bomb) + ((int)UnitDetail.Bass + 1) * 2;
        if (unitDetail == UnitDetail.Bomb)
        {
            // ����
            DoHit(unitMaxHp);
            // ����Ʈ
            bullet = ObjectManager.instance.GetBullet(idx, transform.position); // 12 => Bullet A ��� ���� * 2
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
            }
            // Ÿ�ٸ� �������� �Ѿ� �߻�
            else if (unitDetail == UnitDetail.Wizard)
            {
                if (gameObject.layer == 8)
                    bullet = ObjectManager.instance.GetBullet(idx, targetTrans.position + Vector3.right * 0.5f);
                else if (gameObject.layer == 9)
                    bullet = ObjectManager.instance.GetBullet(idx, targetTrans.position + Vector3.left * 0.5f);
            }

            // Attack
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
        // ���ݼӵ��� ���� ����
        attackTimer += Time.deltaTime;
        if (attackTimer < unitAtkSpeed)
            return;

        // Bullet
        int idx = ((int)unitDetail - (int)UnitDetail.Bomb) + ((int)UnitDetail.Bass + 1) * 2;

        if (gameObject.layer == 8)          // ��� �� ������ ���
        {
            // Ÿ���� �������� �߻�
            if (unitDetail == UnitDetail.Heal)
            {
                allyLogic.unitHp += 10;
                allyLogic.unitHp = allyLogic.unitHp > allyLogic.unitMaxHp ? allyLogic.unitMaxHp : allyLogic.unitHp;
                Vector3 vec = new Vector3(-0.5f, 0.5f);
                ObjectManager.instance.GetBullet(idx, allyLogic.transform.position + vec);
            }
        }
        else if (gameObject.layer == 9)     // ���� �� ������ ���
        {
            if (unitDetail == UnitDetail.Heal)
            {
                allyLogic.unitHp += 10;
                allyLogic.unitHp = allyLogic.unitHp > allyLogic.unitMaxHp ? allyLogic.unitMaxHp : allyLogic.unitHp;
                Vector3 vec = new Vector3(0.5f, 0.5f);
                ObjectManager.instance.GetBullet(idx, allyLogic.transform.position + vec);
            }
        }

        // ���� �ߵ�
        anim.SetTrigger("doAttack");
        attackTimer = 0;

        if (isHit)
        {
            anim.SetTrigger("doHit");
            isHit = false;
        }
    }

    // ======================================================= ���� �Լ�
    void DoMove()
    {
        // �̹� �����̰� ������ ȣ�� X
        if (unitState == UnitState.Move)
            return;

        // Skill
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
        // Barrier�� ���������� ���ٸ� ��������
        if (isNoDamage)
            return;

        if (unitDetail == UnitDetail.Berserker)
        {
            // ����ü�� 10 �� 0.1�� ���� ����
            unitAtkSpeed -= damage * 0.01f;
            unitAtkSpeed = unitAtkSpeed < 0.3f ? 0.3f : unitAtkSpeed;
        }
        else if (unitDetail == UnitDetail.Shield)
        {
            // 3������ ����
            damage -= 3;
            damage = damage < 0 ? 0 : damage;
        }

        unitState = UnitState.Hit;
        unitHp -= damage;
        isHit = true;
        anim.SetTrigger("doHit");

        stopTimer = 0;
        
        if (dustObject != null)
            dustObject.Stop();

        if (unitHp <= 0)
        {
            DoDie();
            return;
        }
    }
    void DoDie()
    {
        // Sensor ����
        if (unitDetail == UnitDetail.Devil)
            GameManager.instance.allSensor.SetActive(false);
        else if (unitDetail == UnitDetail.AtkUp)
            atkSensor.SetActive(false);
        else if (unitDetail == UnitDetail.AtkspdUp)
            atsSensor.SetActive(false);
        else if (unitDetail == UnitDetail.SpdUp)
            spdSensor.SetActive(false);
        else if (unitDetail == UnitDetail.Piano)
            bulSensor.SetActive(false);

        // Devil
        if (unitDetail == UnitDetail.Devil)
            GameManager.instance.isDevil = false;

        col.enabled = false;
        unitState = UnitState.Die;
        gameObject.SetActive(false);
    }

    // ======================================================= Ʈ���� �۵� (�ַ� ��������)
    void OnTriggerStay2D(Collider2D collision)
    {
        // Devil����
        // Blue �� ������ Red Sensor�� ����� ��� || Red �� ������ Blue Sensor�� ����� ���
        if ((gameObject.layer == 8 && collision.gameObject.tag == "DevilR") || (gameObject.layer == 9 && collision.gameObject.tag == "DevilB"))
        {
            // ����Ʈ ��ġ���� ����
            Vector3 vec = Vector3.zero;
            if (gameObject.layer == 8)
                vec = new Vector3(-0.5f, 1.5f);
            else if (gameObject.layer == 9)
                vec = new Vector3(0.5f, 1.5f);

            // Hit
            devilTimer += Time.deltaTime;
            if (devilTimer > 2f)  // ������ �޴� �ֱ� (2f)
            {
                // ����Ʈ
                int idx = (int)UnitDetail.Devil - (int)UnitDetail.Bomb + ((int)UnitDetail.Bass + 1) * 2;
                ObjectManager.instance.GetBullet(idx, transform.position + vec);
                // Hit >> ������ (10f)
                DoHit(10);
                devilTimer = 0;
            }
        }
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
