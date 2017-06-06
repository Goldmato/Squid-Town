using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class JailFloor : MonoBehaviour
{
    public float Radius { get { return m_Radius; } }

    private Mesh m_Mesh;

    private float m_Radius;

    void Awake()
    {
        m_Mesh = GetComponent<MeshFilter>().mesh;

        m_Radius = Mathf.Min(m_Mesh.bounds.extents.x, m_Mesh.bounds.extents.z);
    }

    public Vector3 RandomLocation()
    {
        var randomCoordinate = Random.insideUnitCircle * m_Radius;

        float randomX = transform.position.x + randomCoordinate.x;
        float randomZ = transform.position.z + randomCoordinate.y;

        var randomLocation = new Vector3(randomX, transform.position.y, randomZ);

        return randomLocation;
    }
}
