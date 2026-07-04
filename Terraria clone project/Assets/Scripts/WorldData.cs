public class WorldData
{
    public string name;
    public int sizeX;
    public int sizeY;
    public TileData[,] tiles;

    public WorldData(string worldName, int worldSizeX, int worldSizeY, TileData[,] tiles = null)
    {
        this.name = worldName;
        this.sizeX = worldSizeX;
        this.sizeY = worldSizeY;
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
