/****************************\
 * Написано в SharpDevelop.
 * Автор: F.Phoenix
 * Дата: 15.01.2012 21:00
\****************************/
 
using System;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Diagnostics;
using System.Text;

namespace Explorer_Switcher
{
	/// <summary> Класс с точкой входа. </summary>
	internal sealed class Program
	{
		/// <summary> Точка входа. </summary>
		[STAThread]
		private static void Main(string[] args)
		{
			bool ok = false;
			foreach (string arg in args)
				switch (arg.ToLower())
				{
					case "-sh": ChangeValue("Hidden", 3); ok = true; break;
					case "-se": ChangeValue("HideFileExt", 1); ok = true; break;
					case "-ue": WinAPI.SHChangeNotify(0x08000000 /*SHCNE_ASSOCCHANGED*/, 0 /*SHCNF_IDLIST*/, IntPtr.Zero, IntPtr.Zero); ok = true; break;
				}
			
			if (ok) RefreshExplorer();
			else MessageBox.Show("Приложение не имеет инферфейса и рассчитано на запуск с параметрами.\n\n" + 
			                     "-sh Переключает отображение скрытых файлов.\n" +
			                     "-se Переключает отображение расширений.\n" +
			                     "-ue Обновляет ассоциации типов файлов (заставляет перечитать из реестра).",
			                     "Explorer Switcher");
		}
		
		/// <summary> Переключает значение указанного параметра проводника в реестре. </summary>
		/// <param name="keyname"> Имя параметра. </param>
		/// <param name="vsum"> Сумма значений. Например, при vsum=1 значение будет переключаться между 0 и 1, а при vsum=3 - между 1 и 2. </param>
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
		
		/// <summary> Обновляет рабочий стол и все открытые окна Проводника. </summary>
		static void RefreshExplorer()
		{
			RefreshWindow(IntPtr.Zero, "SHELLDLL_DefView", 0x7103);
		}
		
		static void RefreshWindow(IntPtr parent, string win_class, uint wparam)
		{
			var window = IntPtr.Zero;
			var name = new StringBuilder(128);
			while ((window = WinAPI.FindWindowEx(parent, window, null, null)) != IntPtr.Zero)
			{
				WinAPI.GetClassName(window, name, 128);
				if (name.ToString() == win_class)
					WinAPI.PostMessage(window, 0x0111 /*WM_COMMAND*/, wparam, 0);
				else
					RefreshWindow(window, win_class, wparam);
			}
		}
	}
}
