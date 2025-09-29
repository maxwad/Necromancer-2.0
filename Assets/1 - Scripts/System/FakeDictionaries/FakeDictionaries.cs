using System;
using UnityEngine;
using Enums;

namespace FakeDictionaries
{
    [Serializable]
    public struct StringIntDictionary
    {
        public string key;
        public int value;
    }

    [Serializable]
    public struct EnumMonoDictionary
    {
        public EnemiesTypes key;
        public MonoBehaviour value;
    }
}
