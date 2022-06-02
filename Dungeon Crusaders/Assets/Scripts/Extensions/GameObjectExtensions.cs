using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Extensions
{
    public static class GameObjectExtensions
    {
        public static List<GameObject> GetAllChildrenInGameObject(this GameObject gameObject)
        {
            return (from Transform children in gameObject.transform select children.gameObject).ToList();
        }
    }
}