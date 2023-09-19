using System.Collections;
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
        // 시작 값 설정
        if (parent != null)
        {
            unitDetail = parent.unitDetail;
            layer = parent.gameObject.layer;
            col.radius = parent.unitRange;
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        // 게임 중에만 실행
        if (!InGameManager.instance.isGameLive)
            return;

        // 아군일 경우
        if ((layer == 8 && collision.gameObject.layer == 8) || (layer == 9 && collision.gameObject.layer == 9))
        {
            if (unitDetail == UnitDetail.Heal)
            {
                Unit unitLogic = collision.GetComponent<Unit>();

                // 자기 자신은 힐 금지
                if (unitLogic.unitDetail == UnitDetail.Heal)
                    return;

                // 아군의 체력이 닳았을 경우
                if (unitLogic.unitHp < unitLogic.unitMaxHp)
                {
                    // 이미 힐중이면 return
                    if (parent.isHeal)
                        return;

                    parent.DoBuff(unitLogic);
                    parent.isHeal = true;
                    // 공격속도만큼 지난 뒤에 힐 가능
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
