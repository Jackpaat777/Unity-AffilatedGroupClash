using UnityEngine;

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
                    parent.DoBuff(unitLogic);
                }
            }
        }
    }
}
