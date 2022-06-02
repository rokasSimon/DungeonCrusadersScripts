using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace DefaultNamespace
{
    public class HeroSelection
    {
        private static readonly string PrefabDir = Application.dataPath + "/Resources/Heroes/";
        private GameObject _currentHero;
        private readonly Queue<GameObject> _unitQueue = new();

        public HeroSelection()
        {
            LoadHeroes();
        }

        public GameObject LoadNextHero()
        {
            _currentHero = _unitQueue.Dequeue();
            _unitQueue.Enqueue(_currentHero);

            return _currentHero;
        }

        private void LoadHeroes()
        {
            var files = Directory.GetFiles(PrefabDir, "*.prefab");
            foreach (var file in files)
            {
                var fileName = string.Join('/', file.Split('/')[new Range(Index.FromEnd(2), Index.FromEnd(0))]).Replace(".prefab", "");
                Debug.Log(fileName);
                var gameObject = Resources.Load<GameObject>(fileName);

                if (gameObject is null)
                {
                    continue;
                }
                
                if (gameObject.GetComponent<Unit>() is null)
                {
                    Debug.Log($"Unit component doesn't exist in {fileName}");
                    continue;
                }

                _unitQueue.Enqueue(gameObject);
            }

            if (!_unitQueue.Any())
            {
                throw new Exception("No units loaded");
            }
        }
    }
}