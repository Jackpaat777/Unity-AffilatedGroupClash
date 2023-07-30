using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEngine.UI.CanvasScaler;

public class Bullet : MonoBehaviour
{
    public UnitDetail unitDetail;
    public int layer;
    public int dmg;
    public float speed;
    public bool isHit;
    public bool isRotate;
    public bool isNotAtk;
    public bool isDebuff;

    Rigidbody2D rigid;
    Collider2D col;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    void OnEnable()
    {
        transform.localRotation = Quaternion.identity;
        isHit = false;

        if (col != null)
            col.enabled = true;

        // ��������Ʈ�� speed���� ������ �������� ����(Rotate�� �ϱ�����)
        if (unitDetail != UnitDetail.Devil)
            rigid.AddForce(Vector2.right * speed, ForceMode2D.Impulse);
    }

    void Update()
    {
        if (isRotate)
            transform.Rotate(Vector3.forward * speed);

        // Circle�� ����
        if (unitDetail != UnitDetail.AtkUp || unitDetail != UnitDetail.AtkspdUp || unitDetail != UnitDetail.SpdUp || unitDetail != UnitDetail.Piano)
        {
            // ȭ�� ������ ���� ��� ����
            if (transform.position.x < -15 || transform.position.x > 15)
                gameObject.SetActive(false);
        }

        // 2.5�ʳѰ� ���ư��� ������ ���� -> ������ ����?
        DisableRoutine(2.5f);
    }

    void FixedUpdate()
    {
        // �������� ����, �Ʊ� ���� ������ ScanEnemy���� �Ѿ
        if (unitDetail == UnitDetail.Stick || unitDetail == UnitDetail.Bomb || unitDetail == UnitDetail.Wizard ||
            unitDetail == UnitDetail.AtkUp || unitDetail == UnitDetail.AtkspdUp || unitDetail == UnitDetail.SpdUp || unitDetail == UnitDetail.Piano)
            return;

        // �������� �ʴ� Bullet�� ��� ScanEnemy���� �ٷ� ����
        if (isNotAtk)
        {
            if (unitDetail == UnitDetail.Devil)
            {
                StartCoroutine(DisableRoutine(0.5f));
                return;
            }

            // �̵����� �ʴ� ��� 2�� �ڿ� ����
            if (speed == 0)
                StartCoroutine(DisableRoutine(1f));
            else
                gameObject.SetActive(false);
            return;
        }

        ScanSensor();
        ScanEnemy();
    }

    void ScanEnemy()
    {
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
        RaycastHit2D rayHit = Physics2D.Raycast(transform.position, dir, 0.4f, LayerMask.GetMask(enemyLayer));

        // ������ ������
        if (rayHit.collider != null)
        {
            // �� ������Ʈ ��������
            Unit enemyLogic = rayHit.collider.gameObject.GetComponent<Unit>();

            isHit = true;
            enemyLogic.DoHit(dmg);

            // ����� �Ѿ�
            if (isDebuff)
            {
                // Singer�� ������ ���ݷ� ���� (1����)
                if (unitDetail == UnitDetail.Singer)
                {
                    // �̹� ����� ���̸� �ߵ����� ����
                    if (!enemyLogic.isAtkDebuff)
                    {
                        enemyLogic.unitAtk -= 5;
                        enemyLogic.unitAtk = enemyLogic.unitAtk < 0 ? 0 : enemyLogic.unitAtk;
                        enemyLogic.isAtkDebuff = true;
                    }
                    // Ÿ�̸Ӵ� �ʱ�ȭ
                    enemyLogic.atkDebuffTimer = 0;
                }
                // Bass�� ������ �̼� ���� (1����)
                if (unitDetail == UnitDetail.Bass)
                {
                    if (!enemyLogic.isSpdDebuff)
                    {
                        if (enemyLogic.gameObject.layer == 8)
                            enemyLogic.unitSpeed -= 0.4f;
                        else if (enemyLogic.gameObject.layer == 9)
                            enemyLogic.unitSpeed += 0.4f;
                        enemyLogic.isSpdDebuff = true;
                    }
                    enemyLogic.spdDebuffTimer = 0;
                }
            }

            // Destroy
            gameObject.SetActive(false);
        }
    }

    // Destroy Bullet ���� ��ĵ
    void ScanSensor()
    {
        Vector2 dir = Vector2.zero;
        string enemyLayer = "";

        if (gameObject.layer == 8)
        {
            dir = Vector2.right;
            enemyLayer = "DestroyR";
        }
        else if (gameObject.layer == 9)
        {
            dir = Vector2.left;
            enemyLayer = "DestroyB";
        }

        RaycastHit2D rayHit = Physics2D.Raycast(transform.position, dir, 0.4f, LayerMask.GetMask(enemyLayer));

        if (rayHit.collider != null)
        {
            // Destroy
            gameObject.SetActive(false);
        }
    }

    // �������� Ʈ����
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (unitDetail == UnitDetail.Stick || unitDetail == UnitDetail.Wizard || unitDetail == UnitDetail.Bomb)
        {
            // ������ ���
            if ((layer == 8 && collision.gameObject.layer == 9) || (layer == 9 && collision.gameObject.layer == 8))
            {
                Unit unitLogic = collision.GetComponent<Unit>();

                unitLogic.DoHit(dmg);
            }

            // collider�� �ٷ� ���� (��Ʈ�Ӹ����� ������ ���ӵ����� �޴� ����)
            if (col != null)
                col.enabled = false;
            StartCoroutine(DisableRoutine(1f));
        }
    }

    IEnumerator DisableRoutine(float time)
    {
        yield return new WaitForSeconds(time);

        gameObject.SetActive(false);
    }
}
