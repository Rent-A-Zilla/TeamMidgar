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


    private void PlayRandomTrack(AudioSource source, AudioClip[] tracks, ref int lastTrack, float vol)
    {
        if (tracks.Length == 0) return;

        int newTrack;
        do
        {
            newTrack = Random.Range(0, tracks.Length);
        }
        while (tracks.Length > 1 && newTrack == lastTrack);

        lastTrack = newTrack;

        source.volume = vol;
        source.clip = tracks[newTrack];
        source.Play();
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
        PlayRandomTrack(backgroundSource, backgroundTracks, ref lastBackgroundTrack, backgroundVol);
        currentState = MusicState.Background;
    }

    public void playBattleMusic()
    {
        if (currentState == MusicState.Battle) return;
        PlayRandomTrack(battleSource, battleTracks, ref lastBattleTrack, battleVol);
        currentState = MusicState.Battle;
    }

    public void playNearbyMusic()
    {
        if (currentState == MusicState.NearbyEnemy) return;
        PlayRandomTrack(nearbySource, nearbyTracks, ref lastNearbyTrack, nearbyVol);
        currentState = MusicState.NearbyEnemy;
    }
}
