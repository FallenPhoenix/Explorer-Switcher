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
			bool ok = false;
			foreach (string arg in args)
				switch (arg.ToLower())
				{
					case "-sh": ChangeValue("Hidden", 3); ok = true; break;
					case "-se": ChangeValue("HideFileExt", 1); ok = true; break;
					case "-ue": SHChangeNotify(0x08000000 /*SHCNE_ASSOCCHANGED*/, 0 /*SHCNF_IDLIST*/, IntPtr.Zero, IntPtr.Zero); ok = true; break;
				}
			
			if (ok) RefreshExplorer();
			else MessageBox.Show("Приложение не имеет инферфейса и рассчитано на запуск с параметрами.\n\n" + 
			                     "-sh Переключает отображение скрытых файлов.\n" +
			                     "-se Переключает отображение расширений.\n" +
			                     "-ue Обновляет ассоциации типов файлов (заставляет перечитать из реестра).",
			                     "Explorer Switcher");
		}
		
		
		static void ChangeValue(string keyname, int vsum)
		{
			string keypath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced";
			var key = Registry.CurrentUser.OpenSubKey(keypath);
			int val = (int)key.GetValue(keyname);
			key.Close();
			key = Registry.CurrentUser.CreateSubKey(keypath);
			key.SetValue(keyname, vsum - val);
			key.Close();
		}
		
		static void RefreshExplorer()
		{
			RefreshWindow(IntPtr.Zero, "SHELLDLL_DefView", 0x7103);
		}
		
		static void RefreshWindow(IntPtr parent, string win_class, uint wparam)
		{
			var window = IntPtr.Zero;
			var name = new StringBuilder(128);
			while ((window = FindWindowEx(parent, window, null, null)) != IntPtr.Zero)
			{
				GetClassName(window, name, 128);
				if (name.ToString() == win_class)
					PostMessage(window, 0x0111 /*WM_COMMAND*/, wparam, 0);
				else
					RefreshWindow(window, win_class, wparam);
			}
		}
		
		#region WinAPI-функции
		[DllImport("shell32.dll")]
		public static extern void SHChangeNotify(int eventID, uint flags, IntPtr item1, IntPtr item2);
		
		[DllImport("user32.dll", SetLastError = true)]
		static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
		
		[DllImport("user32.dll")]
		static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
		
		[DllImport("user32.dll", SetLastError = true)]
		static extern uint PostMessage(IntPtr hWnd, int Msg, uint wParam, int lParam);
		#endregion
		
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
	}
}
