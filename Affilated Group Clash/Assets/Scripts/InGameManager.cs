using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using Image = UnityEngine.UI.Image;
using Slider = UnityEngine.UI.Slider;
using System.Collections;

public class InGameManager : MonoBehaviour
{
    public static InGameManager instance;

    [Header("---------------[InGame]")]
    public bool isGameLive;
    public int maxCost;
    public int blueCost;
    public int redCost;
    public float gameTimer;
    public float costBTimer;
    public float costRTimer;
    public float costRedUp;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI costText;
    public List<GameObject> blueUnitList;
    public List<GameObject> redUnitList;

    [Header("---------------[Team Setting]")]
    public float spawnTimer;
    public int patternIdx;

    [Header("---------------[Base]")]
    public int blueHP;
    public int redHP;
    public Sprite[] blueBaseSprite;
    public Sprite[] redBaseSprite;
    public SpriteRenderer blueBaseSpriteRen;
    public SpriteRenderer redBaseSpriteRen;
    public Slider blueBaseSlider;
    public Slider redBaseSlider;
    public TextMeshProUGUI blueHpText;
    public TextMeshProUGUI redHpText;
    public TextMeshProUGUI blueHpShadowText;
    public TextMeshProUGUI redHpShadowText;
    public GameObject blueDestroyEffect;
    public GameObject redDestroyEffect;


    public GameObject optionPanel;

    [Header("---------------[Button UI]")]
    public Image[] blueButtonImage;
    public TextMeshProUGUI[] blueTypeText;
    public TextMeshProUGUI[] blueCostText;
    public GameObject lastButton;
    [Header("---------------[Unit Info UI]")]
    public bool isUnitClick;
    public GameObject unitObj;
    public Image unitImage;
    public Slider hpSlider;
    public TextMeshProUGUI unitNameText;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI atkText;
    public TextMeshProUGUI atsText;
    public TextMeshProUGUI ranText;
    public TextMeshProUGUI spdText;

    [Header("---------------[Result]")]
    public GameObject overSet;
    public GameObject controlSet;
    public GameObject victoryObj;
    public GameObject defeatObj;

    [Header("---------------[Devil]")]
    public bool isDevilB;
    public bool isDevilR;
    public float devilBTimer;
    public float devilRTimer;
    public bool isDevilBAttack;
    public bool isDevilRAttack;

    [Header("---------------[Camera]")]
    public Transform camTrans;
    public float camSpeed;
    public bool isMove;

    void Awake()
    {
        instance = this;
        isGameLive = true;

        // Team Button Setting
        for (int i = Variables.startBlueIdx; i < Variables.startBlueIdx + Variables.groupBlueNum; i++)
        {
            Unit teamUnit = Variables.teamBluePrefabs[i].GetComponent<Unit>();
            SpriteRenderer spriteRen = teamUnit.GetComponent<SpriteRenderer>();
            // Image
            blueButtonImage[i - Variables.startBlueIdx].sprite = spriteRen.sprite;
            // Type
            TypeTextSetting(blueTypeText[i - Variables.startBlueIdx], teamUnit.unitType);
            // Cost
            blueCostText[i - Variables.startBlueIdx].text = teamUnit.unitCost.ToString();
        }

        // 5명이면 마지막버튼 끄기
        if (Variables.groupBlueNum == 5)
            lastButton.SetActive(false);

        // Base Text Update
        blueHpText.text = blueHP.ToString();
        redHpText.text = redHP.ToString();
        blueHpShadowText.text = blueHP.ToString();
        redHpShadowText.text = redHP.ToString();

        // Game Level
        GameLevel();
    }
    void TypeTextSetting(TextMeshProUGUI text, UnitType typeName)
    {
        switch (typeName)
        {
            case UnitType.Tanker:
                text.text = "탱커";
                text.color = new Color(0, 255, 0);
                break;
            case UnitType.Warrior:
                text.text = "전사";
                text.color = new Color(255, 0, 0);
                break;
            case UnitType.Ranger:
                text.text = "원딜";
                text.color = new Color(0, 200, 255);
                break;
            case UnitType.Buffer:
                text.text = "버프";
                text.color = new Color(255, 255, 0);
                break;
            case UnitType.Special:
                text.text = "특수";
                text.color = new Color(255, 0, 255);
                break;
        }
    }
    void GameLevel()
    {
        switch (Variables.gameLevel)
        {
            case 0:
                costRedUp = 2.25f;
                break;
            case 1:
                costRedUp = 2f;
                break;
            case 2:
                costRedUp = 1.75f;
                break;
            case 3:
                costRedUp = 1.5f;
                break;
            case 4:
                costRedUp = 1.25f;
                break;
        }

    }

