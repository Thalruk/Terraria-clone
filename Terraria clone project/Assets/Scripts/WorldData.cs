public class WorldData
{
    public string name;
    public int seed;
    public int sizeX;
    public int sizeY;
    public TileData[,] tiles;

    public WorldData(string worldName, int worldSizeX, int worldSizeY, int worldSeed, TileData[,] tiles = null)
    {
        name = worldName;
        sizeX = worldSizeX;
        sizeY = worldSizeY;
        seed = worldSeed;
        if (tiles != null)
        {
            this.tiles = tiles;
        }
        else
        {
            this.tiles = new TileData[worldSizeX, worldSizeY];
        }
    }
}
