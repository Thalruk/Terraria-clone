public enum TileType : byte
{
    Air = 0,
    Grass = 1,
    Dirt = 2,
    Stone = 3,
}

public enum WallType : byte
{
    Air = 0,
    DirtWall = 2,
    StoneWall = 3,
}
public static class TileStats
{
    public static byte GetMaxHP(this TileType type)
    {
        return type switch
        {
            TileType.Air => 0,
            TileType.Grass => 1,
            TileType.Dirt => 2,
            TileType.Stone => 4,
            _ => 1
        };
    }
}
