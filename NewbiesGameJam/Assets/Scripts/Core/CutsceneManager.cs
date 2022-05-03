using UnityEngine;
using Cinemachine;

public class CutsceneManager : MonoBehaviour
{
    [SerializeField] private Boss _boss;
    [SerializeField] private PlayerMovement _player;
    [SerializeField] private CinemachineVirtualCamera _vCam;
    [SerializeField] private CinemachineVirtualCamera _vCamPostFight;
    [SerializeField] private GameObject[] _bossName;
    [SerializeField] private float _shakePower;
    [SerializeField] private GameObject _tiles;
    [SerializeField] private GameObject _bossHealthBar;

    [Header ("Sfx")]
    [SerializeField] private AudioClip _nameAppearSfx;
    [SerializeField] private AudioClip _bossScreamSfx;

    private bool _cutsceneHasPlayed = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_cutsceneHasPlayed) return;

        if (other.name == "Player")
        {
            _cutsceneHasPlayed = true;
            StartCutscene();
        }
    }

    private void StartCutscene()
    {
        DisableMovements();
        _boss.FacePlayer(true);
        _vCam.Follow = _boss.transform;
        _vCam.m_Lens.OrthographicSize = 2f;
        MakeBossNameAppear();
        FunctionTimer.Create(() => _tiles.SetActive(true), 1f);
        FunctionTimer.Create(() => StartFight(), 4f);
    }

    private void StartFight()
    {
        _player.ForceMovementStop = true;
        _boss.CanMove = true;
        _vCam.Follow = _player.transform;
        _vCam.m_Lens.OrthographicSize = 3f;
        AudioManager.Instance.MusicSource.Play();
        _bossHealthBar.SetActive(true);
        for (int i = 0; i < _bossName.Length; i++)
            _bossName[i].SetActive(false);
        //_confiner.m_BoundingShape2D = _newBounds;
        _vCamPostFight.Priority += 2;
        this.gameObject.SetActive(false);
    }

    private void DisableMovements()
    {
        _player.ForceMovementStop = false;
        _boss.CanMove = false;
    }

    private void MakeBossNameAppear()
    {
        FunctionTimer.Create(() => AudioManager.Instance.PlaySound(_bossScreamSfx), 0.5f);
        FunctionTimer.Create(() => _bossName[0].SetActive(true), 1f);
        FunctionTimer.Create(() => iTween.ShakePosition(_bossName[0], Vector3.one * _shakePower, 1f), 1.01f);
        FunctionTimer.Create(() => AudioManager.Instance.PlaySound(_nameAppearSfx), 1.01f);
        FunctionTimer.Create(() => _bossName[1].SetActive(true), 2f);
        FunctionTimer.Create(() => iTween.ShakePosition(_bossName[1], Vector3.one * _shakePower, 1f), 2.01f);
        FunctionTimer.Create(() => AudioManager.Instance.PlaySound(_nameAppearSfx), 2.01f);
        FunctionTimer.Create(() => _bossName[2].SetActive(true), 3f);
        FunctionTimer.Create(() => iTween.ShakePosition(_bossName[2], Vector3.one * _shakePower, 1f), 3.01f);
        FunctionTimer.Create(() => AudioManager.Instance.PlaySound(_nameAppearSfx), 3.01f);
    }
}