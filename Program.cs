/*
 * Created with SharpDevelop 3.
 * User: F. Phoenix
 * Date: 15.01.2012
 * Time: 21:00
 * 
 */
 
using System;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Diagnostics;

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
			
			string keypath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced";
			string keyname = (ext ? "HideFileExt" : "Hidden");
			var key = Registry.CurrentUser.OpenSubKey(keypath);
			int val = (int)key.GetValue(keyname);
			key.Close();
			key = Registry.CurrentUser.CreateSubKey(keypath);
			key.SetValue(keyname, (ext ? 1 : 3) - val);
			key.Close();
		}

	}
}
