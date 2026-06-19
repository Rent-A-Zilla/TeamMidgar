using System.Collections;
using UnityEngine;

public class musicManager : MonoBehaviour
{

    [Header("----- Main Menu Music-----")]
    [SerializeField] AudioSource mainMenuSource;
    [SerializeField] AudioClip[] mainMenuTracks;
    [SerializeField] float mainMenuVol;


    [Header("----- Background Music -----")]
    [SerializeField] AudioSource backgroundSource;
    [SerializeField] AudioClip[] backgroundTracks;
    [SerializeField] float backgroundVol;


    [Header("----- Enemy Nearby Music -----")]
    [SerializeField] AudioSource nearbySource;
    [SerializeField] AudioClip[] nearbyTracks;
    [SerializeField] float nearbyVol;

    [Header("----- Battle Music -----")]
    [SerializeField] AudioSource battleSource;
    [SerializeField] AudioClip[] battleTracks;
    [SerializeField] float battleVol;

    public enum MusicState { MainMenu, Background, NearbyEnemy, Battle }
    private MusicState currentState;

    private int lastMainMenuTrack = -1;
    private int lastBackgroundTrack = -1;
    private int lastNearbyTrack = -1;
    private int lastBattleTrack = -1;
    public static musicManager instance;
    Coroutine fadeRoutine;

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void PlayRandomTrack(AudioSource source, AudioClip[] tracks, ref int lastTrack, float vol)
    {
        if (tracks.Length == 0) return;

        stopAllMusic();


        int newTrack;
        do
        {
            newTrack = Random.Range(0, tracks.Length);
        }
        while (tracks.Length > 1 && newTrack == lastTrack);

        lastTrack = newTrack;

        source.Stop();
        source.clip = tracks[newTrack];
        source.volume = 0f;
        source.Play();

        if(fadeRoutine != null) { StopCoroutine(fadeRoutine); }
        fadeRoutine = StartCoroutine(FadeIn(source, vol));
    }
    public void playMenuMusic()
    {
        if (currentState == MusicState.MainMenu) return;
        PlayRandomTrack(mainMenuSource, mainMenuTracks, ref lastMainMenuTrack, mainMenuVol);
        currentState = MusicState.MainMenu;
    }

    public void playBackgroundMusic()
    {
        if (currentState == MusicState.Background) return;
        currentState = MusicState.Background;
        PlayRandomTrack(backgroundSource, backgroundTracks, ref lastBackgroundTrack, backgroundVol);

    }

    public void playBattleMusic()
    {
        if (currentState == MusicState.Battle) return;
        currentState = MusicState.Battle;
        PlayRandomTrack(battleSource, battleTracks, ref lastBattleTrack, battleVol);

    }

    public void playNearbyMusic()
    {
        if (currentState == MusicState.NearbyEnemy) return;
        PlayRandomTrack(nearbySource, nearbyTracks, ref lastNearbyTrack, nearbyVol);
        currentState = MusicState.NearbyEnemy;
    }

    void stopAllMusic()
    {
        mainMenuSource.Stop();
        backgroundSource.Stop();
        nearbySource.Stop();
        battleSource.Stop();

        mainMenuSource.volume = mainMenuVol;
        backgroundSource.volume = backgroundVol;
        nearbySource.volume = nearbyVol;
        battleSource.volume = battleVol;
    }

    IEnumerator FadeIn(AudioSource source, float targetVol)
    {
        source.volume = 0f;
        while(source.volume < targetVol)
        {
            source.volume += Time.deltaTime * targetVol;
            yield return null;
        }
        source.volume = targetVol;
    }
}
