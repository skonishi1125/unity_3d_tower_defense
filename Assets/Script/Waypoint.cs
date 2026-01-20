using UnityEngine;

public class Waypoint : MonoBehaviour
{
    [SerializeField] private Transform[] waypointPath;

    public int Count => waypointPath != null ? waypointPath.Length : 0;

    private void Awake()
    {
        if (waypointPath == null)
            Debug.LogWarning("WaypointPathがnullです");
    }

    public Transform get(int index)
    {
        if (waypointPath == null || index < 0 || index >= waypointPath.Length)
            return null;

        return waypointPath[index];
    }


}
