
using UnityEngine;

public class JPScrollText : MonoBehaviour
{
    private JPFontDrawer fontDrawer;

    private string curMsg;
    public float WriteInterval;
    private float writeTimer;
    private int writeCount;

    public delegate void OnFinishedWritingFunc();
    public event OnFinishedWritingFunc OnFinishedWriting;

    protected virtual void Start()
    {
        fontDrawer = GetComponent<JPFontDrawer>();
        writeTimer = WriteInterval;
        writeCount = 0;
        fontDrawer.SetCharCount(0);
    }

    private void Update()
    {
        if (curMsg == null) return;
        
        writeTimer -= Time.deltaTime;
        if (!(writeTimer <= 0)) return;
        writeTimer = WriteInterval;
        writeCount++;
        // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
        fontDrawer.SetCharCount(writeCount);

        if (writeCount == curMsg.Length && OnFinishedWriting != null)
        {
            OnFinishedWriting();
        }
    }

    public void SetMessage(string message)
    {
        if(!fontDrawer)
            fontDrawer = GetComponent<JPFontDrawer>();
        
        curMsg = message;
        fontDrawer.SetMessage(message);
        fontDrawer.SetCharCount(0);
        writeCount = 0;
        writeTimer = WriteInterval;
    }
}
