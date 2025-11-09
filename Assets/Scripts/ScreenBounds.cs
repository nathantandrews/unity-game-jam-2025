using UnityEngine;

[ExecuteAlways]
public class ScreenBounds : MonoBehaviour
{
    public Camera cam;
    public float wallThickness = 1f;

    void Start()
    {
        if (cam == null)
        {
            cam = Camera.main;
        }
        if (cam == null)
        {
            return;
        }
        UpdateWalls();
    }

    void UpdateWalls()
    {
        float camHeight = 2f * cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        Vector2 camPos = cam.transform.position;
        float halfW = camWidth / 2f;
        float halfH = camHeight / 2f;

        Transform top = transform.Find("TopWall");
        Transform bottom = transform.Find("BottomWall");
        Transform left = transform.Find("LeftWall");
        Transform right = transform.Find("RightWall");

        top.position = new Vector2(camPos.x, camPos.y + halfH + wallThickness / 2f);
        bottom.position = new Vector2(camPos.x, camPos.y - halfH - wallThickness / 2f);
        left.position = new Vector2(camPos.x - halfW - wallThickness / 2f, camPos.y);
        right.position = new Vector2(camPos.x + halfW + wallThickness / 2f, camPos.y);

        top.localScale = new Vector2(camWidth + 2 * wallThickness, wallThickness);
        bottom.localScale = new Vector2(camWidth + 2 * wallThickness, wallThickness);
        left.localScale = new Vector2(wallThickness, camHeight + 2 * wallThickness);
        right.localScale = new Vector2(wallThickness, camHeight + 2 * wallThickness);
    }
}
