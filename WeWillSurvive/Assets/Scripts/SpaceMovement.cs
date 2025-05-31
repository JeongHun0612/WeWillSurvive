using UnityEngine;

public class SpaceMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float boostForce = 10f;
    public float gravityScale = 0.2f;  // 중력 스케일 조절

    private Rigidbody rb;
    private Vector3 inputDir;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;             // 중력 활성화
        Physics.gravity = new Vector3(0, -9.81f * gravityScale, 0);  // 중력 약하게
        rb.linearDamping = 0.1f;
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        inputDir = transform.forward * v + transform.right * h;

        if (Input.GetKey(KeyCode.Space))
        {
            inputDir += Vector3.up * boostForce;
        }
    }

    void FixedUpdate()
    {
        // 실제 힘은 FixedUpdate에서 가하기
        rb.AddForce(inputDir * moveSpeed, ForceMode.Acceleration);
    }
}

