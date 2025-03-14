using Photon.Pun;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UI_Crosshair : UI_Base
{
    #region VARIABLES
    private TextMeshProUGUI _outlineText;
    private TextMeshProUGUI _innerText;

    private Ray _ray;
    #endregion





    #region OVERRIDE
    public override void Init()
    {
        _name = name;
    }

    public override void On()
    {
        gameObject.SetActive(true);
    }

    public override void Off()
    {
        gameObject.SetActive(false);
    }

    public override void ResetUI()
    {

    }
    #endregion





    #region BEHAVIOURS
    void Awake()
    {
        _outlineText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        _innerText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        _outlineText.gameObject.SetActive(false);
        _innerText.gameObject.SetActive(false);

    }

    void Update()
    {
        Raycasting();
    }
    #endregion





    #region FUNCTION
    private void Raycasting()
    {
        _ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        if (Physics.Raycast(_ray, out RaycastHit hit, 500f) && hit.collider.CompareTag("Player"))
        {
            PhotonView targetPhotonView = hit.collider.GetComponent<PhotonView>();
            if (targetPhotonView != null)
            {
                string playerName = targetPhotonView.Owner.NickName;

                _outlineText.gameObject.SetActive(true);
                _innerText.gameObject.SetActive(true);

                _outlineText.text = playerName;
                _innerText.text = playerName;
            }
        }
        else
        {
            _outlineText.gameObject.SetActive(false);
            _innerText.gameObject.SetActive(false);
        }
    }
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(_ray.origin, _ray.direction * 100);
    }
}
