using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugGrid : MonoBehaviour
{
    public float gridSize = 1;
    public int gridResolution = 10;
    public Color gridColor = Color.grey;

    private void OnDrawGizmos()
    {
        Gizmos.color = gridColor;

        for (int i = 0; i < gridResolution + 1; i++)
        {
            float pos = i * gridSize - gridSize * gridResolution / 2;

            // Draw horizontal lines
            Gizmos.DrawLine(new Vector3(-gridSize * gridResolution / 2, pos, 0), new Vector3(gridSize * gridResolution / 2, pos, 0));

            // Draw vertical lines
            Gizmos.DrawLine(new Vector3(pos, -gridSize * gridResolution / 2, 0), new Vector3(pos, gridSize * gridResolution / 2, 0));
        }
    }
}
