using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatbotScriptUpdater.Exceptions {
	public class UnableToLoadReleaseException : Exception {
		public UnableToLoadReleaseException ( ) : base ( "Unable to load latest release information" ) {
		}
	}

	public class NoMatchingAssetsException : Exception {
		public NoMatchingAssetsException ( ) : base ( "No assets match what is expected." ) {
		}
	}
}
