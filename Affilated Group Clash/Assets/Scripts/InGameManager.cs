using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Random = UnityEngine.Random;
using Image = UnityEngine.UI.Image;
using Slider = UnityEngine.UI.Slider;
using Button = UnityEngine.UI.Button;
using UnityEngine.UIElements;

public class InGameManager : MonoBehaviour
{
    public static InGameManager instance;

    [Header("---------------[InGame]")]
    public bool isGameLive;
    public int blueMaxCost;
    public int redMaxCost;
    public int blueCost;
    public int redCost;
    public float gameTimer;
    public float costBTimer;
    public float costRTimer;
    public float costBlueUp;
    public float costRedUp;
    public float redUpgradeTime;
    public float spawnTimer;
    public bool[] onCoolTime;
    public float[] btnCoolTimer;
    public Image[] btnCoolImage;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI costText;

    [Header("---------------[Pattern]")]
    public bool isUpgradeTime;
    public bool isRedBomb;
    public int patternIdx;
    public int[] patternCost;
    public int[] patternRatio;
    public int redUnitCount;
    public int[] spawnBlueData;
    public int[] spawnRedData;

    [Header("---------------[Base]")]
    public int baseBLevel;
    public int baseRLevel;
    public int blueHP;
    public int redHP;
    public Unit baseB;
    public Unit baseR;
    public Sprite[] baseBSprite;
    public Sprite[] baseRSprite;
    public SpriteRenderer baseBSpriteRen;
    public SpriteRenderer baseRSpriteRen;
    public Slider baseBSlider;
    public Slider baseRSlider;
    public TextMeshProUGUI blueHpText;
    public TextMeshProUGUI redHpText;
    public TextMeshProUGUI blueHpShadowText;
    public TextMeshProUGUI redHpShadowText;
    public TextMeshProUGUI blueUpgradeCostText;
    public TextMeshProUGUI upgradeText;
    public GameObject blueDestroyEffect;
    public GameObject redDestroyEffect;
    public GameObject[] blueUpgradeyEffect;
    public GameObject[] redUpgradeyEffect;


    [Header("---------------[Top UI]")]
    public Animator fadeAc;
    public GameObject optionPanel;
    public GameObject restartButton;
    public TextMeshProUGUI levelText;
    public Image blueLogo;
    public Image redLogo;
    public Sprite[] logoSprites;
    [Header("---------------[Button UI]")]
    public Image[] blueButtonImage;
    public TextMeshProUGUI[] blueTypeText;
    public TextMeshProUGUI[] blueCostText;
    public GameObject lastButton;
    public GameObject[] xObject;
    [Header("---------------[Unit Info UI]")]
    public bool isUnitClick;
    public GameObject unitObj;
    public Image unitImage;
    public Slider hpSlider;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI atkText;
    public TextMeshProUGUI atsText;
    public TextMeshProUGUI ranText;
    public TextMeshProUGUI spdText;
    [Header("---------------[Team Logo UI]")]
    public int logoPage;
    public float scaleNum;
    public string teamType;
    public GameObject teamInfoPanel;
    public Image unitPictureImage;
    public Image[] unitButtonImage;
    public TextMeshProUGUI[] unitButtonNameText;
    public Sprite[] unitPictureSprites;
    public TextMeshProUGUI unitNameText;
    public TextMeshProUGUI unitTypeText;
    public TextMeshProUGUI unitCostText;
    public TextMeshProUGUI unitHpText;
    public TextMeshProUGUI unitAtkText;
    public TextMeshProUGUI unitAtsText;
    public TextMeshProUGUI unitRanText;
    public TextMeshProUGUI unitSpdText;
    public TextMeshProUGUI unitSkillText;

    [Header("---------------[Result]")]
    public bool onFire;
    public int burstCount;
    public float fireTimer;
    public GameObject overSet;
    public GameObject controlSet;
    public GameObject victoryObj;
    public GameObject clearObj;
    public GameObject defeatObj;
    public GameObject nextBtn;
    public GameObject resetBtn;
    public ParticleSystem[] burstObj;

    [Header("---------------[Spawn]")]
    public bool isDevilB;
    public bool isDevilR;
    public bool isCostB;
    public bool isCostR;
    public bool isHealB;
    public bool isHealR;
    public float devilBTimer;
    public float devilRTimer;
    public bool isDevilBStart;
    public bool isDevilRStart;
    public bool isDevilBAttack;
    public bool isDevilRAttack;

    [Header("---------------[Description]")]
    public int descriptionPage;
    public Button optionButton;
    public Button teamBLogoButton;
    public Button teamRLogoButton;
    public Button upgradeButton;
    public TextMeshProUGUI descriptionText;
    public GameObject descriptionPanel;
    public GameObject desPrevButton;
    public GameObject desNextButton;
    public GameObject[] descriptionRect;

    [Header("---------------[Camera]")]
    public Transform camTrans;
    public float camSpeed;
    public bool isMove;

