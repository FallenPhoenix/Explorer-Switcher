/*
 * Created with SharpDevelop 3.
 * User: F. Phoenix
 * Date: 15.01.2012
 * Time: 21:00
 * 
 */
 
#define UpdateExplorer
 
using System;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text;

namespace Explorer_Switcher
{
	/// <summary> Class with program entry point. </summary>
	internal sealed class Program
	{
		/// <summary> Program entry point. </summary>
		[STAThread]
		private static void Main(string[] args)
		{
			bool ext = (args.Length == 1 && args[0] == "-ext");
			
			#region Реестр
			string keypath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced";
			string keyname = (ext ? "HideFileExt" : "Hidden");
			var key = Registry.CurrentUser.OpenSubKey(keypath);
			int val = (int)key.GetValue(keyname);
			key.Close();
			key = Registry.CurrentUser.CreateSubKey(keypath);
			key.SetValue(keyname, (ext ? 1 : 3) - val);
			key.Close();
			#endregion
			
			#if UpdateExplorer
			RefreshExplorer();
			#endif
		}
		
		#if UpdateExplorer
		static void RefreshWindow(IntPtr parent, string win_class, uint wparam)
		{
			var window = IntPtr.Zero;
			var name = new StringBuilder(128);
			while ((window = FindWindowEx(parent, window, null, null)) != IntPtr.Zero)
			{
				GetClassName(window, name, 128);
				if (name.ToString() == win_class)
					PostMessage(window, WM_COMMAND, wparam, 0);
				else
					RefreshWindow(window, win_class, wparam);
			}
		}
		
		static void RefreshExplorer()
		{
			RefreshWindow(IntPtr.Zero, "SHELLDLL_DefView", 0x7103);
		}
		
		#region Код обновления в HiFiTo
//		static void refreshWindow(HWND parent, TCHAR * winClass, WPARAM wparam){
//			HWND window = NULL;
//			TCHAR name[128];
//			while (window = FindWindowEx(parent, window, NULL, NULL)) {
//				GetClassName(window, name, 128);
//				if (_tcscmp(name, winClass) == 0) {
//					PostMessage(window, WM_COMMAND, wparam, 0);
//				} else
//					refreshWindow(window, winClass, wparam);
//			}
//		}
//
//		/* Refreshes all open Windows Explorer Windows and the desktop. */
//		static void refreshExplorer() {
//			/* Update explorer windows */
//			refreshWindow(NULL, _T("SHELLDLL_DefView"), 0x7103);
//		}
		#endregion
		
		#region WinAPI-функции
		[DllImport("user32.dll", SetLastError = true)]
		static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
		
		[DllImport("user32.dll")]
		static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
		
		[DllImport("user32.dll", SetLastError = true)]
		static extern uint PostMessage(IntPtr hWnd, int Msg, uint wParam, int lParam);
		
		const int WM_COMMAND = 0x0111;
		#endregion
		#endif
	}
}
