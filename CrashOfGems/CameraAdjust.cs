using UnityEngine;
using CrashOfGems.Game;

namespace CrashOfGems.Cam
{
    /// <summary>
    /// Настройка камеры.
    /// </summary>
    public static class CameraAdjust
    {
        public static void FitCamera(Camera camera, GameField field, Sprite blockSprite)
        {
            Vector3 pos1 = field.Field[0, 0].transform.position;
            Vector3 pos2 = field.Field[field.FieldHeight - 1, field.FieldWidth - 1].transform.position;
            Vector3 center = (pos2 - pos1) / 2;

            camera.transform.position = new Vector3(center.x, center.y - blockSprite.bounds.size.y, -2f);
        }
    }
}
