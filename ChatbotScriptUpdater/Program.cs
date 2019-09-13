using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows.Forms;
using ChatbotScriptUpdater;

namespace ChatbotScriptUpdaterss {
	static class Program {
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main ( string[] args ) {
			Application.EnableVisualStyles ( );
			var StartUpDir = Application.StartupPath;
			Console.WriteLine ( StartUpDir );
			Application.SetCompatibleTextRenderingDefault ( false );
			var mainForm = new MainForm ( );
			Application.Run ( mainForm );
			
			if ( StartUpDir.ToLowerInvariant().StartsWith ( Path.GetTempPath ( ).ToLowerInvariant ( ) ) ) {

				Console.WriteLine ( "Remove Startup Directory" );
				ProcessStartInfo psi = new ProcessStartInfo ( "cmd.exe", String.Format ( "/k {0} & {1} & {2}", "timeout /T 1 /NOBREAK >NUL", "rmdir /s /q \"" + StartUpDir + "\"", "exit" ) );
				psi.UseShellExecute = false;
				psi.CreateNoWindow = true;
				psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;

				Process.Start ( psi );
			}
		}
	}
}
