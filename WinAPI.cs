/****************************\
 * Написано в SharpDevelop.
 * Автор: F.Phoenix
 * Дата: 14.07.2015 15:25
\****************************/

using System;
using System.Text;
using System.Runtime.InteropServices;

namespace Explorer_Switcher
{
	/// <summary> Функции Windows API. </summary>
	public static class WinAPI
	{
		[DllImport("shell32.dll")]
		public static extern void SHChangeNotify(int eventID, uint flags, IntPtr item1, IntPtr item2);
		
		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
		
		[DllImport("user32.dll")]
		public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
		
		[DllImport("user32.dll", SetLastError = true)]
		public static extern uint PostMessage(IntPtr hWnd, int Msg, uint wParam, int lParam);
	}
}
