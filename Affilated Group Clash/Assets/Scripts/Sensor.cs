using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Sensor : MonoBehaviour
{
    public UnitDetail unitDetail;
    public int layer;

    Unit parent;
    CircleCollider2D col;

    void Awake()
    {
        parent = GetComponentInParent<Unit>();
        col = GetComponent<CircleCollider2D>();
    }

    void Start()
    {
        // ���� �� ����
        if (parent != null)
        {
            unitDetail = parent.unitDetail;
            layer = parent.gameObject.layer;
            col.radius = parent.unitRange;
        }
    }

    // �Ʊ� ���� Ʈ���� (�Ѹ� ����)
    void OnTriggerStay2D(Collider2D collision)
    {
        // �Ʊ��� ���
        if ((layer == 8 && collision.gameObject.layer == 8) || (layer == 9 && collision.gameObject.layer == 9))
        {
            if (unitDetail == UnitDetail.Heal)
            {
                Unit unitLogic = collision.GetComponent<Unit>();
                // �Ʊ��� ü���� ����� ���
                if (unitLogic.unitHp < unitLogic.unitMaxHp)
                {
                    // �̹� �����̸� return
                    if (parent.isHeal)
                        return;

                    parent.DoBuff(unitLogic);
                    parent.isHeal = true;
                    // ���ݼӵ���ŭ ���� �ڿ� �� ����
                    StartCoroutine(OnHeal(parent.unitAtkSpeed));
                }
            }
        }
    }
    IEnumerator OnHeal(float time)
    {
        yield return new WaitForSeconds(time);

        parent.isHeal = false;
    }

}
