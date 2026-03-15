using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ControllerPlayer : MonoBehaviour
{
    [Header("Movement Control")]
    public float lucXoay = 15f; 
    public float lucNhay = 1200f; 

    [Header("Trick System")]
    [Tooltip("Points awarded for each 360-degree rotation in the air.")]
    public int diemThuongXoay = 2;

    [Header("Collision Settings")]
    public Collider2D hopVaChamDau; 
    [Header("External Components")]
    public ParticleSystem hieuUngTangToc; 
    public ParticleSystem hieuUngVaCham; 
    public GameObject diaHinh; 
    public Audio quanLyAmThanh; 

    [Header("Physics Tuning")]
    public float lucCanXoayMatDat = 5f; 
    public float lucCanXoayTrenKhong = 0.5f; 

    [Header("UI References")]
    public TextMeshProUGUI hienThiDiem; 
    public TextMeshProUGUI hienThiTrangThai; 
    public TextMeshProUGUI hienThiDiemTongKet; 
    public GameObject bangKetQua; 
    public GameObject nutManChoi2; 

    // Biến nội bộ
    private Rigidbody2D _rb2d; 
    private SurfaceEffector2D _boDieuKhienBeMat; 

    private float _dauVaoNgang; 
    private bool _gameDangChay = true; 
    private int _soDiemTiepDat = 0; 
    private bool _dangTrenMatDat => _soDiemTiepDat > 0; 

    // Biến cho hệ thống trick
    private float _doXoayHienTai = 0f; 
    private bool _khungHinhTruocTrenDat = true;

    // Được gọi đầu tiên
    void Awake()
    {
        _rb2d = GetComponent<Rigidbody2D>();

        if (hopVaChamDau == null)
            Debug.LogWarning("Player: Thiếu tham chiếu 'Head Hitbox' (Hop Va Cham Dau) trong Inspector!");

        if (diaHinh == null)
            Debug.LogWarning("Player: Thiếu tham chiếu 'Terrain' (Dia Hinh) trong Inspector!");
        else
            _boDieuKhienBeMat = diaHinh.GetComponent<SurfaceEffector2D>();
    }

    // Được gọi khi bắt đầu
    void Start()
    {
        // Ẩn UI kết quả
        if (bangKetQua != null)
            bangKetQua.SetActive(false);

        if (nutManChoi2 != null)
            nutManChoi2.SetActive(false);

        hieuUngTangToc.Stop();
        _gameDangChay = true;

        // Reset text
        if (hienThiTrangThai != null) hienThiTrangThai.text = "";
        if (hienThiDiemTongKet != null) hienThiDiemTongKet.text = "";
    }

    // Được gọi mỗi khung hình (dùng cho Input)
    void Update()
    {
        if (!_gameDangChay) return; // Nếu game đã kết thúc, không làm gì cả
        ProcessInput();
    }

    // Được gọi theo chu kỳ vật lý (dùng cho Rigidbody)
    void FixedUpdate()
    {
        if (!_gameDangChay) return; // Nếu game đã kết thúc, không làm gì cả
        ProcessPhysics();
        UpdateTrickSystem();
    }

    /// Xử lý tất cả đầu vào (input) của người chơi.
    private void ProcessInput()
    {
        _dauVaoNgang = Input.GetAxis("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space) && _dangTrenMatDat)
            ApplyJumpForce();

        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            ActivateBoost();
        }
    }

    /// Xử lý di chuyển vật lý và xoay.
    private void ProcessPhysics()
    {
        // Áp dụng lực cản xoay khác nhau tùy thuộc vào việc trên mặt đất hay trên không
        _rb2d.angularDamping = _dangTrenMatDat ? lucCanXoayMatDat : lucCanXoayTrenKhong;

        // Áp dụng lực xoay
        if (_dauVaoNgang != 0)
            _rb2d.AddTorque(-_dauVaoNgang * lucXoay);
    }

    /// Theo dõi và tính điểm khi xoay trên không.
    private void UpdateTrickSystem()
    {
        if (!_dangTrenMatDat)
        {
            // Tích lũy độ xoay
            _doXoayHienTai += _rb2d.angularVelocity * Time.fixedDeltaTime;

            // Kiểm tra xem đã xoay đủ 300 độ chưa
            if (Mathf.Abs(_doXoayHienTai) >= 300f)
            {
                AddScore(diemThuongXoay);
                Debug.Log($"Thực hiện trick! +{diemThuongXoay} điểm");
                // Reset bộ đếm sau khi trừ đi 300 độ
                _doXoayHienTai -= 300f * Mathf.Sign(_doXoayHienTai);
            }
        }

        // Nếu vừa mới chạm đất, reset bộ đếm xoay
        if (_dangTrenMatDat && !_khungHinhTruocTrenDat)
            _doXoayHienTai = 0f;

        _khungHinhTruocTrenDat = _dangTrenMatDat;
    }

    // --- XỬ LÝ VA CHẠM ---
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _soDiemTiepDat++;

            // Lấy góc xoay hiện tại (trục Z)
            float rotationZ = transform.eulerAngles.z;

            // Chuẩn hóa góc: ví dụ: lật nếu góc từ 80 độ đến 280 độ
            bool isFlipped = (rotationZ > 80f && rotationZ < 280f);

            if ((collision.otherCollider == hopVaChamDau || isFlipped) && _gameDangChay)
            {
                // Thêm kiểm tra null để an toàn
                if (quanLyAmThanh != null)
                    // Đồng bộ với tên biến mới trong Audio.cs
                    quanLyAmThanh.PlayEffect(quanLyAmThanh.amThanhThatBai);
                if (hieuUngVaCham != null)
                    hieuUngVaCham.Play();

                TriggerGameEnd(false); // false = thua
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _soDiemTiepDat--;
        }
    }

    // Xử lý khi đi qua trigger (Vạch đích, Đồng xu)
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (!_gameDangChay) return;

        if (other.CompareTag("CheckPoint"))
        {
            // Đồng bộ với tên biến mới trong Audio.cs
            quanLyAmThanh.PlayEffect(quanLyAmThanh.amThanhQuaTram);
            TriggerGameEnd(true); // true = thắng
        }

        if (other.CompareTag("Coin"))
        {
            // Đồng bộ với tên biến mới trong Audio.cs
            quanLyAmThanh.PlayEffect(quanLyAmThanh.amThanhDongXu);
            AddScore(1);
            Destroy(other.gameObject);
        }
    }

    private void ApplyJumpForce()
    {
        _rb2d.AddForce(Vector2.up * lucNhay);
    }

    private void AddScore(int amount)
    {
        // Thử chuyển đổi text sang số
        if (int.TryParse(hienThiDiem.text, out int currentScore))
        {
            currentScore += amount;
            hienThiDiem.text = currentScore.ToString();
        }
        else
        {
            // Nếu text không phải là số, bắt đầu từ 0
            hienThiDiem.text = amount.ToString();
        }
    }

    /// Kích hoạt khi game kết thúc (thắng hoặc thua).
    private void TriggerGameEnd(bool didWin)
    {
        _gameDangChay = false;
        DisablePlayerMovement();

        if (hienThiTrangThai != null)
            hienThiTrangThai.text = didWin ? "LEVEL COMPLETE!" : "GAME OVER";

        // Hiển thị bảng kết quả sau một khoảng trễ
        StartCoroutine(DisplayEndGameUI(didWin));
    }

    /// Đóng băng người chơi khi game kết thúc.
    private void DisablePlayerMovement()
    {
        this.enabled = false; // Tắt hàm Update() và FixedUpdate()
        _rb2d.linearVelocity = Vector2.zero;
        _rb2d.angularVelocity = 0f;
        _rb2d.isKinematic = true; // Dừng hoàn toàn vật lý
        hieuUngTangToc.Stop();
    }

    private IEnumerator DisplayEndGameUI(bool playerWon)
    {
        yield return new WaitForSeconds(0.6f);

        if (bangKetQua != null)
            bangKetQua.SetActive(true);

        // Hiển thị điểm số cuối cùng
        if (hienThiDiemTongKet != null && hienThiDiem != null)
        {
            hienThiDiemTongKet.text = "Score: " + hienThiDiem.text;
        }

        // Chỉ hiển thị nút "Level 2" nếu người chơi thắng
        if (nutManChoi2 != null)
            nutManChoi2.SetActive(playerWon);
    }

    /// Bắt đầu Coroutine tăng tốc.
    public void ActivateBoost()
    {
        // Dừng coroutine cũ (nếu có) trước khi bắt đầu cái mới
        StopCoroutine("BoostSequence");
        StartCoroutine("BoostSequence");
    }

    /// Coroutine xử lý logic tăng tốc.
    private IEnumerator BoostSequence()
    {
        if (_boDieuKhienBeMat == null) yield break; // An toàn nếu thiếu tham chiếu

        hieuUngTangToc.Play();
        _boDieuKhienBeMat.speed = 30; // Tăng tốc
        yield return new WaitForSeconds(3f); // Chờ 3 giây
        hieuUngTangToc.Stop();
        _boDieuKhienBeMat.speed = 20; // Trở lại tốc độ thường
    }
}