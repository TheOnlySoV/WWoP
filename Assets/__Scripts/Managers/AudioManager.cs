using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public List<AudioClip> routeThemes;
    public List<AudioClip> wildBattleThemes;
    public List<AudioClip> gymBattleThemes;
    public List<AudioClip> npcBattleThemes;
    public List<AudioClip> pvpBattleThemes;

    public static AudioManager instance;

    private void Awake()
    {
        if (instance)
            Destroy(instance);
        instance = this;
    }

    public AudioClip GetRouteTheme(int index)
    {
        return routeThemes[index];
    }

    public AudioClip GetWildTheme()
    {
        return wildBattleThemes[Random.Range(0, wildBattleThemes.Count)];
    }

    public AudioClip GeNPCTheme()
    {
        return npcBattleThemes[Random.Range(0, npcBattleThemes.Count)];
    }

    public AudioClip GetPVPTheme()
    {
        return pvpBattleThemes[Random.Range(0, pvpBattleThemes.Count)];
    }

    public AudioClip GetGymTheme(int index)
    {
        return gymBattleThemes[index];
    }
}
