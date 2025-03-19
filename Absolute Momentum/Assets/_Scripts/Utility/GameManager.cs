using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace _Scripts.Utility
{
    public class GameManager : ScriptableSingleton<GameManager>
    {
        public List<GameObject> Players = new List<GameObject>();
    }
}