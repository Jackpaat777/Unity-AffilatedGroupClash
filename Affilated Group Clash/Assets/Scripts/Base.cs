using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Base : MonoBehaviour
{
    public int baseHP;
    public RectTransform baseHpObj;
    public TextMeshProUGUI baseHpText;
    public TextMeshProUGUI baseHpShadowText;
    public SpriteRenderer baseBaseSpriteRed;
    public Sprite[] baseSprite;
    public GameObject destroyEffect;

    public void HitBase(int dmg)
    {
        baseHP -= dmg;
    }
}
