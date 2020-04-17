using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ChatbotScriptUpdater.Extensions;
using ChatbotScriptUpdater.Github;

namespace ChatbotScriptUpdater {
	public partial class MainForm : Form {

		public MainForm ( ) {


			Updater = new ApplicationUpdater ( );
			Updater.BeginUpdateCheck += Updater_BeginUpdateCheck;
			Updater.EndUpdateCheck += Updater_EndUpdateCheck;
			Updater.Error += Updater_Error;

			Configuration = new ApplicationUpdater.Configuration {
				Version = "0.0.0",
				Repository = null
			};


			InitializeComponent ( );

			this.statusLabel.Text = "";
		}

		public string UpdateFile { get; set; } = "update.manifest";

		private void Updater_Error ( object sender, System.IO.ErrorEventArgs e ) {
			this.statusLabel.Text = e.GetException ( ).Message;
		}

		private void Updater_EndUpdateCheck ( object sender, ApplicationUpdater.UpdateStatusEventArgs e ) {
			UpdateStatus = e.Status;
			if ( UpdateStatus.HasUpdate ) {
				this.statusLabel.Text = $"Update Available: v{UpdateStatus.LatestVersion}. You are running v{UpdateStatus.UserVersion}";
			} else {
				this.statusLabel.Text = $"You are running the latest version: {UpdateStatus.UserVersion.ToString ( )}";
			}
			updateNow.Enabled = UpdateStatus.HasUpdate;
		}

		private void Updater_BeginUpdateCheck ( object sender, EventArgs e ) {
			this.statusLabel.Text = "Checking for Update...";
			updateNow.Enabled = false;
			progress.Value = 0;

		}

		public ApplicationUpdater Updater { get; set; }
		public UpdateCheck UpdateStatus { get; set; }
		public ApplicationUpdater.Configuration Configuration { get; set; }

		private async void MainForm_Load ( object sender, EventArgs e ) {
			try {
				Configuration = Updater.GetConfiguration ( );
				InitalizeUI ( );
				
				if ( !Updater.HasError ) {
					await Updater.CheckUpdateStatus ( Configuration );
				}
			} catch {

			}
		}

		private void InitalizeUI ( ) {
			if ( !string.IsNullOrWhiteSpace ( Configuration.Name ) ) {
				Text = $"{Text} ({Configuration.Name})";
			}
			this.cancel.Text = Configuration.Interface.CloseButton;
			this.updateNow.Text = Configuration.Interface.DownloadButton;
			this.linkRepo.Text = Configuration.Interface.OpenRepositoryLink;
			if ( !string.IsNullOrWhiteSpace ( Configuration.Repository?.Name ) && !string.IsNullOrWhiteSpace ( Configuration.Repository?.Owner ) ) {
				this.linkRepo.Enabled = true;
			} else {
				this.linkRepo.Enabled = false;
			}
			if ( !string.IsNullOrWhiteSpace ( Configuration.Website ) ) {
				this.website.Text = Configuration.Website;
				this.website.Enabled = true;
			} else {
				this.website.Enabled = false;
			}
		}

		private void DownloadAsset ( ) {

			if ( UpdateStatus == null || UpdateStatus.Asset == null || string.IsNullOrWhiteSpace ( UpdateStatus.Asset.DownloadUrl ) ) {
				return;
			}

			using ( var http = new WebClient ( ) ) {
				var path = Path.GetDirectoryName ( Assembly.GetExecutingAssembly ( ).Location );
				//var tempPath = Path.GetTempPath ( );
				var local = Path.Combine ( path, UpdateStatus.Asset.Name );

				http.DownloadFileCompleted += Http_DownloadFileCompleted;
				http.DownloadProgressChanged += Http_DownloadProgressChanged;
				http.DownloadFile ( new Uri ( UpdateStatus.Asset.DownloadUrl ), local );
			}
		}

