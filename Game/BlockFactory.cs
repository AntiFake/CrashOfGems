using UnityEngine;
using System.Collections;
using CrashOfGems.Enums;
using CrashOfGems.Classes;
using CrashOfGems.Components;
using CrashOfGems.Management;

public class BlockFactory : MonoBehaviour
{
    public GameObject prefabBomb;
    public GameObject prefabLightning;
    public GameObject prefabMultiplier;
    public GameObject prefabBlock;
    public BlockSprite[] sprites;
    public GameManager gameManager;

    private System.Random rnd;

    private void Awake()
    {
        rnd = new System.Random();
    }

    /// <summary>
    /// Создание простого блока. 
    /// </summary>
    public GameObject CreateBlock(int x, int y, Vector2 pos)
    {
        return CreateNewFieldItem(prefabBlock, pos, x, y, BonusType.None);
    }


    /// <summary>
    /// Создание молнии.
    /// </summary>
    public GameObject CreateLightning(int x, int y, Vector2 pos)
    {
        return CreateNewFieldItem(prefabLightning, pos, x, y, BonusType.Lightning);
    }

    /// <summary>
    /// Создание множителя.
    /// </summary>
    public GameObject CreateMultiplier(int x, int y, Vector2 pos)
    {
        return CreateNewFieldItem(prefabMultiplier, pos, x, y, BonusType.Multiplication);
    }

    /// <summary>
    /// Создание бомбы.
    /// </summary>
    public GameObject CreateBomb(int x, int y, Vector2 pos)
    {
        return CreateNewFieldItem(prefabBomb, pos, x, y, BonusType.Bomb);
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
        var block = (GameObject)GameObject.Instantiate(prefab, new Vector3(pos.x, pos.y, 0f), Quaternion.identity);
        block.name = string.Format("{0};{1}", x, y);
        int spriteNumber = GetRandomSpriteNumber();
        BlockType blockType = sprites[spriteNumber].blockType;

        block = UpdateBlockComponent(block, x, y, blockType);
        block = UpdateSpriteRendererComponent(block, x, y, spriteNumber, bonusType);

        if (bonusType != BonusType.None)
            AddBonusComponent(block, bonusType);

        return block;
    }

    private GameObject UpdateBlockComponent(GameObject block, int x, int y, BlockType blockType)
    {
        BlockComponent blockComponent = block.GetComponent<BlockComponent>();
        blockComponent.x = x;
        blockComponent.y = y;
        blockComponent.type = blockType;
        blockComponent.gameManager = gameManager;

        return block;
    }

    private GameObject UpdateSpriteRendererComponent(GameObject block, int x, int y, int spriteNumber, BonusType bonusType)
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

        return block;
    }

    private GameObject AddBonusComponent(GameObject bonusBlock, BonusType bonusType)
    {
        BonusComponent bonus = bonusBlock.AddComponent<BonusComponent>();
        bonus.type = bonusType;
        bonusBlock.GetComponent<BlockComponent>().bonus = bonus;

        if (bonusType == BonusType.Multiplication)
            bonus.value = 2;

        if (bonusType == BonusType.Lightning)
            bonus.value = 3;

        return bonusBlock;
    }

    private int GetRandomSpriteNumber()
    {
        return rnd.Next(0, sprites.Length);
    }
    #endregion
}
