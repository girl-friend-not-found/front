using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;



public class transparent : MonoBehaviour
{
#if UNITY_STANDALONE_WIN
    [DllImport("user32.dll")]
    static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll")]
    static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

    [DllImport("user32.dll")]
    static extern int SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll")]
    static extern int SetLayeredWindowAttributes(IntPtr hWnd, int crKey, byte bAlpha, uint dwFlags);


    // Windows API declarations
    private const int GWL_STYLE = -16;
    private const uint WS_VISIBLE = 0x10000000;
    private const uint WS_POPUP = 0x80000000;
    private const uint HWND_TOPMOST = 0xFFFFFFFF;
    private const uint SWP_NOMOVE = 0x2;
	const int GWL_EXSTYLE = -20;
	const uint WS_EX_LAYERED = 0x00080000;
	const int LWA_COLORKEY = 1;
#endif

    // Start is called before the first frame update
    void Start()
    {
#if !UNITY_EDITOR && UNITY_STANDALONE_WIN
    int width = Screen.width;
    int height = Screen.height;
    var hWnd = GetActiveWindow();
    SetWindowLong(hWnd, GWL_STYLE, WS_VISIBLE | WS_POPUP);
    SetWindowPos(hWnd, -1/*HWND_TOPMOSTの代わり*/, 0, 0, width, height, SWP_NOMOVE);

    SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED);
    SetLayeredWindowAttributes(hWnd, 0, 0, LWA_COLORKEY);
#endif
    }

    // Update is called once per frame
    void Update()
    {
    if (Input.GetKey(KeyCode.Escape))
    {
        Application.Quit();
    }
        
    }
}
