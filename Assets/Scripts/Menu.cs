using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuGame : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject bangTamDung; 

    private bool _daTamDung = false; 

    // Hàm Start để đảm bảo trạng thái ban đầu
    void Start()
    {
        // Đảm bảo bangTamDung bị tắt khi bắt đầu scene (nếu nó được gán)
        if (bangTamDung != null)
        {
            bangTamDung.SetActive(false);
        }
        _daTamDung = false;
        // Đảm bảo thời gian chạy bình thường khi bắt đầu scene
        RestoreGameSpeed();
    }

    void Update()
    {
        // Nhấn ESC để bật/tắt Pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Gọi hàm xử lý bật/tắt mới
            TogglePauseState();
        }
    }

    public void LoadLevel2() { LoadSceneByIndex(3); }
    public void BatDau() { LoadSceneByIndex(2); }
    public void Thoat() { Debug.Log("Exiting application..."); Application.Quit(); }
    public void QuayLaiMenu() { LoadSceneByIndex(0); }
    public void TaiLai() { LoadSceneByIndex(SceneManager.GetActiveScene().buildIndex); }
    public void HuongDan() { LoadSceneByIndex(1); }
    // --- ---

    public void TiepTuc()
    {
        // Gọi hàm private helper mới
        SetPauseState(false);
    }

    public void TamDung()
    {
        // Gọi hàm private helper mới
        SetPauseState(true);
    }

    private void LoadSceneByIndex(int buildIndex)
    {
        RestoreGameSpeed();
        SceneManager.LoadScene(buildIndex);
    }

    private void RestoreGameSpeed()
    {
        Time.timeScale = 1f;
    }

    private void TogglePauseState()
    {
        // Đảo ngược trạng thái hiện tại
        SetPauseState(!_daTamDung);
    }

    private void SetPauseState(bool pause)
    {
        _daTamDung = pause;

        if (bangTamDung != null)
        {
            bangTamDung.SetActive(_daTamDung);
        }

        if (_daTamDung)
        {
            // Dừng game
            Time.timeScale = 0f;
        }
        else
        {
            // Tiếp tục game
            RestoreGameSpeed();
        }
    }
}