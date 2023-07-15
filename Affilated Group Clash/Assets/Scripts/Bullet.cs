using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    public bool isHit;
    public int dmg;

    void Update()
    {
        Vector3 nextMove = Vector3.right * speed * Time.deltaTime;
        transform.Translate(nextMove);

        if (transform.position.x < -8 || transform.position.x > 8)
            Destroy(gameObject);
    }

    void FixedUpdate()
    {
        if (!isHit)
        {
            ScanEnemy();
        }
    }

    void ScanEnemy()
    {
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
        RaycastHit2D rayHit = Physics2D.Raycast(transform.position, dir, 0.4f, LayerMask.GetMask(enemyLayer));

        // 적군이 감지됨
        if (rayHit.collider != null)
        {
            // 적 오브젝트 가져오기
            Unit enemyLogic = rayHit.collider.gameObject.GetComponent<Unit>();

            isHit = true;
            enemyLogic.DoHit(dmg);

            // 화살인지 마법인지에 따라 삭제 시간 조절
            if (speed == 0)
                Destroy(gameObject, 3f);
            else
                Destroy(gameObject);
        }
    }
}
