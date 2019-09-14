using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatbotScriptUpdater.Extensions {
	public static partial class OrValue {
		public static T Or<T> ( this T s, Func<T, bool> test, T value ) {
			return test ( s ) ? s : value;
		}

		public static String Or ( this String s, String value ) {
			return s.Or ( x => { return !String.IsNullOrWhiteSpace ( x ); }, value );
		}

	}
}
