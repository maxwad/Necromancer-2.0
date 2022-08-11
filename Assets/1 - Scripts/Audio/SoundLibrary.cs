using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundLibrary : MonoBehaviour
{
    public static SoundLibrary instance;
    public AudioClip levelTheme;

    // collection of single specific sounds
    [System.Serializable]
    public class SoundSingle
    {
        public string soundId;
        public AudioClip sound;
    }

    public SoundSingle[] soundSingles;

    private Dictionary<string, AudioClip> singleDictionary = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = this;

        foreach (SoundSingle single in soundSingles)
            singleDictionary.Add(single.soundId, single.sound);
    }

    public AudioClip GetSpecificClipFromName(string name)
    {
        if (singleDictionary.ContainsKey(name)) 
            return singleDictionary[name];

        return null;
    }
}
