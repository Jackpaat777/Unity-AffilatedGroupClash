using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using Image = UnityEngine.UI.Image;

public static class Variables
{
    public static bool isFirstGame = false;
    public static bool isStage = true;
    public static bool[] isStageClear = { false, false, false, false, false, false };
    public static bool[] isSelectTeam = { false, false, false, false, false, false };
    public static int gameLevel = 0;
    public static int teamBlueNum = 0;
    public static int teamRedNum = 0;
    public static int startBlueIdx = 0;
    public static int groupBlueNum = 0;
    public static int startRedIdx = 0;
    public static int groupRedNum = 0;
    public static float bgmVolume = -15f;
    public static float sfxVolume = -15f;
    public static GameObject[] teamBluePrefabs = { };
    public static GameObject[] teamRedPrefabs = { };
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // 싱글톤 패턴 : 인스턴스를 여러번 사용하지 않고 하나의 인스턴스로 사용하기

    [Header("---------------[UI]")]
    public Animator fadeAc;
    public Animator menuSetAc;
    public Animator modeSetAc;
    public Animator bookSetAc;
    public Animator normalSetAc;
    public Animator stageSetAc;
    public Animator levelSetAc;
    public Animator settingSetAc;
    public Animator nButtonGroupAc;
    public Animator sButtonGroupAc;
    public Animator creditAc;
    public GameObject modePanel;
    public GameObject normalSelectPanel;
    public GameObject normalLevelPanel;
    public GameObject stageSelectPanel;
    public Button stageButton;
    public Button normalButton;
    public Sprite[] teamLogoSprite;
    public GameObject[] clearObj;

    [Header("---------------[Normal]")]
    public bool isBlueTeam;
    public bool isRedTeam;
    public bool isSelectLevel;
    public int nTeamSelectIdx;
    public Image nBlueTeamLogo;
    public Image nRedTeamLogo;
    public TextMeshProUGUI nBlueTeamNameText;
    public TextMeshProUGUI nRedTeamNameText;
    public TextMeshProUGUI nSelectButtonText;
    public Button[] nSelectButton;
    public Button[] nLevelButton;
    [Header("---------------[Stage]")]
    public int stageTeamSelectIdx;
    public Image stageTeamLogo;
    public TextMeshProUGUI sBlueTeamNameText;
    public TextMeshProUGUI sSelectButtonText;
    public Button[] sTeamSelectButton;

    [Header("---------------[Book]")]
    public int startIdxInBook;
    public int groupNumInBook;
    public int infoPageIdx;
    public string groupNameInInfo;
    public GameObject lastButtonInBook;
    public GameObject unitInfoSet;
    public Image logoImageInBook;
    public Image infoUnitImage;
    public TextMeshProUGUI infoLogoNameText;
    public TextMeshProUGUI infoUnitNameText;
    public TextMeshProUGUI infoUnitTypeText;
    public TextMeshProUGUI infoCostText;
    public TextMeshProUGUI infoHpText;
    public TextMeshProUGUI infoAtkText;
    public TextMeshProUGUI infoAtsText;
    public TextMeshProUGUI infoRanText;
    public TextMeshProUGUI infoSpdText;
    public TextMeshProUGUI infoSkillText;
    public TextMeshProUGUI infoDetailText;
    public GameObject[] groupPrefabsInBook;
    public Image[] buttonImageInBook;
    public TextMeshProUGUI[] buttonNameInBook;
    public Sprite[] unitPictureSprites;
    public Sprite[] unitLogoSprites;
    float scaleNum;

    void Awake()
    {
        instance = this;

        // Fade In
        fadeAc.SetTrigger("fadeIn");
        StartCoroutine(DisableFade(0.5f));

        // BGM
        SoundManager.instance.BgmPlay("Menu");

        // 업적
        for (int i = 0; i < 6; i++)
        {
            // 불러오기
            Variables.isStageClear[i] = PlayerPrefs.GetInt("Clear" + i) == 1 ? true : false;

            if (Variables.isStageClear[i])
                clearObj[i].SetActive(true);
        }
        // 첫 게임인지 불러오기
        Variables.isFirstGame = PlayerPrefs.GetInt("First") == 1 ? true : false;
    }
    IEnumerator DisableFade(float time)
    {
        yield return new WaitForSeconds(time);

        fadeAc.gameObject.SetActive(false);
    }

