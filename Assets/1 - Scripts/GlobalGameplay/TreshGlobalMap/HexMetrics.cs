using UnityEngine;

public static class HexMetrics
{
    public const float outerRadius = 10f;

    public const float innerRadius = outerRadius * 0.866f;

    public static Vector2[] corners =
    {
        new Vector2(0, outerRadius),
        new Vector2(innerRadius, outerRadius * 0.5f),
        new Vector2(innerRadius, -outerRadius * 0.5f),
        new Vector2(0, -outerRadius),
        new Vector2(-innerRadius, -outerRadius * 0.5f),
        new Vector2(-innerRadius, outerRadius * 0.5f),
        new Vector2(0, outerRadius)
    };

    public const int partSizeX = 5;
    public const int partSizeY = 5;
}
