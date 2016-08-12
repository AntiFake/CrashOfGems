using UnityEngine;

namespace Alchemy.Level
{
    public class BlockFactory : MonoBehaviour
    {
        public GameObject prefabBlock;
        public BlockSprite[] sprites;

        private System.Random _rnd = new System.Random();
        private static BlockFactory _instance;

        public static BlockFactory Instance
        {
            get
            {
                if (_instance == null)
                    _instance = (BlockFactory)FindObjectOfType(typeof(BlockFactory));
                return _instance;
            }
        }

        private void Start()
        {
            _rnd = new System.Random();
        }

        /// <summary>
        /// Создание блока. 
        /// </summary>
        public GameObject CreateBlock(int x, int y, Vector2 pos)
        {
            var block = CreateNewFieldItem(prefabBlock, pos, x, y);
            return block;
        }

        /// <summary>
        /// Генерация поля.
        /// </summary>
        public GameObject[,] GenerateField(int width, int height, float spriteWidth, float spriteHeight)
        {
            var field = new GameObject[height, width];

            for (int x = 0; x < height; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    field[x, y] = CreateBlock(x, y, new Vector2(y * spriteWidth, x * spriteHeight));
                }
            }

            return field;
        }

        #region Служебные функции
        private GameObject CreateNewFieldItem(GameObject prefab, Vector2 pos, int x, int y)
        {
            int spriteNumber = GetRandomSpriteNumber();

            var block = (GameObject)GameObject.Instantiate(prefab, new Vector3(pos.x, pos.y, 0f), Quaternion.identity);
            block.name = string.Format("{0};{1}", x, y);

            SetBlockComponent(block, x, y, sprites[spriteNumber].blockType);
            SetSpriteRendererComponent(block, spriteNumber);

            return block;
        }

        private void SetBlockComponent(GameObject block, int x, int y, BlockType blockType)
        {
            BlockComponent blockComponent = block.GetComponent<BlockComponent>();
            blockComponent.x = x;
            blockComponent.y = y;
            blockComponent.type = blockType;
        }

        private void SetSpriteRendererComponent(GameObject block, int spriteNumber)
        {
            SpriteRenderer sr = block.GetComponent<SpriteRenderer>();
            sr.sprite = sprites[spriteNumber].blockSprite;
        }

        private int GetRandomSpriteNumber()
        {
            return _rnd.Next(0, sprites.Length);
        }
        #endregion
    }
}