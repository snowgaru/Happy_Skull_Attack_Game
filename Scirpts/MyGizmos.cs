using UnityEngine;

public class MyGizmos : MonoBehaviour
{
    public Color gizmoColor = Color.yellow;
    public float radius = 0.1f;

    private void OnDrawGizmos()
    {
        // ���� ����
        Gizmos.color = gizmoColor;

        // ��ü ����� ����
        Gizmos.DrawSphere(transform.position, radius);
    }
}