		private void MoveDirectory ( string source, string dest ) {
			if ( !Directory.Exists ( dest ) ) {
				Console.WriteLine ( $"Create Directory: {dest}" );
				Directory.CreateDirectory ( dest );
			}
			var files = Directory.GetFiles ( source );
			var directories = Directory.GetDirectories ( source );
			foreach ( string s in files ) {
				Console.WriteLine ( $"Copy File: {s}" );
				File.Copy ( s, Path.Combine ( dest, Path.GetFileName ( s ) ), true );
			}
			foreach ( string d in directories ) {
				MoveDirectory ( Path.Combine ( source, Path.GetFileName ( d ) ), Path.Combine ( dest, Path.GetFileName ( d ) ) );
			}

		}

		private void ExtractAsset ( ) {
			if ( UpdateStatus == null || UpdateStatus.Asset == null || string.IsNullOrWhiteSpace ( UpdateStatus.Asset.DownloadUrl ) ) {
				return;
			}
			var path = Path.GetDirectoryName ( Assembly.GetExecutingAssembly ( ).Location );
			var local = Path.Combine ( path, UpdateStatus.Asset.Name );
			// the path is the path of Medal Overlay. This gets the parent.
			var slcScriptsPath = new DirectoryInfo ( Configuration.Path
			/*@"D:\Development\projects\github\chatbot-medal\MedalRunner\MedalOverlayUpdater\bin\Debug\Scripts\MedalOverlay"*/
			/*@"D:\Data\OBS\apps"*/
			);
			if ( File.Exists ( local ) ) {
				if ( slcScriptsPath.Exists ) {
					var dest = slcScriptsPath.FullName;
					Console.WriteLine ( $"Dest: {dest}" );
					if ( Directory.Exists ( Path.Combine ( path, "temp" ) ) ) {
						Directory.Delete ( Path.Combine ( path, "temp" ), true );
					}
					ZipFile.ExtractToDirectory ( local, Path.Combine ( path, "temp" ) );
					MoveDirectory ( Path.Combine ( path, "temp", Configuration.FolderName ), Path.Combine ( dest, Configuration.FolderName ) );
					if ( Directory.Exists ( Path.Combine ( path, "temp" ) ) ) {
						Directory.Delete ( Path.Combine ( path, "temp" ), true );
					}
				} else {
					throw new DirectoryNotFoundException ( $"Unable to locate directory: {slcScriptsPath}" );
				}
				File.Delete ( local );
			} else {
				throw new FileNotFoundException ( $"Unable to locate file: {local}" );
			}
		}

		private void ShutdownApplicationProcess ( ) {
			ProcessHelper.Stop ( this.Configuration.ProcessName );
		}

		private void RestartApplicationProcess ( ) {
			var processInfo = new System.Diagnostics.ProcessStartInfo ( Configuration.Application );
			processInfo.WorkingDirectory = System.IO.Path.GetDirectoryName ( Configuration.Application );
			var process = new System.Diagnostics.Process ( );
			process.StartInfo = processInfo;
			process.Start ( );
		}

		private void OpenRepository ( ) {
			System.Diagnostics.Process.Start ( $"https://github.com/{Configuration.Repository.Owner}/{Configuration.Repository.Name}" );
		}


		private void Http_DownloadProgressChanged ( object sender, DownloadProgressChangedEventArgs e ) {
			double bytesIn = double.Parse ( e.BytesReceived.ToString ( ) );
			double totalBytes = double.Parse ( e.TotalBytesToReceive.ToString ( ) );
			double percentage = bytesIn / totalBytes * 100;
			progressLabel.Text = "Downloaded " + e.BytesReceived + " of " + e.TotalBytesToReceive;
			progress.Value = int.Parse ( Math.Truncate ( percentage ).ToString ( ) );
		}


		private void Http_DownloadFileCompleted ( object sender, AsyncCompletedEventArgs e ) {
			progressLabel.Text = "Download Completed...";
			progress.Value = 20;
		}

