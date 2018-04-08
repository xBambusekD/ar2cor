using System.Collections;
using System.Text;
using SimpleJSON;

/* 
 * @brief ROSBridgeLib
 * @author Daniel Bambušek
 */

namespace ROSBridgeLib {
	namespace std_msgs {
		public class BoolMsg : ROSBridgeMsg {
			private bool _data;
			
			public BoolMsg(JSONNode msg) {
				_data = msg["data"].AsBool;
			}
			
			public BoolMsg(bool data) {
				_data = data;
			}
			
			public static string GetMessageType() {
				return "std_msgs/Bool";
			}
			
			public bool GetData() {
				return _data;
			}
			
			public override string ToString() {
				return "Bool [data=" + _data.ToString().ToLower() + "]";
			}
			
			public override string ToYAMLString() {
				return "{\"data\" : " + _data.ToString().ToLower() + "}";
			}
		}
	}
}