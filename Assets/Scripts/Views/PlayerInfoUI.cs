using Photon.Pun;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerInfoUI : MonoBehaviour
{
    #region Fields

    [SerializeField]
    private Vector3 _screenOffset = new Vector3(0f, 1.5f, 0f);

    [SerializeField]
    private TextMeshProUGUI _playerNameText;

    [Tooltip("UI Slider to display Player's Health")]
    [SerializeField]
    private Slider _playerHealthSlider;

    Transform _targetTransform;

    Renderer _targetRenderer;

    [SerializeField] private CanvasGroup _canvasGroup;

    Vector3 _targetPosition;

    private bool _isSet;

	#endregion


	#region UnityMethods

	void Awake()
	{
		if (_canvasGroup == null)
			_canvasGroup = GetComponent<CanvasGroup>();

		transform.SetParent(GameObject.Find("Canvas").transform, false);
	}

    private void Update()
    {
        if (_isSet && _targetTransform == null)
            Destroy(gameObject);
    }

    void LateUpdate()
	{
		if (_targetTransform != null)
		{
			_targetPosition = _targetTransform.position;

			transform.position = Camera.main.WorldToScreenPoint(_targetPosition + _screenOffset);
		}
	}

    #endregion


    #region Methods

	public void SetPlayerName(string playerName)
    {
        _playerNameText.text = playerName;
    }

	public void SetPlayerHealth(int health)
    {
        _playerHealthSlider.value = Mathf.Clamp(health, _playerHealthSlider.minValue, _playerHealthSlider.maxValue);
    }

    public void SetUiEnabled(bool isEnabled)
    {
        _canvasGroup.alpha = isEnabled ? 1f : 0f;
    }

	public void SetTarget(Transform followTarget)
	{
		_targetTransform = followTarget;
        _isSet = true;
	}

	#endregion
}
