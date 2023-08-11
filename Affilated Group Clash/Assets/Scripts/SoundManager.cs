using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    // 슬라이더를 통한 볼륨조절 변수
    public AudioMixer audioMixer;
    public Slider sliderBGM;
    public Slider sliderSFX;

    public AudioSource bgmPlayer;
    public AudioSource sfxPlayer;
    public AudioClip[] bgmClip;
    public AudioClip[] sfxClip;
    public string bgmName;

    void Awake()
    {
        instance = this;
        Application.targetFrameRate = 60;

        // 불러오기
        //Variables.bgmVolume = PlayerPrefs.GetFloat("Bgm");
        //Variables.sfxVolume = PlayerPrefs.GetFloat("Sfx");
    }

    void Start()
    {
        // 오디오 믹서에 볼륨 값 넣기 (씬 이동 시 필요)
        audioMixer.SetFloat("BGM", Variables.bgmVolume);
        audioMixer.SetFloat("SFX", Variables.sfxVolume);

        // 슬라이더에 볼륨 값 넣기
        sliderBGM.value = Variables.bgmVolume;
        sliderSFX.value = Variables.sfxVolume;
    }

    // 슬라이더를 통한 볼륨조절 함수들
    public void BGMControl()
    {
        Variables.bgmVolume = sliderBGM.value;

        // 현재 -40을 최소값으로 설정해주었으므로 -40이 되버리면 아예 꺼버림(오디오믹서의 소리 최소값은 -80)
        if (Variables.bgmVolume == -40f)
            audioMixer.SetFloat("BGM", -80);
        else
            audioMixer.SetFloat("BGM", Variables.bgmVolume);

        VolumeSave();
    }
    public void SFXControl()
    {
        Variables.sfxVolume = sliderSFX.value;

        if (Variables.sfxVolume == -40f)
            audioMixer.SetFloat("SFX", -80);
        else
            audioMixer.SetFloat("SFX", Variables.sfxVolume);

        VolumeSave();
    }

    public void BgmPlay(string type)
    {
        if (bgmName == type)
            return;

        switch (type)
        {
            case "Menu":
                bgmPlayer.clip = bgmClip[0];
                bgmName = "Menu";
                break;
            case "Game":
                bgmPlayer.clip = bgmClip[1];
                bgmName = "Game";
                break;
            case "Book":
                bgmPlayer.clip = bgmClip[2];
                bgmName = "Book";
                break;
        }

        bgmPlayer.Play();
    }
    public void BgmStop()
    {
        bgmPlayer.Stop();
    }
    public void SfxPlay(string type)
    {
        switch (type)
        {
            case "Buy":
                sfxPlayer.clip = sfxClip[0];
                break;
            case "Sword": // 일반 전사
                sfxPlayer.clip = sfxClip[1];
                break;
            case "Guard": // 일반 탱커
                sfxPlayer.clip = sfxClip[2];
                break;
            case "Archer": // 활
                sfxPlayer.clip = sfxClip[3];
                break;
            case "Gun":
                sfxPlayer.clip = sfxClip[4];
                break;
            case "Crash": // 폭발
                sfxPlayer.clip = sfxClip[5];
                break;
            case "Magic":
                sfxPlayer.clip = sfxClip[6];
                break;
            case "Heal":
                sfxPlayer.clip = sfxClip[7];
                break;
            case "Coin":
                sfxPlayer.clip = sfxClip[8];
                break;
            case "Devil":
                sfxPlayer.clip = sfxClip[9];
                break;
            case "Bomb":
                sfxPlayer.clip = sfxClip[10];
                break;
            case "Berserker":
                sfxPlayer.clip = sfxClip[11];
                break;
            case "Grow":
                sfxPlayer.clip = sfxClip[12];
                break;
            case "Base":
                sfxPlayer.clip = sfxClip[13];
                break;
            case "Upgrade":
                sfxPlayer.clip = sfxClip[14];
                break;
            case "Destroy":
                sfxPlayer.clip = sfxClip[15];
                break;
            case "Button1":
                sfxPlayer.clip = sfxClip[16];
                break;
            case "Button2":
                sfxPlayer.clip = sfxClip[17];
                break;
            case "Back":
                sfxPlayer.clip = sfxClip[18];
                break;
        }

        sfxPlayer.PlayOneShot(sfxPlayer.clip);
    }

    void VolumeSave()
    {
        //// 값 저장
        PlayerPrefs.SetInt("Vol", Variables.isChangeVol ? 1 : 0);
        PlayerPrefs.SetFloat("Bgm", Variables.bgmVolume);
        PlayerPrefs.SetFloat("Sfx", Variables.sfxVolume);
    }
}
