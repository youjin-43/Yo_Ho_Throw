using UnityEngine;
using Photon.Pun;

public class AudioManager : MonoBehaviourPun
{
    public static AudioManager Instance;

    [Header("BGM")]
    public AudioClip bgmClip;
    //public float bgmVolume = 1f; // 기본값 설정
    AudioSource bgmPlayer;
    AudioHighPassFilter bgmEffect;

    [Header("SFX")]
    public AudioClip[] sfxClips;
    //public float sfxVolume = 1f; // 기본값 설정
    public int channels;//동시 다발적으로 많은 사운드를 내기 위해 (조절필요)
    AudioSource[] sfxPlayers;
    int channelIndex;//현재 재생중인 채널 인덱스

    public enum Sfx
    {
        UIClick, UIToggle,

        PlayerDead, PlayerKill, PlayerHit, PlayerAttack, PlayerMelee, PlayerWalk, PlayerJump, PlayerDash, PlayerRevive,

        Knife, Block, Bounty, Countdown_Low, Countdown_High, Coin, MeleeKnifeHit, ThrowKnifeHit, ExplosionHit
    }

    void Awake()
    {
        channels = 20;
        Init();

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        PlayBgm(true);
    }

    void Init()
    {
        
        // 배경음 플레이어 초기화
        GameObject bgmObject = new GameObject("BgmPlayer");//코드안에서 오브젝트 만들기 가능. 따옴표 안이 오브젝트 이름
        bgmObject.transform.parent = transform;// 윗줄에서 만든 플레이어를 오디오 매니져 오브젝트에 자식으로 넣음
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;//게임시작시 바로 배경음이 안나오게함. 캐릭 선택 후 나오게 하기 위해.
        bgmPlayer.loop = true;
        bgmPlayer.volume = GameManager.Instance.GetBgmValue();
        bgmPlayer.clip = bgmClip;
        bgmEffect = Camera.main.GetComponent<AudioHighPassFilter>();//오디오 하이패스 컴포넌트가 메인 카메라 오브젝트에 있는데 Camera.main 으로 자동 접근


        // 효과음 플레이어 초기화
        GameObject sfxObject = new GameObject("SfxPlayer");//코드안에서 오브젝트 만들기 가능. 따옴표 안이 오브젝트 이름
        sfxObject.transform.parent = transform;
        sfxPlayers = new AudioSource[channels];//채널 개수만큼 오디오 소스를 만들 것임
        for (int index = 0; index < sfxPlayers.Length; index++)//만들어놓은 채널에 포문으로 클립 파일 넣기
        {
            sfxPlayers[index] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[index].playOnAwake = false;
            sfxPlayers[index].bypassListenerEffects = true;//배경음을필터를 하는걸 패스함. 
            sfxPlayers[index].volume = GameManager.Instance.GetSfxValue();
        }
    }

    public void PlayBgm(bool isPlay) //배경음을 플레이 해줌 // 게임 시작 , 끝 루틴
    {
        if (isPlay)
        {
            Debug.Log("BGM켜짐");
            bgmPlayer.Play();

        }
        else
        {
            bgmPlayer.Stop();
        }

    }

    public void EffectBgm(bool isPlay) // 배경음 필터 // 레벨업 구간
    {
        bgmEffect.enabled = isPlay;

    }


    public void PlaySfx(Sfx sfx)
    {
        switch(sfx)
        {
            case Sfx.Countdown_Low:
            case Sfx.Countdown_High:
                Debug.Log("PlaySFX :" + sfx.ToString());
                break;
        }

        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            //채널 인덱스는 마지막에 플레이된 클립이다
            int loopIndex = (index + channelIndex) % sfxPlayers.Length;//넘어가지않게하기위해 모듈러 사용

            /*
            //여러 소리가 중첩되는 경우 랜덤으로 둘중에 하나 나옴
            int ranIndex = 0;
            if (sfx == Sfx.Hit || sfx == Sfx.Melee)
                ranIndex = Random.Range(0, 2);
            */
            if (sfxPlayers[loopIndex].isPlaying)//재생 되는 효과음이 있다면 넘어감
                continue;
            channelIndex = loopIndex;
            sfxPlayers[loopIndex].clip = sfxClips[(int)sfx];
            sfxPlayers[loopIndex].Play();
            break;
        }
    }

    // BGM 볼륨 설정
    public void SetBgmVolume(float volume)
    {
        bgmPlayer.volume = volume; // BGM의 볼륨을 설정
    }
    public float GetBgmVolume()
    {
        return bgmPlayer.volume;
    }

    // SFX 볼륨 설정
    public void SetSfxVolume(float volume)
    {
        foreach (var sfxPlayer in sfxPlayers)
        {
            sfxPlayer.volume = volume; // 모든 SFX의 볼륨을 설정
        }
    }
    public float GetSfxVolume()
    {
        return sfxPlayers[0].volume;
    }


    [PunRPC]
    public void PlaySfxAtPosition(Sfx sfx, Vector3 position)
    {
        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            //채널 인덱스는 마지막에 플레이된 클립이다
            int loopIndex = (index + channelIndex) % sfxPlayers.Length;//넘어가지않게하기위해 모듈러 사용

            /*
            //여러 소리가 중첩되는 경우 랜덤으로 둘중에 하나 나옴
            int ranIndex = 0;
            if (sfx == Sfx.Hit || sfx == Sfx.Melee)
                ranIndex = Random.Range(0, 2);
            */
            if (sfxPlayers[loopIndex].isPlaying)//재생 되는 효과음이 있다면 넘어감
                continue;
            channelIndex = loopIndex;
            sfxPlayers[loopIndex].clip = sfxClips[(int)sfx];
            sfxPlayers[loopIndex].transform.position = position;
            sfxPlayers[loopIndex].spatialBlend = 1.0f;
            sfxPlayers[loopIndex].Play();
            
            break;

            
        }
    }
}