    void Awake()
    {
        instance = this;
        Application.targetFrameRate = 60;

        isGameLive = true;

        // Fade In
        fadeAc.SetTrigger("fadeIn");
        // Fade 없앤 뒤 게임설명 띄우기
        StartCoroutine(DisableFade(0.5f));

        // Game Setting
        GameSetting();
    }
    void GameSetting()
    {
        // BlueTeam Setting
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

        // 5명팀이면 마지막버튼 끄기
        if (Variables.groupBlueNum == 5)
            lastButton.SetActive(false);
        
        // Red Setting
        RedSetting();

        // Base
        baseBSpriteRen.gameObject.SetActive(true);
        baseRSpriteRen.gameObject.SetActive(true);
        blueHpText.text = blueHP.ToString();
        redHpText.text = redHP.ToString();
        blueHpShadowText.text = blueHP.ToString();
        redHpShadowText.text = redHP.ToString();

        // Logo
        blueLogo.sprite = logoSprites[Variables.teamBlueNum];
        redLogo.sprite = logoSprites[Variables.teamRedNum];

        // Restart Button
        if (Variables.isStage)
            restartButton.SetActive(false);
        else
            restartButton.SetActive(true);

        // X Image
        xObject[0].SetActive(false);
        xObject[1].SetActive(false);
        xObject[2].SetActive(false);
        xObject[3].SetActive(false);
        xObject[4].SetActive(false);
        xObject[5].SetActive(false);

        // BGM
        SoundManager.instance.BgmPlay("Game");
    }
    void RedSetting()
    {
        // Red Team Ratio
        switch (Variables.teamRedNum)
        {
            case 0:
                patternRatio[0] = 30;   // 30%
                patternRatio[1] = 50;   // 20%
                patternRatio[2] = 70;   // 20%
                patternRatio[3] = 85;   // 15%
                patternRatio[4] = 100;  // 15%
                break;
            case 1:
                patternRatio[0] = 30;   // 30%
                patternRatio[1] = 50;   // 20%
                patternRatio[2] = 70;   // 20%
                patternRatio[3] = 85;   // 15%
                patternRatio[4] = 100;  // 15%
                break;
            case 2:
                patternRatio[0] = 30;   // 30%
                patternRatio[1] = 50;   // 20%
                patternRatio[2] = 65;   // 15%
                patternRatio[3] = 75;   // 10%
                patternRatio[4] = 90;   // 15%  10%
                break;
            case 3:
                patternRatio[0] = 25;   // 25%
                patternRatio[1] = 50;   // 25%
                patternRatio[2] = 65;   // 15%
                patternRatio[3] = 75;   // 10%
                patternRatio[4] = 90;   // 15%  10%
                break;
            case 4:
                patternRatio[0] = 25;   // 25%
                patternRatio[1] = 50;   // 25%
                patternRatio[2] = 70;   // 20%
                patternRatio[3] = 85;   // 15%
                patternRatio[4] = 100;  // 15%
                break;
            case 5:
                patternRatio[0] = 25;   // 25%
                patternRatio[1] = 50;   // 25%
                patternRatio[2] = 70;   // 20%
                patternRatio[3] = 90;   // 20%
                patternRatio[4] = 100;  // 10%
                break;
        }


        // Red Team Cost
        for (int i = Variables.startRedIdx; i < Variables.startRedIdx + Variables.groupRedNum; i++)
        {
            Unit redUnit = Variables.teamRedPrefabs[i].GetComponent<Unit>();
            patternCost[i - Variables.startRedIdx] = redUnit.unitCost;
        }

        // RedTeam Level
        switch (Variables.gameLevel)
        {
            case 0:
                // 코스트 증가 시간
                costRedUp = 3f;
                // 업그레이드 시간
                redUpgradeTime = 180;
                // 몇초에 한명씩 나오는지
                spawnTimer = 3f;
                // 레벨 텍스트 변경
                levelText.text = "매우쉬움";
                break;
            case 1:
                costRedUp = 2.5f;
                redUpgradeTime = 120;
                spawnTimer = 2f;
                levelText.text = "쉬움";
                break;
            case 2:
                costRedUp = 2.25f;
                redUpgradeTime = 80;
                spawnTimer = 2f;
                levelText.text = "보통";
                break;
            case 3:
                costRedUp = 2f;
                redUpgradeTime = 60;
                spawnTimer = 1.5f;
                levelText.text = "어려움";
                break;
            case 4:
                costRedUp = 1.75f;
                redUpgradeTime = 60;
                spawnTimer = 1.5f;
                levelText.text = "매우어려움";
                break;
        }
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
    IEnumerator DisableFade(float time)
    {
        yield return new WaitForSeconds(time);

        fadeAc.gameObject.SetActive(false);

        // 게임 설명 띄우기
        if (!Variables.isAlreadyGame)
        {
            DescriptionButton();
            Variables.isAlreadyGame = true;
            PlayerPrefs.SetInt("First", 1);
        }
    }

    void Update()
    {
        // Option
        Option();

        if (!isGameLive)
        {
            // FireWork
            ExecuteFireWork();
            return;
        }

        // Timer
        GameTimer();
        // Button
        ButtonTimer();
        // Devil
        DevilTimer();
        // Camera Move
        CameraMove();
        // Red Level Up
        RedLevelUp();
        // Red Upgrade;
        RedUpgrade();
        // Cost
        BlueCostUp();
        RedCostUp();
        // KeyBoard
        KeyBoardControl();
        // Unit Infomation
        UnitInfo();

        // Loop Pattern
        StartCoroutine(Pattern(spawnTimer, patternIdx));
    }

    void FixedUpdate()
    {
        if (!isGameLive)
            return;

    }
    // ======================================================= Update 함수
    void RedLevelUp()
    {
        switch (Variables.gameLevel)
        {
            case 2:
                if (gameTimer < 40f)
                {
                    costRedUp = 2.25f;
                    spawnTimer = 2f;
                }
                else if (gameTimer < 80f)
                {
                    costRedUp = 2f;
                    spawnTimer = 1.75f;
                }
                else
                    spawnTimer = 1.5f;
                break;
            case 3:
                if (gameTimer < 40f)
                {
                    costRedUp = 2f;
                    spawnTimer = 1.5f;
                }
                else if (gameTimer < 80f)
                {
                    costRedUp = 1.75f;
                    spawnTimer = 1.2f;
                }
                else
                    spawnTimer = 1f;
                break;
            case 4:
                if (gameTimer < 30f)
                {
                    costRedUp = 2f;
                    spawnTimer = 1.5f;
                }
                else if (gameTimer < 60f)
                {
                    costRedUp = 1.75f;
                    spawnTimer = 1.2f;
                }
                else
                    spawnTimer = 1f;
                break;
        }
    }
    void GameTimer()
    {
        gameTimer += Time.deltaTime;
        timeText.text = ((int)gameTimer / 60).ToString("D2") + ":" + ((int)gameTimer % 60).ToString("D2");
    }
    void ButtonTimer()
    {
        for (int i = 0; i < 6; i++)
        {
            // 쿨타임 시작
            if (onCoolTime[i])
            {
                // 쿨타임 돌리기
                btnCoolTimer[i] += Time.deltaTime;
                // 쿨타임 이미지
                btnCoolImage[i].fillAmount = 1f - (btnCoolTimer[i] / 3f);
                // 3초 쿨타임
                if (btnCoolTimer[i] > 3f)
                {
                    btnCoolTimer[i] = 0;
                    onCoolTime[i] = false;
                }
            }
        }
    }
    void DevilTimer()
    {
        if (isDevilBStart)
        {
            devilBTimer += Time.deltaTime;
            if (devilBTimer > 3f)
            {
                isDevilBAttack = true;
                devilBTimer = 0;
            }
            else
                isDevilBAttack = false;
        }
        if (isDevilRStart)
        {
            devilRTimer += Time.deltaTime;
            if (devilRTimer > 3f)
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
        if ((camSpeed == -4f && camTrans.position.x > -6) || (camSpeed == 4f && camTrans.position.x < 6))
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
    void RedUpgrade()
    {
        if ((gameTimer > redUpgradeTime && baseRLevel == 0) || (gameTimer > redUpgradeTime * 2.5f && baseRLevel == 1))
        {
            isUpgradeTime = true;
            UpgradeRed();
        }
    }
    void BlueCostUp()
    {
        costText.text = $"{blueCost}/{blueMaxCost}";

        costBTimer += Time.deltaTime;
        if (costBTimer > costBlueUp)
        {
            blueCost += 1;
            blueCost = blueCost > blueMaxCost ? blueMaxCost : blueCost;

            costBTimer = 0;
        }
    }
    void RedCostUp()
    {
        costRTimer += Time.deltaTime;
        if (costRTimer > costRedUp)
        {
            redCost += 1;
            redCost = redCost > redMaxCost ? redMaxCost : redCost;

            costRTimer = 0;
        }
    }
    void KeyBoardControl()
    {
        // 키보드를 통한 이동
        if (Input.GetKey(KeyCode.RightArrow))
            camSpeed = 4f;
        if (Input.GetKey(KeyCode.LeftArrow))
            camSpeed = -4f;
        if (Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.LeftArrow))
            camSpeed = 0;
        if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow))
            camSpeed = 0;

        // 키보드를 통한 유닛 생성
        if (Input.GetKeyDown(KeyCode.Q))
            MakeBlueUnit(0);
        if (Input.GetKeyDown(KeyCode.W))
            MakeBlueUnit(1);
        if (Input.GetKeyDown(KeyCode.E))
            MakeBlueUnit(2);
        if (Input.GetKeyDown(KeyCode.A))
            MakeBlueUnit(3);
        if (Input.GetKeyDown(KeyCode.S))
            MakeBlueUnit(4);
        if (Input.GetKeyDown(KeyCode.D) && Variables.groupBlueNum == 6)
            MakeBlueUnit(5);

        // 키보드로 업그레이드
        if (Input.GetKeyDown(KeyCode.B))
            UpgradeBlueButton();

        // red
        //if (Input.GetKeyDown(KeyCode.I))
        //    MakeRedUnit(0 + Variables.startRedIdx + Variables.groupRedNum);
        //if (Input.GetKeyDown(KeyCode.O))
        //    MakeRedUnit(1 + Variables.startRedIdx + Variables.groupRedNum);
        //if (Input.GetKeyDown(KeyCode.P))
        //    MakeRedUnit(2 + Variables.startRedIdx + Variables.groupRedNum);
        //if (Input.GetKeyDown(KeyCode.J))
        //    MakeRedUnit(3 + Variables.startRedIdx + Variables.groupRedNum);
        //if (Input.GetKeyDown(KeyCode.K))
        //    MakeRedUnit(4 + Variables.startRedIdx + Variables.groupRedNum);
        //if (Input.GetKeyDown(KeyCode.L) && Variables.groupRedNum == 6)
        //    MakeRedUnit(5 + Variables.startRedIdx + Variables.groupRedNum);
    }
    void Option()
    {
        // 일시정지 버튼
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (optionPanel.activeSelf)
                OptionOut();
            else
                OptionButton();
        }
    }
    void UnitInfo()
    {
        if (!isUnitClick)
        {
            unitImage.gameObject.SetActive(false);
            nameText.text = "-";
            hpSlider.value = 1;
            hpText.text = "- / -";
            atkText.text = "ATK : -";
            atsText.text = "ATS : -";
            ranText.text = "RAN : -";
            spdText.text = "SPD : -";
            return;
        }

        // Unit Setting
        Unit unitLogic = unitObj.GetComponent<Unit>();
        unitImage.gameObject.SetActive(true);

        // Image
        SpriteRenderer spriteRen = unitLogic.GetComponent<SpriteRenderer>();
        unitImage.sprite = spriteRen.sprite;
        // Name
        nameText.text = unitLogic.unitName;
        // Hp
        hpSlider.value = (float)unitLogic.unitHp / unitLogic.unitMaxHp;
        hpText.text = $"{unitLogic.unitHp} / {unitLogic.unitMaxHp}";
        // Atk
        atkText.text = $"ATK : {unitLogic.unitAtk}";
        // Atk Speed
        string floatAts = unitLogic.unitAtkSpeed.ToString("F1");
        atsText.text = $"ATS : " + floatAts;
        // Range
        ranText.text = $"RAN : {unitLogic.unitRange * 10}";
        // Speed
        string floatSpd = "";
        if (unitLogic.unitSpeed > 0)
            floatSpd = (unitLogic.unitSpeed * 10).ToString("F0");
        else
            floatSpd = (unitLogic.unitSpeed * -10).ToString("F0");
        spdText.text = $"SPD : " + floatSpd;

        // 유닛이 죽으면 상세정보 초기화
        if (unitLogic.unitHp <= 0)
            isUnitClick = false;
    }
    void ExecuteFireWork()
    {
        if (!onFire)
            return;

        fireTimer += Time.deltaTime;
        if (fireTimer > 2f)
        {
            burstObj[burstCount].Play();
            burstCount++;
            burstCount = burstCount > 4 ? 0 : burstCount;

            fireTimer = 0;
        }
    }
    // ======================================================= 인게임 버튼 함수
    public void CameraMoveButton(string type)
    {
        // Set Speed By Button
        if (type == "RightDown" || type == "LeftUp") // 오른쪽 버튼을 누르거나 왼쪽버튼을 떼면 2 더하기
            camSpeed += 4f;
        else if (type == "RightUp" || type == "LeftDown")
            camSpeed -= 4f;
    }
    // 업그레이드
    public void UpgradeBlueButton()
    {
        // 기지 업그레이드
        if (baseBLevel < 2)
        {
            // 코스트 사용
            if (blueCost < 10)
                return;

            blueCost -= 10;
            blueMaxCost += 5;
            // 이펙트
            blueUpgradeyEffect[baseBLevel].SetActive(true);
            // Level Up
            baseBLevel++;
            // 스프라이트 변경 (집, 빌딩, 성)
            baseBSpriteRen.sprite = baseBSprite[baseBLevel];

            // cost 텍스트 변경
            if (baseBLevel == 2)
            {
                blueUpgradeCostText.text = "-";
                upgradeText.text = "Max";
            }
            else
                blueUpgradeCostText.text = "10";

            //코스트 증가 속도 상승
            costBlueUp -= 0.25f;
            // 베이스 강화
            baseB.unitAtk += 2;
            baseB.unitAtkSpeed -= 0.1f;

            // Sound
            SoundManager.instance.SfxPlay("Upgrade");
        }
    }
    void UpgradeRed()
    {
        // red는 7만 있어도 업그레이드 가능
        if (redCost < 7 || baseRLevel == 2)
            return;
        redCost -= 7;
        redMaxCost += 5;
        redUpgradeyEffect[baseRLevel].SetActive(true);
        baseRLevel++;
        costRedUp -= 0.25f;
        baseRSpriteRen.sprite = baseRSprite[baseRLevel];

        baseR.unitAtk += 2;
        baseR.unitAtkSpeed -= 0.1f;

        isUpgradeTime = false;
        // Sound
        SoundManager.instance.SfxPlay("Upgrade");
    }
    // 유닛 소환
    public void MakeBlueUnit(int idx)
    {
        // 쿨타임 동안은 소환 못함 (Blue팀 시작인덱스만큼 빼서 적용)
        if (onCoolTime[idx])
            return;

        GameObject unitB = Variables.teamBluePrefabs[idx + Variables.startBlueIdx];
        Unit unitBLogic = unitB.GetComponent<Unit>();

        // 예외처리
        if ((isDevilB && unitBLogic.unitDetail == UnitDetail.Devil) || (isCostB && unitBLogic.unitDetail == UnitDetail.CostUp) || (isHealB && unitBLogic.unitDetail == UnitDetail.Heal))
            return;

        // 3명 제한
        if (spawnBlueData[idx] == 3)
            return;

        // Cost 감소
        blueCost -= unitBLogic.unitCost;
        if (blueCost < 0)
        {
            blueCost += unitBLogic.unitCost;
            return;
        }
        // 생성
        GetUnitObject(Variables.teamBlueNum, idx + Variables.startBlueIdx, unitB.transform.position);
        // 쿨타임 시작
        onCoolTime[idx] = true;
        // 유닛 데이터
        spawnBlueData[idx]++;
        if (spawnBlueData[idx] == 3)
            xObject[idx].SetActive(true);

        // Sound
        SoundManager.instance.SfxPlay("Buy");
    }
    public void MakeRedUnit(int idx)
    {
        GameObject unitR = Variables.teamRedPrefabs[idx];
        Unit unitRLogic = unitR.GetComponent<Unit>();
        int rand = Random.Range(Variables.startRedIdx + Variables.groupRedNum, Variables.startRedIdx + Variables.groupRedNum + Variables.groupRedNum);

        // 최대 유닛 개수 제한
        if ((Variables.teamRedNum == 0 && redUnitCount == 13) ||
            (Variables.teamRedNum == 1 && redUnitCount == 13) ||
            (Variables.teamRedNum == 2 && redUnitCount == 18) ||
            (Variables.teamRedNum == 3 && redUnitCount == 16) ||
            (Variables.teamRedNum == 4 && redUnitCount == 15) ||
            (Variables.teamRedNum == 5 && redUnitCount == 15))
            return;


        // 예외처리
        if ((isDevilR && unitRLogic.unitDetail == UnitDetail.Devil) || (isCostR && unitRLogic.unitDetail == UnitDetail.CostUp) || (isHealR && unitRLogic.unitDetail == UnitDetail.Heal))
        {
            MakeRedUnit(rand);
            return;
        }
        // 버퍼 소환 조건
        else if (redUnitCount < 2 && unitRLogic.unitType == UnitType.Buffer)
        {
            MakeRedUnit(rand);
            return;
        }

        // 3명 제한
        if (spawnRedData[idx - Variables.startRedIdx - Variables.groupRedNum] == 3)
        {
            MakeRedUnit(rand);
            return;
        }

        // Cost 감소
        redCost -= unitRLogic.unitCost;
        if (redCost < 0)
        {
            redCost += unitRLogic.unitCost;
            return;
        }
        // 생성
        GetUnitObject(Variables.teamRedNum, idx, unitR.transform.position);
        redUnitCount++;
        // 유닛 데이터
        spawnRedData[idx - Variables.startRedIdx - Variables.groupRedNum]++;

        // Sound
        SoundManager.instance.SfxPlay("Buy");

        // 생성 후에 패턴인덱스 변경
        PatternRatio();
    }
    void PatternRatio()
    {
        int rand = Random.Range(0, 100);

        // 패턴 인덱스 결정
        if (rand < patternRatio[0])
            patternIdx = 0;
        else if (rand < patternRatio[1])
            patternIdx = 1;
        else if (rand < patternRatio[2])
            patternIdx = 2;
        else if (rand < patternRatio[3])
            patternIdx = 3;
        else if (rand < patternRatio[4])
            patternIdx = 4;
        else
            patternIdx = 5;
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
    // 옵션 버튼
    public void OptionButton()
    {
        optionPanel.SetActive(true);
        isGameLive = false;
        Time.timeScale = 0;
        // Sound
        SoundManager.instance.SfxPlay("Button1");
    }
    public void OptionOut()
    {
        optionPanel.SetActive(false);
        isGameLive = true;
        Time.timeScale = 1;
        // Sound
        SoundManager.instance.SfxPlay("Button1");
    }
    public void UnitButton(int typeNum)
    {
        isGameLive = false;
        // Time Stop
        Time.timeScale = 0;

        // 해당 버튼의 유닛 가져오기
        Unit unitLogic = Variables.teamBluePrefabs[Variables.startBlueIdx + typeNum].GetComponent<Unit>();

        // Left Panel
        unitPictureImage.sprite = UnitImage(typeNum);
        unitPictureImage.SetNativeSize();
        unitPictureImage.transform.localScale = Vector3.one * scaleNum;
        unitNameText.text = unitLogic.unitName;
        TypeTextSetting(unitTypeText, unitLogic.unitType);

        // Right Panel
        unitCostText.text = $"코인 : {unitLogic.unitCost}";
        unitHpText.text = $"체력 : {unitLogic.unitMaxHp}";
        unitAtkText.text = $"공격 : {unitLogic.unitAtk}";
        unitAtsText.text = $"공격속도 : {unitLogic.unitAtkSpeed}";
        unitRanText.text = $"공격범위 : {unitLogic.unitRange * 10}";
        unitSpdText.text = $"이동속도 : {unitLogic.unitSpeed * 10}";
        unitSkillText.text =  UnitSkillText("Blue", typeNum);

        // 패널 켜기
        teamInfoPanel.SetActive(true);
    }
    Sprite UnitImage(int idx)
    {
        int teamNum = Variables.teamBlueNum;
        Sprite image = null;

        switch (teamNum)
        {
            case 0:
                switch (idx)
                {
                    case 0:
                        image = unitPictureSprites[0];
                        break;
                    case 1:
                        image = unitPictureSprites[1];
                        break;
                    case 2:
                        image = unitPictureSprites[2];
                        break;
                    case 3:
                        image = unitPictureSprites[3];
                        break;
                    case 4:
                        image = unitPictureSprites[4];
                        break;
                }
                scaleNum = 0.22f;
                break;
            case 1:
                switch (idx)
                {
                    case 0:
                        image = unitPictureSprites[5];
                        break;
                    case 1:
                        image = unitPictureSprites[6];
                        break;
                    case 2:
                        image = unitPictureSprites[7];
                        break;
                    case 3:
                        image = unitPictureSprites[8];
                        break;
                    case 4:
                        image = unitPictureSprites[9];
                        break;
                }
                scaleNum = 0.22f;
                break;
            case 2:
                switch (idx)
                {
                    case 0:
                        image = unitPictureSprites[10];
                        break;
                    case 1:
                        image = unitPictureSprites[11];
                        break;
                    case 2:
                        image = unitPictureSprites[12];
                        break;
                    case 3:
                        image = unitPictureSprites[13];
                        break;
                    case 4:
                        image = unitPictureSprites[14];
                        break;
                    case 5:
                        image = unitPictureSprites[15];
                        break;
                }
                scaleNum = 0.45f;
                break;
            case 3:
                switch (idx)
                {
                    case 0:
                        image = unitPictureSprites[16];
                        break;
                    case 1:
                        image = unitPictureSprites[17];
                        break;
                    case 2:
                        image = unitPictureSprites[18];
                        break;
                    case 3:
                        image = unitPictureSprites[19];
                        break;
                    case 4:
                        image = unitPictureSprites[20];
                        break;
                    case 5:
                        image = unitPictureSprites[21];
                        break;
                }
                scaleNum = 0.22f;
                break;
            case 4:
                switch (idx)
                {
                    case 0:
                        image = unitPictureSprites[22];
                        break;
                    case 1:
                        image = unitPictureSprites[23];
                        break;
                    case 2:
                        image = unitPictureSprites[24];
                        break;
                    case 3:
                        image = unitPictureSprites[25];
                        break;
                    case 4:
                        image = unitPictureSprites[26];
                        break;
                }
                scaleNum = 0.22f;
                break;
            case 5:
                switch (idx)
                {
                    case 0:
                        image = unitPictureSprites[27];
                        break;
                    case 1:
                        image = unitPictureSprites[28];
                        break;
                    case 2:
                        image = unitPictureSprites[29];
                        break;
                    case 3:
                        image = unitPictureSprites[30];
                        break;
                    case 4:
                        image = unitPictureSprites[31];
                        break;
                }
                scaleNum = 0.45f;
                break;
        }
        return image;
    }
    string UnitSkillText(string teamType, int idx)
    {
        int teamNum = 0;
        if (teamType == "Blue")
            teamNum = Variables.teamBlueNum;
        else if (teamType == "Red")
            teamNum = Variables.teamRedNum;

        string skillText = "";

        switch (teamNum)
        {
            case 0:
                switch (idx)
                {
                    case 0:
                        skillText = "일반적인 탱커.";
                        break;
                    case 1:
                        skillText = "공격에 적중당한 적 2초간 공격력 절반 감소.\n(디버프 중첩불가)";
                        break;
                    case 2:
                        skillText = "HP가 적어질수록 공격속도 증가.\n(공격속도 최대치 : 0.5)";
                        break;
                    case 3:
                        skillText = "범위 내의 아군 전체에게 공격속도 0.5만큼 버프.\n(버프 중첩불가)";
                        break;
                    case 4:
                        skillText = "적 전체에게 피해를 입힘. 기지는 공격하지 못함.\n(공격속도 고정)\n(중복 소환 불가)";
                        break;
                }
                break;
            case 1:
                switch (idx)
                {
                    case 0:
                        skillText = "일반적인 전사.";
                        break;
                    case 1:
                        skillText = "적에게 피해받을 때마다 적의 공격력의 절반만큼 피해를 되돌려줌.\n(근접 적만 해당)";
                        break;
                    case 2:
                        skillText = "공격에 적중당한 적 2초간 공격속도 0.5만큼 디버프.\n(디버프 중첩불가)";
                        break;
                    case 3:
                        skillText = "범위 내 아군 한명에게 5만큼 힐.\n(중복 소환 불가)";
                        break;
                    case 4:
                        skillText = "적을 킬하면 최대체력 +5, 공격력 +2, 공격속도 0.1만큼 증가\n(최대 5중첩)";
                        break;
                }
                break;
            case 2:
                switch (idx)
                {
                    case 0:
                        skillText = "일반적인 전사.";
                        break;
                    case 1:
                        skillText = "자신이 받는 모든 피격데미지가 2 감소되어 적용.\n(최소데미지 1)";
                        break;
                    case 2:
                        skillText = "범위 내 탱커 이동속도 2 증가, 전사 공격속도 0.2 증가, 원딜 공격력 2 증가.\n(버프 중첩불가)";
                        break;
                    case 3:
                        skillText = "광역으로 원거리 공격.";
                        break;
                    case 4:
                        skillText = "적이 본인과 근접범위까지 오면 백스탭. (쿨타임 3초)";
                        break;
                    case 5:
                        skillText = "체력이 40이하가 되면 3초간 모든 데미지를 받지 않음.\n(한번만 발동)";
                        break;
                }
                break;
            case 3:
                switch (idx)
                {
                    case 0:
                        skillText = "일반적인 탱커.";
                        break;
                    case 1:
                        skillText = "공격 시 본인의 공격력의 절반만큼 체력 흡혈. 체력이 20이하가 되면 공격속도 0.5만큼 버프.";
                        break;
                    case 2:
                        skillText = "광역으로 근접 공격.";
                        break;
                    case 3:
                        skillText = "3초마다 1코인씩 증가.\n(중복 소환 불가)";
                        break;
                    case 4:
                        skillText = "일반적인 원딜.\n(가장 긴 공격범위)";
                        break;
                    case 5:
                        skillText = "범위 내에 적이 있다면 빠르게 이동 후 자폭. 적 최대체력의 절반만큼 데미지.\n(기지의 경우 100 데미지)";
                        break;
                }
                break;
            case 4:
                switch (idx)
                {
                    case 0:
                        skillText = "일반적인 탱커.";
                        break;
                    case 1:
                        skillText = "3번째 공격마다 추가 데미지 +2.";
                        break;
                    case 2:
                        skillText = "공격할 때마다 공격속도 증가.\n(공격속도 최대치 : 0.7)\n(이동 시 초기화)";
                        break;
                    case 3:
                        skillText = "범위 내 아군 전체 공격력 2 증가.\n(버프 중첩불가)";
                        break;
                    case 4:
                        skillText = "공격 시 체력 20이하 적 처형.";
                        break;
                }
                break;
            case 5:
                switch (idx)
                {
                    case 0:
                        skillText = "일반적인 전사.";
                        break;
                    case 1:
                        skillText = "공격할 때마다 공격력 2 증가\n(최대 2 중첩)\n(이동 시 초기화)";
                        break;
                    case 2:
                        skillText = "범위 원거리 투사체 공격 무효.\n(광역공격 또는 기지의 공격은 해당하지 않음.)";
                        break;
                    case 3:
                        skillText = "적 기지의 체력이 절반 이하일 때, 공격력, 공격속도 2배 버프";
                        break;
                    case 4:
                        skillText = "적 기지까지 이동할 동안 공격하지 않음.\n적에게 피격당해도 멈추지 않음.";
                        break;
                }
                break;
        }

        return skillText;
    }
    public void BackLogoButton()
    {
        isGameLive = true;
        // Time Start
        Time.timeScale = 1;
        // 패널 끄기
        teamInfoPanel.SetActive(false);
    }
    // 설명 버튼
    public void DescriptionButton()
    {
        if (!descriptionPanel.activeSelf)
        {
            isGameLive = false;
            // 멈춤
            Time.timeScale = 0;
            // 패널 켜기
            descriptionPanel.SetActive(true);
            Description("None");
            // 다른 버튼 작동중지
            optionButton.interactable = false;
            teamBLogoButton.interactable = false;
            teamRLogoButton.interactable = false;
            upgradeButton.interactable = false;
        }
        else if (descriptionPanel.activeSelf)
        {
            isGameLive = true;
            // 이미 켜져있는 경우 다시 끄기
            Time.timeScale = 1;
            // 초기화
            descriptionPage = 0;
            descriptionPanel.SetActive(false);
            // 이전버튼 끄기
            desPrevButton.SetActive(false);
            // 다른버튼 작동
            optionButton.interactable = true;
            teamBLogoButton.interactable = true;
            teamRLogoButton.interactable = true;
            upgradeButton.interactable = true;
            for (int i = 0; i < 8; i++)
            {
                // 사각형 다 끄기
                descriptionRect[i].SetActive(false);
            }
        }
    }
    public void PrevDescriptionButton()
    {
        descriptionPage--;

        if (descriptionPage == 0)
            desPrevButton.SetActive(false);
        else
            desNextButton.SetActive(true);

        Description("Prev");
    }
    public void NextDescriptionButton()
    {
        descriptionPage++;

        // 다봤으면 나가기
        if (descriptionPage > 11)
        {
            DescriptionButton();
            return;
        }

        desPrevButton.SetActive(true);

        Description("Next");
    }
    void Description(string btnType)
    {
        // 사각형 키고 끄기
        if (descriptionPage == 0 || descriptionPage == 1)
        {
            // 첫 페이지와 두번째에는 사각형이 안나와야함
            descriptionRect[0].SetActive(false);
        }
        else if (descriptionPage > 1)
        {
            // 사각형 키기 (마지막 페이지에는 사각형이 없으므로 못킴)
            if (descriptionPage < 11)
                descriptionRect[descriptionPage - 2].SetActive(true);

            // 다음 버튼을 눌렀을 경우
            if (btnType == "Next")
            {
                // 이전 사각형 끄기(두번째 이상일 때)
                if (descriptionPage > 2)
                    descriptionRect[descriptionPage - 3].SetActive(false);
            }
            // 이전버튼을 눌렀을 경우
            else if (btnType == "Prev")
            {
                // 다음 사각형 끄기(10번째 이하일 때)
                if (descriptionPage < 10)
                    descriptionRect[descriptionPage - 1].SetActive(false);
            }
        }

        // 설명란
        switch (descriptionPage)
        {
            case 0:
                descriptionText.text = "침하잎하~ <color=blue>산하대격돌</color>에 오신것을 환영합니다!\n\n그럼 바로 게임설명을 하도록 하겠습니다!";
                break;
            case 1:
                descriptionText.text = "가장 먼저 <color=red>화면 이동</color>입니다.\n\n<color=red>키보드의 화살표키</color>를 통해\n좌우로 화면을 이동하실 수 있습니다.\n\n(게임설명 중에는 작동하지 않습니다.)";
                break;
            case 2:
                descriptionText.text = "두번째는 <color=red>멤버 소환</color>입니다.\n\n이곳에는 해당 멤버을 뽑기 위한\n코인, 멤버의 타입이 적혀있습니다.\n\n" +
                    "멤버를 소환하기 위해서는 해당하는\n<color=red>키보드를 눌러서 소환</color>하실 수 있습니다.\n\n모든 멤버는 <color=red>최대 3명 소환가능하며 3초간 쿨타임</color>이 존재합니다.";
                break;
            case 3:
                descriptionText.text = "<color=red>멤버 버튼을 클릭</color>하면 해당 <color=red>멤버에 대한\n정보를 확인</color>하실 수 있습니다.\n\n멤버의 스탯, 스킬에 대해 궁금하시다면\n버튼을 클릭해주세요!";
                break;
            case 4:
                descriptionText.text = "이 곳은 <color=red>멤버 상세정보</color>입니다.\n\n현재 <color=red>소환된 멤버를 클릭</color>하여 해당멤버의\n실시간 정보를 확인하실 수 있습니다!\n\n" +
                    "<color=blue>ATK</color>는 공격력, <color=blue>ATS</color>는 공격속도,\n<color=blue>RAN</color>은 공격범위, <color=blue>SPD</color>는 이동속도를 의미합니다.";
                break;
            case 5:
                descriptionText.text = "주의할 점으로 공격속도는 해당 수치만큼\n시간이 지나면 한 번 공격한다는 의미로,\n<color=red>수치가 낮을수록 공격속도가 빨라짐을 의미</color>합니다.\n\n" +
                    "(예시 : 공속 1.5는 1.5초당 한 번 공격함)";
                break;
            case 6:
                descriptionText.text = "다음으로 <color=red>현재 코인</color>입니다.\n\n코인이 부족하면 멤버를 소환할 수 없으며,\n코인은 자동으로 1씩 증가합니다.";
                break;
            case 7:
                descriptionText.text = "<color=red>기지 업그레이드</color>입니다.\n\n기지는 스스로 일정 범위 내에서\n적을 인식해 공격합니다.\n\n" +
                    "<color=red>B버튼</color>을 통해 10코인을 소모하여\n<color=blue>최대 코인 보유량, 코인 증가 속도, 기지의 공격력, 공격속도를 증가</color>시킬 수 있습니다.";
                break;
            case 8:
                descriptionText.text = "기지 업그레이드는 <color=red>최대 2번</color>까지 가능하며,\n업그레이드가 최대인 경우 기지의 공격이 <color=red>광역 공격</color>으로 바뀝니다.";
                break;
            case 9:
                descriptionText.text = "이곳에서는 <color=red>현재 게임 난이도와\n양 팀의 남은 체력</color>을 확인하실 수 있습니다.\n\n" +
                    "양 팀 모두 300으로 시작하며\n가장 먼저 체력이 0이 되는 팀이 패배합니다.\n\n그 아래에는 현재 게임 플레이 시간이 나옵니다.";
                break;
            case 10:
                descriptionText.text = "마지막으로 <color=red>ESC 또는 일시정지 버튼</color>을 눌러\n소리를 조절하거나 게임을 종료하실 수 있으며\n\n <color=red>물음표 버튼</color>을 누르면 게임설명을\n다시 확인하실 수 있습니다.";
                break;
            case 11:
                descriptionText.text = "그럼 재밌게 즐겨주세요!\n\n감사합니다!";
                break;
        }
    }
    // 씬 이동 버튼
    public void NextButton()
    {
        nextBtn.SetActive(false);
        // 레벨 증가
        Variables.gameLevel++;

        // 레드팀 번호 설정
        int rand = Random.Range(0, 6);
        while (Variables.isSelectTeam[rand])
        {
            rand = Random.Range(0, 6);
        }
        // 다음 상대 설정
        StageRedTeamSetting(rand);

        Time.timeScale = 1;
        // Fade Out
        fadeAc.gameObject.SetActive(true);
        fadeAc.SetTrigger("fadeOut");
        StartCoroutine(LoadScene(1f, "InGame"));
    }
    public void RestartButton()
    {
        resetBtn.SetActive(false);
        isGameLive = false;
        Time.timeScale = 1;
        // Fade Out
        fadeAc.gameObject.SetActive(true);
        fadeAc.SetTrigger("fadeOut");
        StartCoroutine(LoadScene(1f, "InGame"));
    }
    public void ResetButton()
    {
        isGameLive = false;
        Time.timeScale = 1;
        // Fade Out
        fadeAc.gameObject.SetActive(true);
        fadeAc.SetTrigger("fadeOut");
        StartCoroutine(LoadScene(1f, "Game"));
    }
    void StageRedTeamSetting(int teamNum)
    {
        switch (teamNum)
        {
            case 0:
                Variables.teamRedPrefabs = ObjectManager.instance.giHa_prefabs;
                Variables.startRedIdx = 0;
                Variables.groupRedNum = 5;
                break;
            case 1:
                Variables.teamRedPrefabs = ObjectManager.instance.giHa_prefabs;
                Variables.startRedIdx = 10;
                Variables.groupRedNum = 5;
                break;
            case 2:
                Variables.teamRedPrefabs = ObjectManager.instance.juFok_prefabs;
                Variables.startRedIdx = 0;
                Variables.groupRedNum = 6;
                break;
            case 3:
                Variables.teamRedPrefabs = ObjectManager.instance.bakChi_prefabs;
                Variables.startRedIdx = 0;
                Variables.groupRedNum = 6;
                break;
            case 4:
                Variables.teamRedPrefabs = ObjectManager.instance.bakChi_prefabs;
                Variables.startRedIdx = 12;
                Variables.groupRedNum = 5;
                break;
            case 5:
                Variables.teamRedPrefabs = ObjectManager.instance.vBand_prefabs;
                Variables.startRedIdx = 0;
                Variables.groupRedNum = 5;
                break;
        }

        // Team Num
        Variables.teamRedNum = teamNum;
        // Random Team List
        Variables.isSelectTeam[teamNum] = true;
    }
    IEnumerator LoadScene(float time, string sceneName)
    {
        yield return new WaitForSeconds(time);

        Time.timeScale = 1;
        SceneManager.LoadScene(sceneName);
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
                baseBSpriteRen.gameObject.SetActive(false);
                blueDestroyEffect.SetActive(true);
                SoundManager.instance.SfxPlay("Destroy");
                isGameLive = false;
                GameOver("Lose");
            }
            // Alive
            else
            {
                baseBSpriteRen.color = new Color(1, 0.5f, 0.5f);
                StartCoroutine(SpriteWhite(0.1f, baseBSpriteRen, blueHP > 0));
            }

            // Text
            baseBSlider.value = blueHP;
            blueHpText.text = blueHP.ToString();
            blueHpShadowText.text = blueHP.ToString();
        }
        else if (layer == 9)
        {
            redHP -= dmg;

            if (redHP <= 0)
            {
                redHP = 0;
                baseRSpriteRen.gameObject.SetActive(false);
                redDestroyEffect.SetActive(true);
                isGameLive = false;
                GameOver("Win");
            }
            else
            {
                baseRSpriteRen.color = new Color(1, 0.5f, 0.5f);
                StartCoroutine(SpriteWhite(0.1f, baseRSpriteRen, redHP > 0));
            }

            baseRSlider.value = redHP;
            redHpText.text = redHP.ToString();
            redHpShadowText.text = redHP.ToString();
        }
    }
    IEnumerator SpriteWhite(float time, SpriteRenderer spriteRen, bool isLive)
    {
        yield return new WaitForSeconds(time);

        // 0.1초 사이에 게임이 끝나버리면 함수를 실행하면 안됨
        if (isLive)
            spriteRen.color = Color.white;
    }
    void GameOver(string result)
    {
        // 이겼을 경우
        if (result == "Win")
        {
            // Button
            if (Variables.isStage)  // Stage 모드
            {
                if (Variables.gameLevel < 4)
                {
                    // Victory
                    victoryObj.SetActive(true);
                    nextBtn.SetActive(true);
                }
                else
                {
                    // 불꽃 시작
                    onFire = true;
                    // Clear
                    clearObj.SetActive(true);
                    // 클리어
                    Variables.isStageClear[Variables.teamBlueNum] = true;
                    // 저장
                    PlayerPrefs.SetInt("Clear" + Variables.teamBlueNum, 1);

                    resetBtn.SetActive(true);
                }
            }
            else                   // Normal 모드
            {
                victoryObj.SetActive(true);
                resetBtn.SetActive(true);
            }
        }
        // 졌을 경우
        else if (result == "Lose")
        {
            // Defeat
            defeatObj.SetActive(true);

            // Button
            resetBtn.SetActive(true);
        }

        // 게임 작동 중지 (시간, 코스트, 카메라이동, 소환버튼, 유닛클릭)
        isGameLive = false;
        camSpeed = 0;
        controlSet.SetActive(false);
        overSet.SetActive(true);
    }
    // ======================================================= COM 함수
    IEnumerator Pattern(float time, int typeNum)
    {
        yield return new WaitForSeconds(time);

        if (!isUpgradeTime)
        {
            // 해당 typeNum에 해당하는 코스트보다 많이 있을 경우 해당하는 유닛 소환
            if (redCost >= patternCost[typeNum])
                MakeRedUnit(typeNum + Variables.startRedIdx + Variables.groupRedNum);
        }
    }
}
