using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatbotScriptUpdater.Exceptions {
	public class NoConfigurationException: Exception {
		public NoConfigurationException ( ) : base ( "Missing configuration information." ) {

		}
	}
}
