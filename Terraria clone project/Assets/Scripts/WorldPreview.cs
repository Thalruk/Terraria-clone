using UnityEngine;
using UnityEngine.UI;

public class WorldPreview : MonoBehaviour
{
    [SerializeField] private RawImage previewImage;

    public void GeneratePreview(WorldData worldData)
    {
        Texture2D texture = new Texture2D(worldData.sizeX, worldData.sizeY);

        texture.filterMode = FilterMode.Point;

        Color[] pixels = new Color[worldData.sizeX * worldData.sizeY];

        for (int y = 0; y < worldData.sizeY; y++)
        {
            for (int x = 0; x < worldData.sizeX; x++)
            {
                int index = y * worldData.sizeX + x;
                TileType type = worldData.tiles[x, y].type;

                pixels[index] = type switch
                {
                    TileType.Air => Color.cyan,
                    TileType.Grass => Color.green,
                    TileType.Dirt => new Color(0.3f, 0.2f, 0.1f),
                    TileType.Stone => Color.gray,
                    _ => Color.black
                };
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();

        previewImage.texture = texture;
    }
}