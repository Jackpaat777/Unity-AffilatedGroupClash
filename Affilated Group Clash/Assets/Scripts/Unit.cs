using System.Collections;
using System.Collections.Generic;
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

public enum UnitDetail
{
    // Bullet B ��� ����
    Archer, Sniper, Farmer, Guitar, Wizard,
    // Bullet T ��� ����
    Bomb, Drum, Book, Vampire, Punch,
    Sword, Guard,
    Noblilty
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

    float attackTimer;
    float stopTimer;
    float moneyTimer;
    bool isFront;
    bool isHit;

    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();

        // ���� ����
        unitState = UnitState.Idle;
        isFront = false;
        DoMove();
    }

    void Update()
    {
        // ����
        if (unitDetail == UnitDetail.Noblilty)
        {
            moneyTimer += Time.deltaTime;
            if (moneyTimer > unitAtkSpeed)
            {
                DoStop();
                // CostUp
                if (gameObject.layer== 8 && GameManager.instance.blueCost < 10)
                {
                    GameManager.instance.blueCost += 1;
                }
                else if (gameObject.layer == 9 && GameManager.instance.redCost < 10)
                {
                    GameManager.instance.redCost += 1;
                }

                anim.SetTrigger("doAttack");
                moneyTimer = 0;
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
            else if (unitType == UnitType.Buffer)
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
            
            if (unitDetail == UnitDetail.Book)
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
            GameObject bullet = ObjectManager.instance.bullet_prefabs[idx + 10];
            Instantiate(bullet, transform.position + Vector3.up * 0.5f, Quaternion.identity);
            SetBuffType("HP", 1);
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
                    GameObject bullet = ObjectManager.instance.bullet_prefabs[idx + 10];
                    Vector3 vec = new Vector3(0.5f, 0.5f);
                    Instantiate(bullet, transform.position + vec, Quaternion.identity);
                }
                else if (gameObject.layer == 9)
                {
                    GameObject bullet = ObjectManager.instance.bullet_prefabs[idx + 10];
                    Vector3 vec = new Vector3(-0.5f, 0.5f);
                    Instantiate(bullet, transform.position + vec, Quaternion.identity);
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
            if (unitDetail == UnitDetail.Archer || unitDetail == UnitDetail.Sniper || unitDetail == UnitDetail.Farmer || unitDetail == UnitDetail.Guitar)
            {
                bullet = ObjectManager.instance.bullet_prefabs[idx];
                Instantiate(bullet, transform.position + Vector3.up * 0.3f, Quaternion.identity);
            }
            // Ÿ�ٸ� �������� �Ѿ� �߻�
            else if (unitDetail == UnitDetail.Wizard)
            {
                bullet = ObjectManager.instance.bullet_prefabs[idx];
                Instantiate(bullet, targetTrans.position, Quaternion.identity);
            }
        }
        else if (gameObject.layer == 9)     // ���� �� ������ ���
        {
            if (unitDetail == UnitDetail.Archer || unitDetail == UnitDetail.Sniper || unitDetail == UnitDetail.Farmer || unitDetail == UnitDetail.Guitar)
            {
                bullet = ObjectManager.instance.bullet_prefabs[idx + 5];
                Instantiate(bullet, transform.position + Vector3.up * 0.3f, Quaternion.identity);
            }
            else if (unitDetail == UnitDetail.Wizard)
            {
                bullet = ObjectManager.instance.bullet_prefabs[idx + 5];
                Instantiate(bullet, targetTrans.position, Quaternion.identity);
            }
        }
        // �Ѿ˿� �� �ֱ�
        Bullet bulletLogic = bullet.GetComponent<Bullet>();
        bulletLogic.dmg = unitAtk;

        // Skill
        if (unitDetail == UnitDetail.Guitar)
        {
            SetBuffType("ATS", 0.05f);
        }

        // Attack
        anim.SetTrigger("doAttack");
        attackTimer = 0;
    }
    void BombAttack()
    {
        // ���� ����Ʈ ��������
        GameObject bomb = null;
        int idx = (int)unitDetail - (int)UnitDetail.Bomb;
        if (unitDetail == UnitDetail.Bomb)
        {
            // ����
            DoHit(unitMaxHp);
            bomb = ObjectManager.instance.bullet_prefabs[idx + 10];
            Instantiate(bomb, transform.position, Quaternion.identity);
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
                    bomb = ObjectManager.instance.bullet_prefabs[idx + 10];
                    Instantiate(bomb, transform.position + Vector3.right * 0.5f, Quaternion.identity);
                }
            }
            else if (gameObject.layer == 9)
            {
                if (unitDetail == UnitDetail.Drum)
                {
                    bomb = ObjectManager.instance.bullet_prefabs[idx + 10];
                    Instantiate(bomb, transform.position + Vector3.left * 0.5f, Quaternion.identity);
                }
            }
            
            // Attack
            anim.SetTrigger("doAttack");
            attackTimer = 0;
        }
        

        // Bullet�� �� �Ѱ��ֱ�
        Bullet bombLogic = bomb.GetComponent<Bullet>();
        bombLogic.unitDetail = unitDetail;
        bombLogic.layer = gameObject.layer;
        bombLogic.dmg = unitAtk;
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
        int idx = (int)unitDetail - (int)UnitDetail.Bomb;

        if (gameObject.layer == 8)          // ��� �� ������ ���
        {
            // Ÿ���� �������� �߻�
            if (unitDetail == UnitDetail.Book)
            {
                type = "HP";
                value = 1;
                GameObject bullet = ObjectManager.instance.bullet_prefabs[idx + 10];
                Vector3 vec = new Vector3(-0.5f, 0.5f);
                Instantiate(bullet, allyLogic.transform.position + vec, Quaternion.identity);
            }
        }
        else if (gameObject.layer == 9)     // ���� �� ������ ���
        {
            if (unitDetail == UnitDetail.Book)
            {
                type = "HP";
                value = 1;
                GameObject bullet = ObjectManager.instance.bullet_prefabs[idx + 10];
                Vector3 vec = new Vector3(0.5f, 0.5f);
                Instantiate(bullet, allyLogic.transform.position + vec, Quaternion.identity);
            }
        }

        // ���� �ߵ�
        allyLogic.SetBuffType(type, value);
        anim.SetTrigger("doAttack");
        attackTimer = 0;

        if (isHit)
        {
            anim.SetTrigger("doHit");
            isHit = false;
        }
    }
    void SetBuffType(string type, float value)
    {
        int valueInt = Mathf.FloorToInt(value);

        switch (type)
        {
            case "HP":
                unitHp += valueInt;
                unitHp = unitHp > unitMaxHp ? unitMaxHp : unitHp;
                break;
            case "ATK":
                unitAtk += valueInt;
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
        unitState = UnitState.Die;
        Destroy(gameObject);
    }
}
