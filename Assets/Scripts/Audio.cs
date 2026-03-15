using UnityEngine;

public class Audio : MonoBehaviour
{
    [Header("Audio Clips")]
    public AudioClip nhacNen; 
    public AudioClip amThanhDongXu; 
    public AudioClip amThanhQuaTram; 
    public AudioClip amThanhThatBai; 

    [Header("Audio Sources")]
    public AudioSource kenhNhacNen; 
    public AudioSource kenhHieuUng; 

    void Awake()
    {
        // Thêm các bước kiểm tra an toàn khi khởi tạo
        if (kenhNhacNen == null)
        {
            Debug.LogError("Audio: Thiếu 'Background Channel' (Kenh Nhac Nen)!", this);
            return;
        }
        if (kenhHieuUng == null)
        {
            Debug.LogError("Audio: Thiếu 'SFX Channel' (Kenh Hieu Ung)!", this);
            return;
        }

        // Vẫn tự động phát nhạc nền khi bắt đầu
        PlayBackground(nhacNen);
    }

    public void PlayBackground(AudioClip track)
    {
        // Hàm public này giờ sẽ gọi một hàm private (nội bộ)
        InternalSetAndPlayMusic(track);
    }

    public void PlayEffect(AudioClip effect)
    {
        // Hàm public này giờ sẽ gọi một hàm private (nội bộ)
        InternalPlayOneShotEffect(effect);
    }

    private void InternalSetAndPlayMusic(AudioClip musicClip)
    {
        if (musicClip == null)
        {
            Debug.LogWarning("InternalSetAndPlayMusic: Không thể phát nhạc, AudioClip bị null.");
            return;
        }

        if (kenhNhacNen.clip == musicClip && kenhNhacNen.isPlaying)
        {
            // Tối ưu hóa: Nếu nhạc này đang phát rồi thì không làm gì cả
            return;
        }

        kenhNhacNen.clip = musicClip;
        kenhNhacNen.loop = true;
        kenhNhacNen.Play();
    }
    private void InternalPlayOneShotEffect(AudioClip sfxClip)
    {
        if (sfxClip == null)
        {
            Debug.LogWarning("InternalPlayOneShotEffect: Không thể phát SFX, AudioClip bị null.");
            return;
        }

        // Sử dụng PlayOneShot để cho phép nhiều âm thanh phát chồng lên nhau
        kenhHieuUng.PlayOneShot(sfxClip);
    }
}