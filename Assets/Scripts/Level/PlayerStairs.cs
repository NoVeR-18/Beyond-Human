using UnityEngine;

public class PlayerStairs : MonoBehaviour
{
    private bool isClimbing = false;
    private Vector2 climbDir;
    private float climbSpeed;
    private float zStart, zEnd;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void StartClimbing(Vector2 direction, float speed, float zStart, float zEnd)
    {
        isClimbing = true;
        climbDir = direction;
        climbSpeed = speed;
        this.zStart = zStart;
        this.zEnd = zEnd;
    }

    private void Update()
    {
        if (isClimbing)
        {
            // �������� �� ����������� ��������
            Vector3 move = (Vector3)(climbDir * climbSpeed * Time.deltaTime);
            transform.position += move;

            // ���������� �������� �� ��������
            float progress = Mathf.InverseLerp(zStart, zEnd, transform.position.y);
            float z = Mathf.Lerp(zStart, zEnd, progress);
            transform.position = new Vector3(transform.position.x, transform.position.y, z);

            // ���������� �� ������
            spriteRenderer.sortingOrder = Mathf.RoundToInt(z * 10);
        }
    }

    internal void StopClimbing()
    {
        isClimbing = false;
        climbDir = Vector2.zero;
        climbSpeed = 0;
    }
}