		private void UpdateNow_Click ( object sender, EventArgs e ) {
			this.cancel.Enabled = false;
			var result = MessageBox.Show ( $"This will shutdown {Configuration.ProcessName}, and restart it after update.\n\nDo you want to continue?",
				"Continue?",
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Question );

			if ( result == DialogResult.Yes ) {
				try {
					DownloadAsset ( );
					progress.Value = 15;

					foreach ( var proc in Configuration.Kill ) {
						progressLabel.Text = $"Kill Process {proc}";
						ProcessHelper.Stop ( proc );
						Thread.Sleep ( 500 );
					}

					progress.Value = 30;

					if ( Configuration.RequiresRestart ) {
						progressLabel.Text = "Shutting Down Application";
						ShutdownApplicationProcess ( );

						progressLabel.Text = $"Waiting for {Configuration.Name} to exit ";
						var waitMax = 150;
						var cnt = 0;
						while ( true ) {
							if ( cnt++ > waitMax ) {
								throw new TimeoutException ( $"Timeout while waiting for {Configuration.ProcessName} to exit" );
							}
							System.Diagnostics.Process[] processList = System.Diagnostics.Process.GetProcessesByName ( Configuration.ProcessName );
							if ( processList.Length > 0 ) {
								progressLabel.Text = $"Waiting for {Configuration.ProcessName} to exit {SpinText ( cnt )}";
								Thread.Sleep ( 100 );
							} else {
								break;
							}
						}
					}
					progress.Value = 45;

					progressLabel.Text = "Running Before Extraction Scripts";
					foreach ( var command in Configuration.Execute.Before ) {
						command.WorkingDirectory = ParseWorkingDirectory ( command.WorkingDirectory );
						Updater.RunCommand ( command );
					}

					progress.Value = 60;

					progressLabel.Text = "Extracting Archive";
					ExtractAsset ( );
					progress.Value = 75;


					progressLabel.Text = "Running After Extraction Scripts";
					foreach ( var command in Configuration.Execute.After ) {
						command.WorkingDirectory = ParseWorkingDirectory ( command.WorkingDirectory );
						Updater.RunCommand ( command );
					}

					progress.Value = 90;

					if ( Configuration.RequiresRestart ) {
						progressLabel.Text = $"Restarting {Configuration.ProcessName}";
						RestartApplicationProcess ( );
					}

					progress.Value = 100;

					this.statusLabel.Text = $"Update to {this.UpdateStatus.LatestVersion} Completed Successfully";
					this.progressLabel.Text = "";
					this.updateNow.Enabled = false;
					this.cancel.Enabled = true;

				} catch ( Exception err ) {
					this.cancel.Enabled = true;
					progressLabel.Text = err.Message;
					Console.WriteLine ( err.ToString ( ) );
				}
			}
		}

		private string ParseWorkingDirectory ( string workingDir ) {
			if ( string.IsNullOrWhiteSpace ( workingDir ) ) {
				return Configuration.Path;
			}

			// todo: get properties through reflection...
			return workingDir
				.Replace ( "${PATH}", Configuration.Path )
				.Replace ( "${SCRIPT}", Configuration.FolderName )
				.Replace ( "${FOLDERNAME}", Configuration.FolderName )
				.Replace ( "${VERSION}", Configuration.Version );
		}

		private void Cancel_Click ( object sender, EventArgs e ) {
			this.Close ( );
		}

		private string SpinText ( int interval ) {

			var items = new string[] {
				"⠋", "⠙", "⠹", "⠸", "⠼", "⠴", "⠦", "⠧", "⠇", "⠏"
			};

			var index = Math.Floor ( (decimal)interval / items.Length );
			return items[(int)index];
		}

		private void LinkRepo_Click ( object sender, EventArgs e ) {
			if ( !string.IsNullOrWhiteSpace ( Configuration.Repository?.Name ) && !string.IsNullOrWhiteSpace ( Configuration.Repository?.Owner ) ) {
				OpenRepository ( );
			}
		}

		private void Website_LinkClicked ( object sender, LinkLabelLinkClickedEventArgs e ) {
			if ( !string.IsNullOrWhiteSpace ( Configuration.Website ) ) {
				System.Diagnostics.Process.Start ( Configuration.Website );
			}
		}

		private void Copyright_LinkClicked ( object sender, LinkLabelLinkClickedEventArgs e ) {
			System.Diagnostics.Process.Start ( "https://github.com/camalot/chatbotscriptupdater" );
		}
	}
}