    void Update()
    {
        if (!isGameLive)
            return;

        // Timer
        gameTimer += Time.deltaTime;
        timeText.text = ((int)gameTimer / 60).ToString("D2") + ":" + ((int)gameTimer % 60).ToString("D2");

        // Devil
        DevilTimer();
        // Camera Move
        CameraMove();
        // Cost
        BlueCostUp();
        RedCostUp();
        // KeyBoard
        KeyBoard();

        // Loop Pattern
        //StartCoroutine(Pattern(1f, patternIdx));

        // Unit Infomation
        if (isUnitClick)
        {
            Unit unitLogic = unitObj.GetComponent<Unit>();
            UnitInfo(unitLogic);
        }
    }
    // ======================================================= Update 함수
    void DevilTimer()
    {
        if (isDevilB)
        {
            devilBTimer += Time.deltaTime;
            if (devilBTimer > 2.5f)
            {
                isDevilBAttack = true;
                devilBTimer = 0;
            }
            else
                isDevilBAttack = false;
        }
        if (isDevilR)
        {
            devilRTimer += Time.deltaTime;
            if (devilRTimer > 2f)
            {
                isDevilRAttack = true;
                devilRTimer = 0;
            }
            else
                isDevilRAttack = false;
        }
    }
    void CameraMove()
    {
        // 화면 밖으로 이동하려고 하면 Move 중지
        if ((camSpeed == -3f && camTrans.position.x > -5) || (camSpeed == 3f && camTrans.position.x < 5))
            isMove = true;
        else
            isMove = false;

        if (isMove)
        {
            // 다음 벡터값만큼 이동
            Vector3 nextMove = Vector3.right * camSpeed * Time.deltaTime;
            camTrans.Translate(nextMove);
        }
    }
    void BlueCostUp()
    {
        costText.text = blueCost.ToString();

        costBTimer += Time.deltaTime;
        if (costBTimer > 2f)
        {
            blueCost += 1;
            blueCost = blueCost > maxCost ? maxCost : blueCost;

            costBTimer = 0;
        }
    }
    void RedCostUp()
    {
        costRTimer += Time.deltaTime;
        if (costRTimer > costRedUp)
        {
            redCost += 1;
            redCost = redCost > maxCost ? maxCost : redCost;

            costRTimer = 0;
        }
    }
    void KeyBoard()
    {
        // 키보드를 통한 이동
        if (Input.GetKey(KeyCode.RightArrow))
            camSpeed = 3f;
        if (Input.GetKey(KeyCode.LeftArrow))
            camSpeed = -3f;
        if (Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.LeftArrow))
            camSpeed = 0;
        if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow))
            camSpeed = 0;

        // 키보드를 통한 유닛 생성
        // 2조로 나뉘어져있는 그룹의 프리펩에는 Blue팀 Red팀 + Blue팀 Red팀으로 섞여있음
        // teamNum에 따라 Blue/ Red로 나누어주었으며, startIdx가 두 팀을 나누는 기준으로 사용하였음
        if (Input.GetKeyDown(KeyCode.Q))
            MakeBlueUnit(0 + Variables.startBlueIdx);
        if (Input.GetKeyDown(KeyCode.W))
            MakeBlueUnit(1 + Variables.startBlueIdx);
        if (Input.GetKeyDown(KeyCode.E))
            MakeBlueUnit(2 + Variables.startBlueIdx);
        if (Input.GetKeyDown(KeyCode.A))
            MakeBlueUnit(3 + Variables.startBlueIdx);
        if (Input.GetKeyDown(KeyCode.S))
            MakeBlueUnit(4 + Variables.startBlueIdx);
        if (Input.GetKeyDown(KeyCode.D) && Variables.groupBlueNum == 6)
            MakeBlueUnit(5 + Variables.startBlueIdx);
        // red
        if (Input.GetKeyDown(KeyCode.Z))
            MakeRedUnit(0 + Variables.startRedIdx + Variables.groupRedNum);
        if (Input.GetKeyDown(KeyCode.X))
            MakeRedUnit(1 + Variables.startRedIdx + Variables.groupRedNum);
        if (Input.GetKeyDown(KeyCode.C))
            MakeRedUnit(2 + Variables.startRedIdx + Variables.groupRedNum);
        if (Input.GetKeyDown(KeyCode.V))
            MakeRedUnit(3 + Variables.startRedIdx + Variables.groupRedNum);
        if (Input.GetKeyDown(KeyCode.B))
            MakeRedUnit(4 + Variables.startRedIdx + Variables.groupRedNum);
        if (Input.GetKeyDown(KeyCode.N) && Variables.groupRedNum == 6)
            MakeRedUnit(5 + Variables.startRedIdx + Variables.groupRedNum);
    }
    void UnitInfo(Unit unitLogic)
    {
        unitImage.gameObject.SetActive(true);

        // Image
        SpriteRenderer spriteRen = unitLogic.GetComponent<SpriteRenderer>();
        unitImage.sprite = spriteRen.sprite;
        // Name
        unitNameText.text = unitLogic.unitName;
        // Hp
        hpSlider.value = (float)unitLogic.unitHp / unitLogic.unitMaxHp;
        hpText.text = $"{unitLogic.unitHp} / {unitLogic.unitMaxHp}";
        // Atk
        atkText.text = $"ATK : {unitLogic.unitAtk}";
        // Atk Speed
        string floatAts = unitLogic.unitAtkSpeed.ToString("F1");
        atsText.text = $"ATS : " + floatAts;
        // Range
        ranText.text = $"RAN : {unitLogic.unitRange}";
        // Speed
        if (unitLogic.unitSpeed > 0)
            spdText.text = $"SPD : {unitLogic.unitSpeed * 10}";
        else
            spdText.text = $"SPD : {unitLogic.unitSpeed * -10}";
    }

    // ======================================================= 인게임 버튼 함수
    public void CameraMoveButton(string type)
    {
        // Set Speed By Button
        if (type == "RightDown" || type == "LeftUp") // 오른쪽 버튼을 누르거나 왼쪽버튼을 떼면 2 더하기
            camSpeed += 3f;
        else if (type == "RightUp" || type == "LeftDown")
            camSpeed -= 3f;
    }
    public void MakeBlueUnit(int idx)
    {
        GameObject unitB = Variables.teamBluePrefabs[idx];
        Unit unitBLogic = unitB.GetComponent<Unit>();

        // 예외처리
        if (isDevilB && unitBLogic.unitDetail == UnitDetail.Devil)
            return;

        // Cost 감소
        blueCost -= unitBLogic.unitCost;
        if (blueCost < 0)
        {
            blueCost += unitBLogic.unitCost;
            return;
        }
        // 생성
        GetUnitObject(Variables.teamBlueNum, idx, unitB.transform.position);
        blueUnitList.Add(unitB);
    }
    public void MakeRedUnit(int idx)
    {
        GameObject unitR = Variables.teamRedPrefabs[idx];
        Unit unitRLogic = unitR.GetComponent<Unit>();

        // 예외처리
        if (isDevilR && unitRLogic.unitDetail == UnitDetail.Devil)
            return;

        // Cost 감소
        redCost -= unitRLogic.unitCost;
        if (redCost < 0)
        {
            redCost += unitRLogic.unitCost;
            return;
        }
        // 생성
        GetUnitObject(Variables.teamRedNum, idx, unitR.transform.position);
        redUnitList.Add(unitR);

        // 생성 후에 패턴인덱스 변경
        patternIdx = Random.Range(0, 3);
    }
    void GetUnitObject(int teamNum, int idx, Vector3 pos)
    {
        switch (teamNum)
        {
            case 0:
            case 1:
                ObjectManager.instance.GetGiHa(idx, pos);
                break;
            case 2:
                ObjectManager.instance.GetJuFok(idx, pos);
                break;
            case 3:
            case 4:
                ObjectManager.instance.GetBakChi(idx, pos);
                break;
            case 5:
                ObjectManager.instance.GetVBand(idx, pos);
                break;
        }
    }
    public void OptionButton()
    {
        optionPanel.SetActive(true);
        Time.timeScale = 0;
    }
    public void OptionOut()
    {
        optionPanel.SetActive(false);
        Time.timeScale = 1;
    }
    public void ResetButton()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Game");
    }

    // ======================================================= Base 함수
    public void BaseHit(int dmg, int layer)
    {
        // 게임이 끝났으면 Base Hit 금지
        if (!isGameLive)
            return;

        if (layer == 8)
        {
            // 데미지만큼 감소
            blueHP -= dmg;

            // Death
            if (blueHP <= 0)
            {
                blueHP = 0;
                blueBaseSpriteRen.sprite = blueBaseSprite[2];
                blueDestroyEffect.SetActive(true);
                isGameLive = false;
                GameOver("Lose");
            }
            // Alive
            else
            {
                blueBaseSpriteRen.sprite = blueBaseSprite[1];
                StartCoroutine(SpriteChange(0.1f, blueBaseSpriteRen, blueBaseSprite[0]));
            }

            // Text
            blueBaseSlider.value = blueHP;
            blueHpText.text = blueHP.ToString();
            blueHpShadowText.text = blueHP.ToString();
        }
        else if (layer == 9)
        {
            redHP -= dmg;

            if (redHP <= 0)
            {
                redHP = 0;
                redBaseSpriteRen.sprite = redBaseSprite[2];
                redDestroyEffect.SetActive(true);
                GameOver("Win");
            }
            else
            {
                redBaseSpriteRen.sprite = redBaseSprite[1];
                StartCoroutine(SpriteChange(0.1f, redBaseSpriteRen, redBaseSprite[0]));
            }

            redBaseSlider.value = redHP;
            redHpText.text = redHP.ToString();
            redHpShadowText.text = redHP.ToString();
        }
    }
    IEnumerator SpriteChange(float time, SpriteRenderer spriteRen, Sprite sprite)
    {
        yield return new WaitForSeconds(time);

        spriteRen.sprite = sprite;
    }
    void GameOver(string result)
    {
        // 이겼을 경우
        if (result == "Win")
        {
            victoryObj.SetActive(true);
        }
        // 졌을 경우
        else if (result == "Lose")
        {
            defeatObj.SetActive(true);
        }

        // 게임 작동 중지 (시간, 코스트, 카메라이동, 소환버튼, 유닛클릭)
        isGameLive = false;
        camSpeed = 0;
        controlSet.SetActive(false);
        overSet.SetActive(true);
    }

    // ======================================================= COM 함수
    // 패턴을 메모장을 이용해 직접 제작하기?
    // 랜덤값을 통해 알아서 소환하기?
    IEnumerator Pattern(float time, int typeNum)
    {
        yield return new WaitForSeconds(time);

        // random 결정용 변수
        int rand = Random.Range(0, 2);
        switch (typeNum)
        {
            // 3코스트 이상일 때, 0번유닛과 1번유닛을 랜덤하게 소환
            case 0:
                if (redCost >= 3)
                    MakeRedUnit(rand + Variables.startRedIdx + Variables.groupRedNum);
                break;
            // 6코스트 이상일 때, rand값이 0이면 0번패턴, 1이면 2번유닛과 3번유닛을 랜덤하게 소환
            case 1:
                if (redCost >= 6)
                {
                    int idx = Random.Range(2, 4);
                    if (rand == 0)
                        StartCoroutine(Pattern(0, 0));
                    else if (rand == 1)
                        MakeRedUnit(idx + Variables.startRedIdx + Variables.groupRedNum);
                }
                break;
            // 9코스트 이상일 때, rand값이 0이면 1번패턴, 1이면 4번유닛을 소환
            case 2:
                if (redCost >= 9)
                {
                    if (rand == 0)
                        StartCoroutine(Pattern(0, 1));
                    else if (rand == 1)
                    {
                        // 팀이 6명일 경우
                        if (Variables.groupRedNum == 6)
                        {
                            int idx = Random.Range(4, 6);
                            MakeRedUnit(idx + Variables.startRedIdx + Variables.groupRedNum);
                        }
                        // 팀이 5명일 경우
                        else
                        {
                            MakeRedUnit(4 + Variables.startRedIdx + Variables.groupRedNum);
                        }
                    }
                }
                break;
        }
    }
}
