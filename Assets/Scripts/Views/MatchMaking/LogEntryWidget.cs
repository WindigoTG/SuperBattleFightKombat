using UnityEngine;
using TMPro;
using System;

public class LogEntryWidget : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _timestampText;
    [SerializeField] private TextMeshProUGUI _messageText;

    private void Update()
    {
        var size = (transform as RectTransform).sizeDelta;
        size.y = Mathf.Max(_messageText.preferredHeight, _timestampText.preferredHeight);
        (transform as RectTransform).sizeDelta = size;
    }

    public void SetText(string message)
    {
        _messageText.text = message;
        var time = DateTime.Now;
        _timestampText.text = $"{time.Hour}:{time.Minute}";
    }
}
