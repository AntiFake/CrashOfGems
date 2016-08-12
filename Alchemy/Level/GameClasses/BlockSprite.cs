using UnityEngine;
using System;
using CrashOfGems.Enums;

namespace Alchemy.Level
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
        public Sprite multiplicationSprite;

        [SerializeField]
        public Sprite lightningSprite;

        [SerializeField]
        public BlockType blockType;
    }
}
