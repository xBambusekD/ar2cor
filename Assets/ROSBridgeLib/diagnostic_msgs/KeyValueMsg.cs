using System.Collections;
using System.Text;
using SimpleJSON;

/* 
 * @brief ROSBridgeLib
 * @author Michael Jenkin, Robert Codd-Downey, Andrew Speers and Miquel Massot Campos
 */

namespace ROSBridgeLib {
	namespace diagnostic_msgs {
		public class KeyValueMsg : ROSBridgeMsg {
			private string _key;
			private string _value;

            public KeyValueMsg(JSONNode msg) {
				_key = msg["key"];
				_value = msg["value"];
            }
			
			public KeyValueMsg(string key, string value) {
				_key = key;
                _value = value;
			}
			
			public static string GetMessageType() {
				return "diagnostic_msgs/KeyValue";
			}
			
			public string GetKey() {
				return _key;
			}

            public string GetValue() {
                return _value;

            }

            public override string ToString() {
				return "KeyValue [key=\"" + _key + "\"" +
                    ", value=\"" + _value + "\"]";
			}
			
			public override string ToYAMLString() {
				return "{\"key\" : \"" + _key + "\"" + 
                    ", \"value\" : \"" + _value + "\"}";
			}
		}
	}
}