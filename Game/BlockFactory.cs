using UnityEngine;
using CrashOfGems.Enums;
using CrashOfGems.Classes;
using CrashOfGems.Components;

public class BlockFactory : MonoBehaviour
{
    public GameObject prefabBomb;
    public GameObject prefabLightning;
    public GameObject prefabMultiplier;
    public GameObject prefabBlock;
    public BlockSprite[] sprites;

    private System.Random rnd;
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


    private void Awake()
    {
        rnd = new System.Random();
    }

    /// <summary>
    /// Создание простого блока. 
    /// </summary>
    public GameObject CreateBlock(int x, int y, Vector2 pos)
    {
        var block = CreateNewFieldItem(prefabBlock, pos, x, y, BonusType.None);
        return block;
    }

    /// <summary>
    /// Создание молнии.
    /// </summary>
    public GameObject CreateLightning(int x, int y, Vector2 pos, int hitLength)
    {
        var lightning = CreateNewFieldItem(prefabLightning, pos, x, y, BonusType.Lightning);
        SetLightningComponent(lightning, hitLength);
        return lightning;
    }

    /// <summary>
    /// Создание множителя.
    /// </summary>
    public GameObject CreateMultiplier(int x, int y, Vector2 pos, int multiplierValue)
    {
        var multiplier = CreateNewFieldItem(prefabMultiplier, pos, x, y, BonusType.Multiplication);
        SetMultiplierComponent(multiplier, multiplierValue);
        return multiplier;
    }

    /// <summary>
    /// Создание бомбы.
    /// </summary>
    public GameObject CreateBomb(int x, int y, Vector2 pos, int explosionRadius)
    {
        var bomb = CreateNewFieldItem(prefabBomb, pos, x, y, BonusType.Bomb);
        SetBombComponent(bomb, explosionRadius);
        return bomb;
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
    private GameObject CreateNewFieldItem(GameObject prefab, Vector2 pos, int x, int y, BonusType bonusType)
    {
        int spriteNumber = GetRandomSpriteNumber();

        var block = (GameObject)GameObject.Instantiate(prefab, new Vector3(pos.x, pos.y, 0f), Quaternion.identity);
        block.name = string.Format("{0};{1}", x, y);

        SetBlockComponent(block, x, y, sprites[spriteNumber].blockType);
        SetSpriteRendererComponent(block, spriteNumber, bonusType);

        return block;
    }

    private void SetBlockComponent(GameObject block, int x, int y, BlockType blockType)
    {
        BlockComponent blockComponent = block.GetComponent<BlockComponent>();
        blockComponent.x = x;
        blockComponent.y = y;
        blockComponent.type = blockType;
    }

    private void SetSpriteRendererComponent(GameObject block, int spriteNumber, BonusType bonusType)
    {
        SpriteRenderer sr = block.GetComponent<SpriteRenderer>();

        switch (bonusType)
        {
            case BonusType.None:
                sr.sprite = sprites[spriteNumber].blockSprite;
                break;
            case BonusType.Bomb:
                sr.sprite = sprites[spriteNumber].bombSprite;
                break;
            case BonusType.Multiplication:
                sr.sprite = sprites[spriteNumber].multiplicationSprite;
                break;
            case BonusType.Lightning:
                sr.sprite = sprites[spriteNumber].lightningSprite;
                break;
        }
    }

    private void SetBombComponent(GameObject block, int radius)
    {
        var bombComponent = block.GetComponent<BombComponent>();
        bombComponent.explosionRadius = radius;
    }

    private void SetLightningComponent(GameObject block, int hitLength)
    {
        var lightningComponent = block.GetComponent<LightningComponent>();
        lightningComponent.hitLength = hitLength;
    }

    private void SetMultiplierComponent(GameObject block, int value)
    {
        var multiplierComponent = block.GetComponent<MultiplierComponent>();
        multiplierComponent.multiplierValue = value;
    }

    private int GetRandomSpriteNumber()
    {
        return rnd.Next(0, sprites.Length);
    }
    #endregion
}
