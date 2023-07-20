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
    Archer, Sniper, Farmer, Guitar, Singer, AtkspdUp, Wizard,
    // Bullet B ��� ����
    Bomb, Drum, AtkUp, Vampire, Punch, Devil, Heal,
    Sword, Guard, Berserker,
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

public class Unit : MonoBehaviour
{
    [Header("---------------[Stat]")]
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

    public bool isATSSensor;
    public bool isATSUp;
    public bool isAtkDebuff;
    public int atkBuffCount;
    public float atkDebuffTimer;
    float attackTimer;
    float stopTimer;
    float devilTimer;
    bool isFront;
    bool isHit;
    GameObject atsSensor;

    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();

        // ���� ����
        unitState = UnitState.Idle;
        isFront = false;
        DoMove();
    }

    // ���� �� �ʱ�ȭ
    void OnEnable()
    {
        // ��ġ �ʱ�ȭ
        if (gameObject.layer == 8)
            transform.localPosition = Vector3.left * 11;
        else if (gameObject.layer == 9)
            transform.localPosition = Vector3.right * 11;

        // Ÿ�̸� �ʱ�ȭ
        attackTimer = 0; stopTimer = 0; devilTimer = 0;

        // ���� ����
        unitState = UnitState.Idle;
        isFront = false;
        isHit = false;
        DoMove();

        // �� �ʱ�ȭ
        unitHp = unitMaxHp;
        atsSensor = null;

        // ��ų�� ���� ��
        isAtkDebuff = false;
        atkBuffCount = 0;
        if (isATSUp) // ��� ���ֿ� ����
        {
            unitAtkSpeed *= 2;
            isATSUp = false;
        }

        // ���� ���� ���� �ʱ�ȭ
        if (unitDetail == UnitDetail.Guitar)
            unitAtkSpeed = 1.5f;

        if (unitDetail == UnitDetail.Berserker)
            unitAtkSpeed = 1.5f;

        if (unitDetail == UnitDetail.AtkspdUp)
        {
            int idx = (int)unitDetail;
            if (gameObject.layer == 8)
            {
                atsSensor = ObjectManager.instance.GetBullet(idx, transform.position + Vector3.left * 0.5f);
                
            }
            else if (gameObject.layer == 9)
            {
                atsSensor = ObjectManager.instance.GetBullet(idx + ((int)UnitDetail.Wizard + 1), transform.position + Vector3.right * 0.5f);
            }
        }
    }

    // �ʱ�ȭ �Լ� -> guard���� �ߺ��Լ���? -> �� �и��ұ�

    void Update()
    {
        // Ư������ - �ڽ�Ʈ���� || ��������
        if (unitDetail == UnitDetail.CostUp || unitDetail == UnitDetail.AtkspdUp)
        {
            if (gameObject.layer == 8)
            {
                atsSensor.transform.position = transform.position + Vector3.left * 0.5f;
            }
            else if (gameObject.layer == 9)
            {
                atsSensor.transform.position = transform.position + Vector3.right * 0.5f;
            }

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
        // ����
        if (unitDetail == UnitDetail.Devil)
        {
            // Sensor �ѱ�
            if (gameObject.layer == 8)
                GameManager.instance.allSensor.layer = 10;
            else if (gameObject.layer == 9)
                GameManager.instance.allSensor.layer = 11;
            GameManager.instance.allSensor.SetActive(true);

            // ���� ���
            attackTimer += Time.deltaTime;
            if (attackTimer > unitAtkSpeed)
            {
                DoStop();
                anim.SetTrigger("doAttack");
                attackTimer = 0;
            }
        }
        // ���� ����� (2�ʵڿ� �ٽ� �ø�)
        if (isAtkDebuff)
        {
            atkDebuffTimer += Time.deltaTime;
            if (atkDebuffTimer > 2f)
            {
                unitAtk += 1;
                isAtkDebuff = false;
                atkDebuffTimer = 0;
            }
        }
        // ���� ����
        if (isATSSensor)
        {
            // ���� �ߵ� (�ѹ��� ����) -> �������� isATSUp�� true�� �Ǿ� ���̻� �������� ������ �������� ����
            // isATSSensor�� �ߵ����̸� ���ӹ��� ������ ������ ����
            if (!isATSUp)
            {
                // ���� ����
                unitAtkSpeed /= 2;
                isATSUp = true;
            }
        }
        else
        {
            // ���� ���� (�ѹ��� ����)
            if (isATSUp)
            {
                unitAtkSpeed *= 2;
                isATSUp = false;
            }
        }

        // ���� �̵�
        UnitMovement();
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
        ScanEnemy();
        //ScanAlly();
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


            // �巳�� ���(�������� �ٸ� �Լ� ���) / ��ź�� ���
            if (unitDetail == UnitDetail.Drum || unitDetail == UnitDetail.Bomb)
                BombAttack();

            // ���� ������ ��� (Unit ���� �ʿ�)
            else if (unitType == UnitType.Warrior || unitType == UnitType.Tanker)
                DirectAttack(enemyLogic);

            // ���Ÿ� ������ ��� (Transform ���� �ʿ�)
            else if (unitType == UnitType.Ranger)
                ShotAttack(enemyLogic.transform);

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

        // ��ź ����
        if (unitDetail == UnitDetail.Bomb)
        {
            RaycastHit2D bombRayHit = Physics2D.Raycast(transform.position, dir, 4f, LayerMask.GetMask(enemyLayer));
            if (bombRayHit.collider != null)
            {
                anim.SetTrigger("doAttack");
                unitSpeed *= 1.1f;
            }
        }
    }
    void ScanAlly()
    {
        // ���۰� �ƴϸ� �Ʊ�üũ X
        if (unitState == UnitState.Die || unitType != UnitType.Buffer)
            return;

        Vector2 dir = Vector2.zero;
        string allyLayer = "";

        if (gameObject.layer == 8)      // Blue �� ����
        {
            dir = Vector2.right;
            allyLayer = "Blue";
        }
        else if (gameObject.layer == 9) // Red �� ����
        {
            dir = Vector2.left;
            allyLayer = "Red";
        }

        // RayCast
        RaycastHit2D rayHit = Physics2D.Raycast(transform.position, dir, unitRange, LayerMask.GetMask(allyLayer));

        // �Ʊ��� ������
        if (rayHit.collider != null)
        {
            // �Ʊ� ������Ʈ ��������
            Unit allyLogic = rayHit.collider.gameObject.GetComponent<Unit>();
            
            if (unitDetail == UnitDetail.Heal)
            {
                // State�� Move�� �ƴҶ� ���� -> Move�� �ɶ����� ������ϴٰ� �ѹ��� ��
                // Ǯ�Ǹ� ���� X
                if (allyLogic.unitHp < allyLogic.unitMaxHp)
                    DoBuff(allyLogic);
            }
        }
        else
        {
            isFront = false;
        }
    }

    // ======================================================= ���� �Լ�
    void DirectAttack(Unit enemyLogic)
    {
        // ����
        isFront = true;
        DoStop();

        // ���ݼӵ��� ���� ����
        attackTimer += Time.deltaTime;
        if (attackTimer < unitAtkSpeed)
            return;


        // Skill
        int idx = (int)unitDetail - (int)UnitDetail.Bomb;
        // �����̾�� ���� �� ü�� ȸ��
        if (unitDetail == UnitDetail.Vampire)
        {
            // ȸ�� ����Ʈ
            ObjectManager.instance.GetBullet(idx + ((int)UnitDetail.Wizard + 1) * 2, transform.position + Vector3.up * 0.5f);
            ExeBuffType("HP", 1);
        }
        // ��ġ�� ��� ü���� 3���ϸ� ����Ŵ
        else if (unitDetail == UnitDetail.Punch)
        {
            // ���
            if (enemyLogic.unitHp <= 5)
            {
                enemyLogic.DoHit(5);
                // ����Ʈ
                if (gameObject.layer == 8)
                {
                    Vector3 vec = new Vector3(0.5f, 0.5f);
                    ObjectManager.instance.GetBullet(idx + ((int)UnitDetail.Wizard + 1) * 2, transform.position + vec);
                }
                else if (gameObject.layer == 9)
                {
                    Vector3 vec = new Vector3(-0.5f, 0.5f);
                    ObjectManager.instance.GetBullet(idx + ((int)UnitDetail.Wizard + 1) * 2, transform.position + vec);
                }
            }
        }

        // Attack
        enemyLogic.DoHit(unitAtk);
        anim.SetTrigger("doAttack");
        attackTimer = 0;
    }
    void ShotAttack(Transform targetTrans)
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
            if (unitDetail == UnitDetail.Archer || unitDetail == UnitDetail.Sniper || unitDetail == UnitDetail.Farmer || unitDetail == UnitDetail.Guitar || unitDetail == UnitDetail.Singer)
            {
                bullet = ObjectManager.instance.GetBullet(idx, transform.position + Vector3.up * 0.3f);
            }
            // Ÿ�ٸ� �������� �Ѿ� �߻�
            else if (unitDetail == UnitDetail.Wizard)
            {
                bullet = ObjectManager.instance.GetBullet(idx, targetTrans.position);
            }
        }
        else if (gameObject.layer == 9)     // ���� �� ������ ���
        {
            if (unitDetail == UnitDetail.Archer || unitDetail == UnitDetail.Sniper || unitDetail == UnitDetail.Farmer || unitDetail == UnitDetail.Guitar || unitDetail == UnitDetail.Singer)
            {
                bullet = ObjectManager.instance.GetBullet(idx + ((int)UnitDetail.Wizard + 1), transform.position + Vector3.up * 0.3f); // => Bullet A ��� ���� ���� �߰�
            }
            else if (unitDetail == UnitDetail.Wizard)
            {
                bullet = ObjectManager.instance.GetBullet(idx + ((int)UnitDetail.Wizard + 1), targetTrans.position);
            }
        }
        // Bullet�� �� �Ѱ��ֱ�
        ObjectManager.instance.BulletSetting(bullet, unitDetail, gameObject.layer, unitAtk);

        // Skill
        if (unitDetail == UnitDetail.Guitar)
        {
            ExeBuffType("ATS", 0.05f);
        }

        // Attack
        anim.SetTrigger("doAttack");
        attackTimer = 0;
    }
    void BombAttack()
    {
        // ����Ʈ ��������
        GameObject bullet = null;

        int idx = (int)unitDetail - (int)UnitDetail.Bomb + ((int)UnitDetail.Wizard + 1) * 2;
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

            if (gameObject.layer == 8)
            {
                if (unitDetail == UnitDetail.Drum)
                {
                    bullet = ObjectManager.instance.GetBullet(idx, transform.position + Vector3.right * 0.5f);
                }
            }
            else if (gameObject.layer == 9)
            {
                if (unitDetail == UnitDetail.Drum)
                {
                    bullet = ObjectManager.instance.GetBullet(idx, transform.position + Vector3.left * 0.5f);
                }
            }

            // Attack
            anim.SetTrigger("doAttack");
            attackTimer = 0;
        }

        // Bullet�� �� �Ѱ��ֱ�
        ObjectManager.instance.BulletSetting(bullet, unitDetail, gameObject.layer, unitAtk);
    }

    // ======================================================= ���� �Լ�
    public void DoBuff(Unit allyLogic)
    {
        // ���ݼӵ��� ���� ����
        attackTimer += Time.deltaTime;
        if (attackTimer < unitAtkSpeed)
            return;

        // Bullet
        string type = "";
        float value = 0f;
        int idx = (int)unitDetail - (int)UnitDetail.Bomb + ((int)UnitDetail.Wizard + 1) * 2;

        if (gameObject.layer == 8)          // ��� �� ������ ���
        {
            // Ÿ���� �������� �߻�
            if (unitDetail == UnitDetail.Heal)
            {
                type = "HP";
                value = 1;
                Vector3 vec = new Vector3(-0.5f, 0.5f);
                ObjectManager.instance.GetBullet(idx, allyLogic.transform.position + vec);
            }
            else if (unitDetail == UnitDetail.AtkUp)
            {
                type = "ATK";
                value = 1;
                allyLogic.atkBuffCount += 1;
                Vector3 vec = new Vector3(-0.5f, 0.5f);
                ObjectManager.instance.GetBullet(idx, allyLogic.transform.position + vec);
            }
        }
        else if (gameObject.layer == 9)     // ���� �� ������ ���
        {
            if (unitDetail == UnitDetail.Heal)
            {
                type = "HP";
                value = 1;
                Vector3 vec = new Vector3(0.5f, 0.5f);
                ObjectManager.instance.GetBullet(idx, allyLogic.transform.position + vec);
            }
            else if (unitDetail == UnitDetail.AtkUp)
            {
                type = "ATK";
                value = 1;
                atkBuffCount += 1;
                Vector3 vec = new Vector3(0.5f, 0.5f);
                ObjectManager.instance.GetBullet(idx, allyLogic.transform.position + vec);
            }
        }

        // ���� �ߵ�
        allyLogic.ExeBuffType(type, value);
        anim.SetTrigger("doAttack");
        attackTimer = 0;

        if (isHit)
        {
            anim.SetTrigger("doHit");
            isHit = false;
        }
    }
    void ExeBuffType(string type, float value)
    {
        // �Ҽ��� ����
        int valueInt = Mathf.FloorToInt(value);

        switch (type)
        {
            case "HP":
                unitHp += valueInt;
                unitHp = unitHp > unitMaxHp ? unitMaxHp : unitHp;
                break;
            case "ATK":
                // ���� ��ø�� 2������
                if (atkBuffCount < 3)
                    unitAtk += valueInt;
                else
                    atkBuffCount = 3;
                break;
            case "ATS":
                unitAtkSpeed -= value;
                unitAtkSpeed = unitAtkSpeed < 0.3f ? 0.3f : unitAtkSpeed;
                break;
            case "RNG":
                unitRange += value;
                break;
            case "SPD":
                unitSpeed += value;
                break;
        }

    }

    // ======================================================= ���� �Լ�
    void DoMove()
    {
        // �̹� �����̰� ������ ȣ�� X
        if (unitState == UnitState.Move)
            return;

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
        if (unitDetail == UnitDetail.Berserker)
        {
            // ����ü�� 1 �� 0.1�� ���� ����
            unitAtkSpeed -= damage * 0.1f;
            unitAtkSpeed = unitAtkSpeed < 0.3f ? 0.3f : unitAtkSpeed;
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
        if (unitDetail == UnitDetail.Devil)
        {
            // Sensor ����
            GameManager.instance.allSensor.SetActive(false);
        }
        else if (unitDetail == UnitDetail.AtkspdUp)
        {
            // Sensor ����
            atsSensor.SetActive(false);
        }
        for (int i = 0; i < atkBuffCount - 1; i++)
        {
            // �������� ���� ���� Ƚ����ŭ ����
            unitAtk -= 1;
        }

        unitState = UnitState.Die;
        gameObject.SetActive(false);
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        // Blue �� ������ Red Sensor�� ����� ��� || Red �� ������ Blue Sensor�� ����� ���
        if ((gameObject.layer == 8 && collision.gameObject.layer == 11) || (gameObject.layer == 9 && collision.gameObject.layer == 10))
        {
            // ����Ʈ ��ġ���� ����
            Vector3 vec = Vector3.zero;
            if (gameObject.layer == 8)
                vec = new Vector3(-0.5f, 1.5f);
            else if (gameObject.layer == 9)
                vec = new Vector3(0.5f, 1.5f);

            // Hit
            devilTimer += Time.deltaTime;
            if (devilTimer > 2.5f)  // ������ �޴� �ֱ� (2.5f)
            {
                // ����Ʈ
                int idx = (int)UnitDetail.Devil - (int)UnitDetail.Bomb + ((int)UnitDetail.Wizard + 1) * 2;
                ObjectManager.instance.GetBullet(idx, transform.position + vec);
                // Hit
                DoHit(2);
                devilTimer = 0;
            }
        }

        // Blue �� ������ Blue Buff Sensor�� ����� ��� || Red �� ������ Red Buff Sensor�� ����� ���
        if ((gameObject.layer == 8 && collision.gameObject.tag == "AtkSpd_UpB") || (gameObject.layer == 9 && collision.gameObject.tag == "AtkSpd_UpR"))
        {
            // ���� ����
            isATSSensor = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if ((gameObject.layer == 8 && collision.gameObject.tag == "AtkSpd_UpB") || (gameObject.layer == 9 && collision.gameObject.tag == "AtkSpd_UpR"))
        {
            // ���� ����
            isATSSensor = false;
        }
    }
}
