using UnityEngine;

public class CameraCircularMotion : MonoBehaviour
{
    public Transform target;  // ステージの中心
    public float radius = 10f; // 円運動の半径
    public float speed = 1f;   // 回転速度
    private float angle = 0f;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    void Update()
    {
        // 角度を更新
        angle += speed * Time.deltaTime;

        // 新しいX,Z座標を計算
        float x = target.position.x + radius * Mathf.Cos(angle);
        float z = target.position.z + radius * Mathf.Sin(angle);

        // カメラの位置を更新
        transform.position = new Vector3(x, transform.position.y, z);

        // カメラを常にターゲットの方向に向ける
        transform.LookAt(target);
    }
}
