using UnityEngine;

public class Camera : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("Đối tượng mà camera sẽ bám theo")]
    public Transform doiTuongMucTieu; 

    [Header("Camera Smoothing")]
    [Tooltip("Tốc độ bám theo. Giá trị càng cao, bám theo càng nhanh.")]
    public float doMuotBamTheo = 20f; 

    // Khoảng cách camera duy trì so với mục tiêu
    private Vector3 _khoangCachVoiMucTieu; 

    // Được sử dụng bởi hàm SmoothDamp để theo dõi vận tốc hiện tại
    private Vector3 _vanTocHienTai = Vector3.zero; 

    // Thời gian làm mượt, được tính toán từ doMuotBamTheo
    private float _thoiGianLamMuot;

    // Hàm Awake được gọi trước Start
    void Awake()
    {
        // Kiểm tra xem đã gán đối tượng doiTuongMucTieu trong Inspector chưa
        if (doiTuongMucTieu == null)
        {
            Debug.LogError("Camera: Chưa gán 'Focus Object' (Doi Tuong Muc Tieu) trong Inspector!", this.gameObject);
            this.enabled = false; // Tắt script này nếu không có mục tiêu
            return;
        }

        // Tính toán và lưu lại khoảng cách ban đầu
        _khoangCachVoiMucTieu = transform.position - doiTuongMucTieu.position;
        if (doMuotBamTheo <= 0)
            doMuotBamTheo = 0.1f; // Tránh lỗi chia cho 0

        _thoiGianLamMuot = 1f / doMuotBamTheo;
    }

    void LateUpdate()
    {
        // Nếu không có mục tiêu (ví dụ: mục tiêu bị phá hủy), không làm gì cả
        if (doiTuongMucTieu == null)
            return;

        // Tính toán vị trí camera mong muốn
        Vector3 desiredPosition = doiTuongMucTieu.position + _khoangCachVoiMucTieu;
        transform.position = Vector3.SmoothDamp(
            transform.position,     // Vị trí hiện tại
            desiredPosition,        // Vị trí muốn đến
            ref _vanTocHienTai,     // Biến tham chiếu vận tốc (bắt buộc)
            _thoiGianLamMuot        // Thời gian để đến mục tiêu
        );
    }
}