using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public UnitName unitName;
    public int layer;
    public int dmg;

    void OnTriggerStay2D(Collider2D collision)
    {
        if (unitName == UnitName.Bomb)
        {
            if (layer == 8 && collision.gameObject.layer == 9)
            {
                Unit unitLogic = collision.GetComponent<Unit>();
                unitLogic.DoHit(dmg);
            }
            else if (layer == 9 && collision.gameObject.layer == 8)
            {
                Unit unitLogic = collision.GetComponent<Unit>();
                unitLogic.DoHit(dmg);
            }

            Destroy(gameObject, 1.5f);
        }
    }
}
