using UnityEngine;
using System;
using CrashOfGems.Enums;

namespace CrashOfGems.Classes
{
    /// <summary>
    /// Класс, использующийся для сопоставления бонусов.
    /// </summary>
    [Serializable]
    public class BlockSprite
    {
        [SerializeField]
        public Sprite blockSprite;

        [SerializeField]
        public Sprite bombSprite;

        [SerializeField]
        public BlockType blockType;
    }
}
