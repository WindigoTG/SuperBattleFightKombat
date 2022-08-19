using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogManager : MonoBehaviour
{
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private RectTransform _messagesParent;
    [SerializeField] private LogEntryWidget _logEntryPrefab;

    private List<LogEntryWidget> _logEntries = new List<LogEntryWidget>();

    public void LogMessage(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return;

        var newEntry = Instantiate(_logEntryPrefab);
        newEntry.SetText(message);
        newEntry.transform.SetParent(_messagesParent, false);
        _logEntries.Add(newEntry);

        _scrollRect.normalizedPosition = _scrollRect.normalizedPosition.Change(y: 0f);
    }

    public void ClearLogs()
    {
        foreach (var entry in _logEntries)
            Destroy(entry.gameObject);

        _logEntries.Clear();
    }
}
