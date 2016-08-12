using UnityEngine;
using System;

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
        public BlockType blockType;
    }
}
