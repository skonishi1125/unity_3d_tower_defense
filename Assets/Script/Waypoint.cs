using UnityEngine;

public class Waypoint : MonoBehaviour
{
    [SerializeField] public Transform[] WaypointPath;

    public int Count => WaypointPath != null ? WaypointPath.Length : 0;

    private void Awake()
    {
        if (WaypointPath == null)
            Debug.LogWarning("WaypointPathがnullです");
    }

    public Transform get(int index)
    {
        if (WaypointPath == null || index < 0 || index >= WaypointPath.Length)
            return null;

        return WaypointPath[index];
    }

    private void OnDrawGizmos()
    {
        float radius = 0.2f;

        if (WaypointPath == null || WaypointPath.Length == 0) return;

        Gizmos.color = Color.red;
        for (int i = 0; i < WaypointPath.Length; i++)
        {
            if (WaypointPath[i] == null) continue;

            Gizmos.DrawSphere(WaypointPath[i].position, radius);

            if (i < WaypointPath.Length - 1 && WaypointPath[i + 1] != null)
                Gizmos.DrawLine(WaypointPath[i].position, WaypointPath[i + 1].position);
        }
    }

}