    // ======================================================= 인게임 버튼 함수
    public void ClickButton(string panelName)
    {
        switch (panelName)
        {
            case "모드":
                menuSetAc.SetTrigger("oldOut");
                modeSetAc.SetTrigger("newIn");
                break;
            case "도감":
                menuSetAc.SetTrigger("oldOut");
                bookSetAc.SetTrigger("newIn");
                break;
            case "설정":
                menuSetAc.SetTrigger("oldOut");
                settingSetAc.SetTrigger("newIn");
                break;
            case "스테":
                modeSetAc.SetTrigger("oldOut");
                stageSetAc.SetTrigger("newIn");
                normalButton.interactable = false;
                break;
            case "일반":
                modeSetAc.SetTrigger("oldOut");
                normalSetAc.SetTrigger("newIn");
                stageButton.interactable = false;
                break;
            case "레벨":
                TeamSelectButton();
                break;
        }
    }
    public void BackButton(string panelName)
    {
        switch (panelName)
        {
            case "모드":
                menuSetAc.SetTrigger("oldIn");
                modeSetAc.SetTrigger("newOut");
                break;
            case "도감":
                menuSetAc.SetTrigger("oldIn");
                bookSetAc.SetTrigger("newOut");
                break;
            case "설정":
                menuSetAc.SetTrigger("oldIn");
                settingSetAc.SetTrigger("newOut");
                break;
            case "스테":
                StageBackButton();
                break;
            case "일반":
                BackSelectButton();
                break;
            case "레벨":
                BackLevelButton();
                break;
        }
    }
    public void CreditButton()
    {
        creditAc.SetTrigger("doCredit");
    }
    public void SkipButton()
    {
        creditAc.SetTrigger("doOut");
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    // ======================================================= Normal 팀 세팅 함수
    public void TeamSetting(string tmName)
    {
        if (nTeamSelectIdx == 0)
        {
            BlueTeamSetting(tmName);
        }
        else if (nTeamSelectIdx == 1)
        {
            RedTeamSetting(tmName);
        }
    }
    void BlueTeamSetting(string tmName) // Variables에 값을 전달해주는 함수 -> InGame에서 받아온 변수들 사용
    {
        // Disable된 버튼이 있다면 true
        if (isBlueTeam)
            nSelectButton[Variables.teamBlueNum].interactable = true;

        isBlueTeam = true;
        switch (tmName)
        {
            case "지하A":
                // 팀 번호
                Variables.teamBlueNum = 0;
                // 팀 프리펩 가져오기
                Variables.teamBluePrefabs = ObjectManager.instance.giHa_prefabs;
                // 프리펩 시작 값
                Variables.startBlueIdx = 0;
                // 사용할 프리펩 개수
                Variables.groupBlueNum = 5;
                // 텍스트 UI 변경
                nBlueTeamNameText.text = "폴리곤엔젤";
                break;
            case "지하B":
                Variables.teamBlueNum = 1;
                Variables.teamBluePrefabs = ObjectManager.instance.giHa_prefabs;
                Variables.startBlueIdx = 10;
                Variables.groupBlueNum = 5;
                nBlueTeamNameText.text = "woo.a.woo";
                break;
            case "주폭":
                Variables.teamBlueNum = 2;
                Variables.teamBluePrefabs = ObjectManager.instance.juFok_prefabs;
                Variables.startBlueIdx = 0;
                Variables.groupBlueNum = 6;
                nBlueTeamNameText.text = "주폭소년단";
                break;
            case "박취A":
                Variables.teamBlueNum = 3;
                Variables.teamBluePrefabs = ObjectManager.instance.bakChi_prefabs;
                Variables.startBlueIdx = 0;
                Variables.groupBlueNum = 6;
                nBlueTeamNameText.text = "앙리 사루엔링 6세";
                break;
            case "박취B":
                Variables.teamBlueNum = 4;
                Variables.teamBluePrefabs = ObjectManager.instance.bakChi_prefabs;
                Variables.startBlueIdx = 12;
                Variables.groupBlueNum = 5;
                nBlueTeamNameText.text = "결석밴드";
                break;
            case "V급":
                Variables.teamBlueNum = 5;
                Variables.teamBluePrefabs = ObjectManager.instance.vBand_prefabs;
                Variables.startBlueIdx = 0;
                Variables.groupBlueNum = 5;
                nBlueTeamNameText.text = "V급밴드";
                break;
        }

        // Logo Setting
        nBlueTeamLogo.sprite = teamLogoSprite[Variables.teamBlueNum];
        nBlueTeamLogo.SetNativeSize();
        // Button Disable
        nSelectButton[Variables.teamBlueNum].interactable = false;
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
    void RedTeamSetting(string enName)
    {
        // 이미 팀을 선택한 적이 있어서 Disable된 버튼이 있다면 true
        if (isRedTeam)
            nSelectButton[Variables.teamRedNum].interactable = true;

        isRedTeam = true;
        switch (enName)
        {
            case "지하A":
                Variables.teamRedNum = 0;
                Variables.teamRedPrefabs = ObjectManager.instance.giHa_prefabs;
                Variables.startRedIdx = 0;
                Variables.groupRedNum = 5;
                nRedTeamNameText.text = "폴리곤엔젤";
                break;
            case "지하B":
                Variables.teamRedNum = 1;
                Variables.teamRedPrefabs = ObjectManager.instance.giHa_prefabs;
                Variables.startRedIdx = 10;
                Variables.groupRedNum = 5;
                nRedTeamNameText.text = "woo.a.woo";
                break;
            case "주폭":
                Variables.teamRedNum = 2;
                Variables.teamRedPrefabs = ObjectManager.instance.juFok_prefabs;
                Variables.startRedIdx = 0;
                Variables.groupRedNum = 6;
                nRedTeamNameText.text = "주폭소년단";
                break;
            case "박취A":
                Variables.teamRedNum = 3;
                Variables.teamRedPrefabs = ObjectManager.instance.bakChi_prefabs;
                Variables.startRedIdx = 0;
                Variables.groupRedNum = 6;
                nRedTeamNameText.text = "앙리 사루엔링 6세";
                break;
            case "박취B":
                Variables.teamRedNum = 4;
                Variables.teamRedPrefabs = ObjectManager.instance.bakChi_prefabs;
                Variables.startRedIdx = 12;
                Variables.groupRedNum = 5;
                nRedTeamNameText.text = "결석밴드";
                break;
            case "V급":
                Variables.teamRedNum = 5;
                Variables.teamRedPrefabs = ObjectManager.instance.vBand_prefabs;
                Variables.startRedIdx = 0;
                Variables.groupRedNum = 5;
                nRedTeamNameText.text = "V급밴드";
                break;
        }

        // Logo Setting
        nRedTeamLogo.sprite = teamLogoSprite[Variables.teamRedNum];
        nRedTeamLogo.SetNativeSize();
        // Button Disable
        nSelectButton[Variables.teamRedNum].interactable = false;
    }
    void TeamSelectButton()
    {
        // Blue팀 선택 페이지
        if (nTeamSelectIdx == 0 && isBlueTeam)
        {
            nSelectButtonText.text = "오른쪽팀 선택";
            nTeamSelectIdx++;
        }
        // Red팀 선택 페이지
        else if (nTeamSelectIdx == 1 && isRedTeam)
        {
            // 버튼 내리기
            nButtonGroupAc.SetTrigger("btnDown");

            nSelectButtonText.text = "다음 단계";
            nTeamSelectIdx++;
        }
        // 다음 단계로
        else if (nTeamSelectIdx == 2)
        {
            // 양팀 모두 버튼을 눌러야 다음 단계로
            if (isBlueTeam && isRedTeam)
            {
                // Animation
                normalSetAc.SetTrigger("oldOut");
                levelSetAc.SetTrigger("newIn");
            }
        }
    }
    void BackSelectButton()
    {
        // 나가기
        if (nTeamSelectIdx == 0)
        {
            stageButton.interactable = true;
            // Animation
            modeSetAc.SetTrigger("oldIn");
            normalSetAc.SetTrigger("newOut");
            // Blue팀은 선택하지 않은 상태
            isBlueTeam = false;
            // 이미지 UI 초기화
            nBlueTeamLogo.sprite = teamLogoSprite[6];
            nBlueTeamLogo.SetNativeSize();
            // 텍스트 UI 초기화
            nBlueTeamNameText.text = "-";
            // 이미 눌렀던 팀버튼 활성화
            nSelectButton[Variables.teamBlueNum].interactable = true;
        }
        // Blue 팀 선택 페이지
        else if (nTeamSelectIdx == 1)
        {
            isRedTeam = false;
            nRedTeamLogo.sprite = teamLogoSprite[6];
            nRedTeamLogo.SetNativeSize();
            nRedTeamNameText.text = "-";
            nSelectButtonText.text = "왼쪽팀 선택";
            nSelectButton[Variables.teamRedNum].interactable = true;
            nTeamSelectIdx--;
        }
        // Red 팀 선택 페이지
        else if (nTeamSelectIdx == 2)
        {
            // 버튼 올리기
            nButtonGroupAc.SetTrigger("btnUp");

            nSelectButtonText.text = "오른쪽팀 선택";
            nTeamSelectIdx--;
        }
    }

    // ======================================================= Stage 팀 세팅 함수
    public void StageBlueTeamSetting(string tmName)
    {
        // Disable된 버튼이 있다면 true
        if (isBlueTeam)
            sTeamSelectButton[Variables.teamBlueNum].interactable = true;

        isBlueTeam = true;
        switch (tmName)
        {
            case "지하A":
                // 팀 선택 번호 (버튼 비활성화에서 씀)
                Variables.teamBlueNum = 0;
                // 팀 프리펩 가져오기
                Variables.teamBluePrefabs = ObjectManager.instance.giHa_prefabs;
                // 프리펩 시작 값
                Variables.startBlueIdx = 0;
                // 사용할 프리펩 개수
                Variables.groupBlueNum = 5;
                // 텍스트 UI 변경
                sBlueTeamNameText.text = "폴리곤엔젤";
                break;
            case "지하B":
                Variables.teamBlueNum = 1;
                Variables.teamBluePrefabs = ObjectManager.instance.giHa_prefabs;
                Variables.startBlueIdx = 10;
                Variables.groupBlueNum = 5;
                sBlueTeamNameText.text = "woo.a.woo";
                break;
            case "주폭":
                Variables.teamBlueNum = 2;
                Variables.teamBluePrefabs = ObjectManager.instance.juFok_prefabs;
                Variables.startBlueIdx = 0;
                Variables.groupBlueNum = 6;
                sBlueTeamNameText.text = "주폭소년단";
                break;
            case "박취A":
                Variables.teamBlueNum = 3;
                Variables.teamBluePrefabs = ObjectManager.instance.bakChi_prefabs;
                Variables.startBlueIdx = 0;
                Variables.groupBlueNum = 6;
                sBlueTeamNameText.text = "앙리 사루엔링 6세";
                break;
            case "박취B":
                Variables.teamBlueNum = 4;
                Variables.teamBluePrefabs = ObjectManager.instance.bakChi_prefabs;
                Variables.startBlueIdx = 12;
                Variables.groupBlueNum = 5;
                sBlueTeamNameText.text = "결석밴드";
                break;
            case "V급":
                Variables.teamBlueNum = 5;
                Variables.teamBluePrefabs = ObjectManager.instance.vBand_prefabs;
                Variables.startBlueIdx = 0;
                Variables.groupBlueNum = 5;
                sBlueTeamNameText.text = "V급밴드";
                break;
        }

        // Logo Setting
        stageTeamLogo.sprite = teamLogoSprite[Variables.teamBlueNum];
        stageTeamLogo.SetNativeSize();
        // Button Disable
        sTeamSelectButton[Variables.teamBlueNum].interactable = false;
        // Random Team List
        for (int i = 0; i < 6; i++)
        {
            Variables.isSelectTeam[i] = false;
        }
        Variables.isSelectTeam[Variables.teamBlueNum] = true;
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
    public void StageTeamSelectButton()
    {
        // Blue팀 선택 페이지
        if (stageTeamSelectIdx == 0 && isBlueTeam)
        {
            // 버튼 내리기
            sButtonGroupAc.SetTrigger("btnDown");

            sSelectButtonText.text = "게임 시작";
            stageTeamSelectIdx++;
        }
        else if (stageTeamSelectIdx == 1)
        {
            // 스테이지 모드
            Variables.isStage = true;

            // Red팀 세팅 (여기 랜덤으로 설정 -> Blue팀과 안겹치게)
            int rand = Random.Range(0, 6);
            while (Variables.isSelectTeam[rand])
            {
                rand = Random.Range(0, 6);
            }
            // 다음 상대 설정
            StageRedTeamSetting(rand);

            // 레벨 설정
            Variables.gameLevel = 0;

            // Fade Out
            fadeAc.gameObject.SetActive(true);
            fadeAc.SetTrigger("fadeOut");
            // 게임 시작
            StartCoroutine(StartGame(1f));
            
        }
    }
    void StageBackButton()
    {
        // 나가기
        if (stageTeamSelectIdx == 0)
        {
            normalButton.interactable = true;
            // Animation
            modeSetAc.SetTrigger("oldIn");
            stageSetAc.SetTrigger("newOut");
            // Blue팀은 선택하지 않은 상태
            isBlueTeam = false;
            // 이미지 UI 초기화
            stageTeamLogo.sprite = teamLogoSprite[6];
            stageTeamLogo.SetNativeSize();
            // 텍스트 UI 초기화
            sBlueTeamNameText.text = "-";
            // 이미 눌렀던 팀버튼 활성화
            sTeamSelectButton[Variables.teamBlueNum].interactable = true;
        }
        // Blue 팀 선택 페이지
        else if (stageTeamSelectIdx == 1)
        {
            // 버튼 올리기
            sButtonGroupAc.SetTrigger("btnUp");

            sSelectButtonText.text = "팀 선택";
            stageTeamSelectIdx--;
        }
    }

    // ======================================================= 레벨 세팅 함수 
    public void SelectLevelButton(int idx)
    {
        // 모든 버튼 활성화
        for (int i = 0; i < 5; i++)
            nLevelButton[i].interactable = true;

        // 레벨 세팅
        isSelectLevel = true;
        Variables.gameLevel = idx;

        // 해당 인덱스 버튼 비활성화
        nLevelButton[idx].interactable = false;
    }
    void BackLevelButton()
    {
        // Animation
        normalSetAc.SetTrigger("oldIn");
        levelSetAc.SetTrigger("newOut");

        isSelectLevel = false;

        // 모든 버튼 활성화
        for (int i = 0; i < 5; i++)
            nLevelButton[i].interactable = true;
    }
    public void GameStartButton()
    {
        // Level을 선택해야 게임 시작
        if (isSelectLevel)
        {
            // 일반 모드
            Variables.isStage = false;

            // Fade Out
            fadeAc.gameObject.SetActive(true);
            fadeAc.SetTrigger("fadeOut");
            // 게임 시작
            StartCoroutine(StartGame(1f));
        }
    }
    IEnumerator StartGame(float time)
    {
        yield return new WaitForSeconds(time);

        SceneManager.LoadScene("InGame");
    }

    // ======================================================= Book 함수
    public void TeamSelectInBook(string teamName)
    {
        groupNameInInfo = teamName;
        lastButtonInBook.SetActive(false);
        int teamIdx = 0;
        string logoName = "";
        
        switch (teamName)
        {
            case "지하A":
                groupPrefabsInBook = ObjectManager.instance.giHa_prefabs;
                startIdxInBook = 0;
                groupNumInBook = 5;
                teamIdx = 0;
                logoName = "폴리곤엔젤";
                break;
            case "지하B":
                groupPrefabsInBook = ObjectManager.instance.giHa_prefabs;
                startIdxInBook = 10;
                groupNumInBook = 5;
                teamIdx = 1;
                logoName = "우아우";
                break;
            case "주폭":
                groupPrefabsInBook = ObjectManager.instance.juFok_prefabs;
                startIdxInBook = 0;
                groupNumInBook = 6;
                teamIdx = 2;
                logoName = "주폭소년단";
                lastButtonInBook.SetActive(true);
                break;
            case "박취A":
                groupPrefabsInBook = ObjectManager.instance.bakChi_prefabs;
                startIdxInBook = 0;
                groupNumInBook = 6;
                teamIdx = 3;
                logoName = "앙리사루엔링6세";
                lastButtonInBook.SetActive(true);
                break;
            case "박취B":
                groupPrefabsInBook = ObjectManager.instance.bakChi_prefabs;
                startIdxInBook = 12;
                groupNumInBook = 5;
                teamIdx = 4;
                logoName = "결석밴드";
                break;
            case "V급":
                groupPrefabsInBook = ObjectManager.instance.vBand_prefabs;
                startIdxInBook = 0;
                groupNumInBook = 5;
                teamIdx = 5;
                logoName = "V급밴드";
                break;
        }

        // Top UI Setting
        // 로고
        logoImageInBook.sprite = unitLogoSprites[teamIdx];
        infoLogoNameText.text = logoName;

        // Button Setting
        for (int i= startIdxInBook; i< startIdxInBook + groupNumInBook; i++)
        {
            Unit bookUnit = groupPrefabsInBook[i].GetComponent<Unit>();
            SpriteRenderer spriteRen = bookUnit.GetComponent<SpriteRenderer>();
            buttonImageInBook[i - startIdxInBook].sprite = spriteRen.sprite;
            buttonNameInBook[i - startIdxInBook].text = bookUnit.unitName;
        }

    }
    public void UnitInfoButton(int idx)
    {
        infoPageIdx = idx;
        // groupPrefabsInBook은 미리 생성됨
        Unit infoUnit = groupPrefabsInBook[startIdxInBook + infoPageIdx].GetComponent<Unit>();

        // Unit 정보 업데이트
        //infoImage.sprite = spriteRen.sprite;
        infoUnitImage.sprite = UnitImage(idx);
        infoUnitImage.SetNativeSize();
        infoUnitImage.transform.localScale = Vector3.one * scaleNum;
        infoUnitNameText.text = infoUnit.unitName;
        TypeTextSetting(infoUnitTypeText, infoUnit.unitType);
        infoCostText.text = $"코인 : {infoUnit.unitCost}";
        infoHpText.text = $"체력 : {infoUnit.unitMaxHp}";
        infoAtkText.text = $"공격 : {infoUnit.unitAtk}";
        infoAtsText.text = $"공격속도 : {infoUnit.unitAtkSpeed}";
        infoRanText.text = $"공격범위 : {infoUnit.unitRange}";
        infoSpdText.text = $"이동속도 : {infoUnit.unitSpeed * 10}";
        infoSkillText.text = UnitSkillText(infoPageIdx);
        infoDetailText.text = UnitDetailText(infoPageIdx);

        unitInfoSet.SetActive(true);
    }
    Sprite UnitImage(int idx)
    {
        Sprite image = null;

        switch (groupNameInInfo)
        {
            case "지하A":
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
                scaleNum = 0.2f;
                break;
            case "지하B":
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
                scaleNum = 0.2f;
                break;
            case "주폭":
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
                scaleNum = 0.4f;
                break;
            case "박취A":
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
                scaleNum = 0.2f;
                break;
            case "박취B":
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
                scaleNum = 0.2f;
                break;
            case "V급":
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
                scaleNum = 0.4f;
                break;
        }
        return image;
    }
    string UnitSkillText(int idx)
    {
        string skillText = "";

        switch (groupNameInInfo)
        {
            case "지하A":
                switch (idx)
                {
                    case 0:
                        skillText = "일반적인 탱커.";
                        break;
                    case 1:
                        skillText = "공격에 적중당한 적 2초간 공격력 5 감소.\n(중첩불가)";
                        break;
                    case 2:
                        skillText = "HP가 적어질수록 공격속도 증가.\n(최대치 : 0.3)";
                        break;
                    case 3:
                        skillText = "범위 내의 아군 전체에게 공격속도 2배 증가.\n(중첩불가)";
                        break;
                    case 4:
                        skillText = "적 전체에게 피해를 입힘.\n(공격속도 고정)\n(중복 소환 불가)";
                        break;
                }
                break;
            case "지하B":
                switch (idx)
                {
                    case 0:
                        skillText = "일반적인 전사.";
                        break;
                    case 1:
                        skillText = "적에게 피해받을 때마다 적의 공격력의 절반만큼 피해를 되돌려줌.\n(근접 적만 해당)";
                        break;
                    case 2:
                        skillText = "공격에 적중당한 적 2초간 공격속도 2배 감소.\n(중첩불가)";
                        break;
                    case 3:
                        skillText = "범위 내 아군 한명에게 10만큼 힐.";
                        break;
                    case 4:
                        skillText = "적을 킬하면 최대체력 10, 공격력 5, 공격속도 0.1 증가\n(공격속도 최대치 : 0.3)";
                        break;
                }
                break;
            case "주폭":
                switch (idx)
                {
                    case 0:
                        skillText = "일반적인 전사.";
                        break;
                    case 1:
                        skillText = "범위 내 아군 전체에게 이동속도 10 증가.\n(중첩불가)";
                        break;
                    case 2:
                        skillText = "자신이 받는 모든 피격데미지 3 감소.";
                        break;
                    case 3:
                        skillText = "광역 원거리 공격.";
                        break;
                    case 4:
                        skillText = "적이 본인과 근접범위까지 오면 백스탭.";
                        break;
                    case 5:
                        skillText = "본인이 처음 공격하기 전까지 무적.";
                        break;
                }
                break;
            case "박취A":
                switch (idx)
                {
                    case 0:
                        skillText = "일반적인 탱커.";
                        break;
                    case 1:
                        skillText = "광역 근접 공격.";
                        break;
                    case 2:
                        skillText = "공격 시 본인의 공격력만큼 체력 흡혈.";
                        break;
                    case 3:
                        skillText = "3.5초마다 1코인씩 증가.";
                        break;
                    case 4:
                        skillText = "일반적인 원딜.\n(가장 긴 공격범위)";
                        break;
                    case 5:
                        skillText = "일정 범위 내에 적이 있다면 빠르게 적에게 이동 후 광역으로 자폭.";
                        break;
                }
                break;
            case "박취B":
                switch (idx)
                {
                    case 0:
                        skillText = "일반적인 탱커.";
                        break;
                    case 1:
                        skillText = "3번째 공격마다 추가 데미지 +5.";
                        break;
                    case 2:
                        skillText = "공격할 때마다 공격속도 증가.\n(최대 0.3)\n(이동 시 초기화)";
                        break;
                    case 3:
                        skillText = "범위 내 아군 전체에게 공격력 5 증가.\n(중첩불가)";
                        break;
                    case 4:
                        skillText = "공격 시 체력 50이하 적은 확정 킬.";
                        break;
                }
                break;
            case "V급":
                switch (idx)
                {
                    case 0:
                        skillText = "일반적인 전사.";
                        break;
                    case 1:
                        skillText = "공격 시 적을 밀쳐냄.";
                        break;
                    case 2:
                        skillText = "공격에 적중당한 적 2초간 이동속도 4 감소.\n(중첩불가)";
                        break;
                    case 3:
                        skillText = "범위 내 원거리 공격 무효.\n(기지의 공격은 해당하지 않음.)";
                        break;
                    case 4:
                        skillText = "원거리유닛만 공격 가능. 근접유닛은 공격할 수 없음. 피격당해도 멈추지 않음.";
                        break;
                }
                break;
        }

        return skillText;
    }
    string UnitDetailText(int idx)
    {
        string detailText = "";

        switch (groupNameInInfo)
        {
            case "지하A":
                switch (idx)
                {
                    case 0:
                        detailText = "머리에 제트플립이 달린 여성. 종족은 인간이 아니고 핸드폰이라고 한다. 특이사항은 말할때마다 핸드폰 화면이 열리는 모습.";
                        break;
                    case 1:
                        detailText = "지하돌의 취지에 가장 적합한 메인보컬. 특수한 발성덕분에 모두의 이목을 집중시킨다. 실제 하와와 여고생이 맞다고 한다.";
                        break;
                    case 2:
                        detailText = "기획력이 좋고 말솜씨가 뛰어나지만 말투때문에 고모라는 별명으로 불린다. 가끔 폭주하는 모습때문에 주변 사람들이 놀라곤 한다.";
                        break;
                    case 3:
                        detailText = "개성이 강한 지하돌에서 일반인으로 특수하게 합격했다. 모두에게 상냥하며 노력하는 리더의 모습으로 매력을 뽐내고 있다.";
                        break;
                    case 4:
                        detailText = "자칭 마계에서 온 마왕님으로 특이한 말투와 허당끼가 있다. 본인 마음대로 인간들에게 저주를 내릴 수 있다고 한다.";
                        break;
                }
                break;
            case "지하B":
                switch (idx)
                {
                    case 0:
                        detailText = "너드미가 있는 공대여신. 많은 사람들을 만나는 걸 힘들어하며 체력이 좋지 않다. 항상 오브젝트라는 비둘기와 함께 다닌다.";
                        break;
                    case 1:
                        detailText = "띠뜨나라의 띠뜨공주. 그녀만의 억센 발음을 알아듣기 힘든 점이 특징. 가끔 통역이 필요할 정도로 알아듣기 어려울 때가 있다.";
                        break;
                    case 2:
                        detailText = "힘빠지는 공기 100% 창법을 가지고 있지만 할 말은 시원시원하게 하는 성격이다. 의외로 공포면역이 없어 공겜여신으로 불리는 중.";
                        break;
                    case 3:
                        detailText = "대한외국인으로 어눌하지만 한국어를 잘 구사한다. 모두에게 밝은 기운을 불어넣는 치유계열 멤버이며 생각보다 키가 크다.";
                        break;
                    case 4:
                        detailText = "이중인격 얀데레 봇치. 평소에는 조용하고 소심한 모습이지만 돌변하면 매우 무서운 얀데레의 모습을 보이는 특징이 있다.";
                        break;
                }
                break;
            case "주폭":
                switch (idx)
                {
                    case 0:
                        detailText = "주폭소년단의 리더. 정열적이며 활기찬 성격덕분에 팀원들을 조율하거나 여러 활동에 참여하는 모습을 보인다.";
                        break;
                    case 1:
                        detailText = "연하남 계열의 아이돌이지만 생각보다 많이 연하인 것이 특징. 상당히 많은 여성들이 오이쿤을 노리고 있다는 소문이 존재한다.";
                        break;
                    case 2:
                        detailText = "쾌활한 학생회장 속성과 웃을 때 \"윤하하하\"하며 호탕하게 웃는 모습이 특징이다. 의외로 카사노바 기질이 있는 것 같다.";
                        break;
                    case 3:
                        detailText = "몸이 안좋은 병약미소년. 빈혈이 심해서 갑자기 쓰러지기도 하지만 이런 모습덕에 팬들에게서 모성애를 자극시킨다.";
                        break;
                    case 4:
                        detailText = "사연이 많아보이는 남자. 여자가 다가오면 소스라치게 놀라며 도망친다. 썰렁한 개그를 치거나 가끔 이상한 말을 할 때가 있다.";
                        break;
                    case 5:
                        detailText = "버터음색과 자상한 목소리로 많은 여심을 홀리는 모습을 보여준다. 현재는 군대이슈로 인해 잠시 활동을 중단한 상태이다.";
                        break;
                }
                break;
            case "박취A":
                switch (idx)
                {
                    case 0:
                        detailText = "억울한 듯한 말투를 가지고 있으며 눈물이 많은 소녀. 반응이 좋아 놀림받을 때가 많지만 가끔씩 날카로운 일침을 날리기도 한다.";
                        break;
                    case 1:
                        detailText = "평소에는 과묵하며 소심하지만 실제로는 근력과 체력이 매우 강하다. 또한 가면을 쓰면 성격이 거칠어지는 모습을 보인다.";
                        break;
                    case 2:
                        detailText = "미친듯한 하이텐션으로 가끔씩 약 처방이 필요하다고 한다. 상대방을 흡수하고 표절하는 것과 배신의 아이콘이라는 특징이 있다.";
                        break;
                    case 3:
                        detailText = "돈 많은 재벌집 아가씨. 사립학교는 재미가 없어서 친구들을 만들기 위해 전학왔다. 남에게 공감하지 못하는 모습을 자주 보여준다.";
                        break;
                    case 4:
                        detailText = "본명은 베르노아르시온느브리취더루왁. 나라의 공주이며 말끝에 \"노라\"를 붙이는 특이한 말투를 쓴다. 총을 굉장히 잘 쏜다.";
                        break;
                    case 5:
                        detailText = "잔잔하고 안정감있는 목소리에서 운전대를 잡으면 한순간에 폭주하는 반전매력을 가지고 있다. 자타공인 데드 아티스트.";
                        break;
                }
                break;
            case "박취B":
                switch (idx)
                {
                    case 0:
                        detailText = "무려 8년을 유급한 복학생 언니/누님. 연륜이 느껴지는 여유있는 말투와 행동이 특징이다. 항상 체력이 부족해 힘들어한다.";
                        break;
                    case 1:
                        detailText = "지방에서 상경한 시골소녀로 구수한 사투리를 쓴다. 노래실력이 매우 좋으며 자칭 섹시호소인이지만 아무도 인정해주지 않는다.";
                        break;
                    case 2:
                        detailText = "Say-No Crystal(줄여서 크짱)이라는 이름의 기타를 가진 소녀. 귀여움 뒤에는 파워풀한 연주실력을 가지고 있다.";
                        break;
                    case 3:
                        detailText = "봇치처럼 소심하지만 의외로 할말은 하는 성격이다. 인싸처럼 보이고 싶다는 이유로 고글을 착용하고 다닌다.";
                        break;
                    case 4:
                        detailText = "노는걸 좋아하는 인싸 여고생. SNS를 즐겨하며 춤을 좋아한다. 노래도 좋아하는 것 같지만 잘하는 편은 아닌 것 같다.";
                        break;
                }
                break;
            case "V급":
                switch (idx)
                {
                    case 0:
                        detailText = "나긋나긋한 기타리스트. 실용음악과를 나왔으며 뛰어난 실력때문에 V급밴드에 위장취업한 것이 아니냐는 말을 자주 듣는다.";
                        break;
                    case 1:
                        detailText = "캐나다에서 온 드러머. 해외파답게 영어실력 좋다. 호기심이 많으며 어디로 튈지 모르는 매력을 가지고 있다.";
                        break;
                    case 2:
                        detailText = "비충베(비챤의 충실한 베이시스트). 겁도 많고 억까도 많지만 V급밴드에서 비타민같이 활기를 넣어주는 역할을 한다.";
                        break;
                    case 3:
                        detailText = "성장형 초보 피아니스트. 실력파 부원들이 많아 열심히 노력하며 실력을 키우고 있다. 이름덕에 대상현이라고 불리고 있다.";
                        break;
                    case 4:
                        detailText = "V급밴드의 마스코트. 고양이지만 매우 똑똑하며 사람말을 한다는 소문이 있다. 예민한 성격때문에 갑자기 다가가면 도망간다.";
                        break;
                }
                break;
        }

        return detailText;
    }
    public void LeftInfoButton()
    {
        // 인덱스 증가
        infoPageIdx--;
        // 0보다 작아질 경우 그룹의 최대값-1 으로 변경
        infoPageIdx = infoPageIdx < 0 ? groupNumInBook - 1 : infoPageIdx;
        // 정보 업데이트
        UnitInfoButton(infoPageIdx);
    }
    public void RightInfoButton()
    {
        // 인덱스 감소
        infoPageIdx++;
        // 그룹의 최대값-1보다 커질 경우 0 으로 변경
        infoPageIdx = infoPageIdx > groupNumInBook - 1 ? 0 : infoPageIdx;
        // 정보 업데이트
        UnitInfoButton(infoPageIdx);
    }
}
