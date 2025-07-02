#include "WhiteNoise.hlsl"

float3 voronoiNoise(float3 value) 
{
    float3 baseCell = floor(value);

    // First pass: Closest cell
    float minDistToCell = 10.0;
    float3 toClosestCell;
    float3 closestCell;

    for (int x1 = -1; x1 <= 1; x1++) {
        for (int y1 = -1; y1 <= 1; y1++) {
            for (int z1 = -1; z1 <= 1; z1++) {
                float3 cell = baseCell + float3(x1, y1, z1);
                float3 cellPosition = cell + rand3dTo3d(cell);
                float3 toCell = cellPosition - value;
                float distToCell = length(toCell);

                if (distToCell < minDistToCell) {
                    minDistToCell = distToCell;
                    closestCell = cell;
                    toClosestCell = toCell;
                }
            }
        }
    }

    // Second pass: Closest edge
    float minEdgeDistance = 10.0;

    for (int x2 = -1; x2 <= 1; x2++) {
        for (int y2 = -1; y2 <= 1; y2++) {
            for (int z2 = -1; z2 <= 1; z2++) {
                float3 cell = baseCell + float3(x2, y2, z2);
                float3 cellPosition = cell + rand3dTo3d(cell);
                float3 toCell = cellPosition - value;

                float3 diffToClosestCell = abs(closestCell - cell);
                bool isClosestCell = dot(diffToClosestCell, float3(1, 1, 1)) < 0.1;
                if (!isClosestCell) {
                    float3 toCenter = (toClosestCell + toCell) * 0.5;
                    float3 cellDifference = normalize(toCell - toClosestCell);
                    float edgeDistance = dot(toCenter, cellDifference);
                    minEdgeDistance = min(minEdgeDistance, edgeDistance);
                }
            }
        }
    }

    float random = rand3dTo1d(closestCell);
    return float3(minDistToCell, random, minEdgeDistance);
}

void FullVoronoi_float(float3 WorldPos, float CellDensity, out float Center, out float Edge, out float3 Cells) 
{
    float3 value = WorldPos * CellDensity;
    Center = voronoiNoise(value).x;
    Edge = voronoiNoise(value).z;
    Cells = rand1dTo3d(voronoiNoise(value).y);
}
