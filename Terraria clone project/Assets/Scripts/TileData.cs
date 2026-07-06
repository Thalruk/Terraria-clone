public struct TileData
{
    public TileType type;
    public WallType wall;

    public TileData(TileType type, WallType wall)
    {
        this.type = type;
        this.wall = wall;
    }
}
