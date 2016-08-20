using Alchemy.Model;
using UnityEngine;
using System.Collections.Generic;

namespace Alchemy.Level
{
    public class BlockFactory : MonoBehaviour
    {
        public GameObject prefabBlock;
        public GameObject substrateItem;

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
        public GameObject CreateBlock(int x, int y, Vector2 pos, GameObject fieldParent)
        {
            var block = CreateNewFieldItem(prefabBlock, pos, x, y, fieldParent);
            return block;
        }

        public GameObject[,] CreateFieldSubstrate(int width, int height, float spriteWidth, float spriteHeight, GameObject substrateParent)
        {
            var substrate = new GameObject[height, width];

            for (int x = 0; x < height; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    substrate[x, y] = (GameObject)Instantiate(substrateItem, new Vector3(y * spriteWidth, x * spriteHeight, 0f), Quaternion.identity);
                    substrate[x, y].transform.SetParent(substrateParent.transform, false);
                }
            }

            return substrate;
        }

        /// <summary>
        /// Генерация поля.
        /// </summary>
        public GameObject[,] GenerateField(int width, int height, float spriteWidth, float spriteHeight, GameObject fieldParent)
        {
            var field = new GameObject[height, width];

            for (int x = 0; x < height; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    field[x, y] = CreateBlock(x, y, new Vector2(y * spriteWidth, x * spriteHeight), fieldParent);
                    field[x, y].transform.SetParent(fieldParent.transform, false);
                }
            }

            return field;
        }

        #region Служебные функции
        private GameObject CreateNewFieldItem(GameObject prefab, Vector2 pos, int x, int y, GameObject fieldParent)
        {
            int spriteNumber = GetRandomSpriteNumber();

            var block = (GameObject) Instantiate(prefab, new Vector3(pos.x, pos.y, 0f), Quaternion.identity);
            block.name = string.Format("{0};{1}", x, y);

            SetBlockComponent(block, x, y, GameManager.Instance.LevelModel.resources[spriteNumber].resourceType);
            SetSpriteRendererComponent(block, spriteNumber);

            block.transform.SetParent(fieldParent.transform, false);

            return block;
        }

        private void SetBlockComponent(GameObject block, int x, int y, ResourceType blockType)
        {
            BlockComponent blockComponent = block.GetComponent<BlockComponent>();
            blockComponent.x = x;
            blockComponent.y = y;
            blockComponent.type = blockType;
        }

        private void SetSpriteRendererComponent(GameObject block, int spriteNumber)
        {
            SpriteRenderer sr = block.GetComponent<SpriteRenderer>();
            sr.sprite = GameManager.Instance.LevelModel.resources[spriteNumber].sprite;
        }

        private double GetRandomNumber(double minimum, double maximum)
        {
            return _rnd.NextDouble() * (maximum - minimum) + minimum;
        }

        private int GetRandomSpriteNumber()
        {
            double rndNumber = GetRandomNumber(0.00, 100.00);

            int i = 0;
            foreach (var item in GameManager.Instance.LevelModel.resources)
            {
                if (item.low_boundary <= rndNumber && item.upper_boundary > rndNumber)
                    return i;
                i++;
            }

            // default.
            return 0;
        }
        #endregion
    }
}