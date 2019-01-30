using System.Collections;
using System.Text;
using SimpleJSON;
using ROSBridgeLib.geometry_msgs;
using System.Collections.Generic;

/* 
 * @brief ROSBridgeLib
 * @author Daniel Bambusek
 */

namespace ROSBridgeLib {
	namespace tf2_msgs {
		public class TFMessageMsg : ROSBridgeMsg {
            private List<TransformStampedMsg> _transforms = new List<TransformStampedMsg>();
			
			public TFMessageMsg(JSONNode msg) {
                foreach (JSONNode item in msg["transforms"].AsArray) {
                    _transforms.Add(new TransformStampedMsg(item));
                }
            }
			
			public TFMessageMsg(List<TransformStampedMsg> transforms) {
                _transforms = transforms;
            }
			
			public static string GetMessageType() {
				return "tf2_msgs/TFMessage";
			}
			
			public List<TransformStampedMsg> GetTransforms() {
                return _transforms;
            }

            public override string ToString() {
                string transformsString = "[";
                for (int i = 0; i < _transforms.Count; i++) {
                    transformsString = transformsString + _transforms[i].ToString();
                    if (_transforms.Count - i > 1) transformsString += ",";
                }
                transformsString += "]";

                return "TFMessage [transforms=" + transformsString + "]";
			}
			
			public override string ToYAMLString() {
                string transformsString = "[";
                for (int i = 0; i < _transforms.Count; i++) {
                    transformsString = transformsString + _transforms[i].ToYAMLString();
                    if (_transforms.Count - i > 1) transformsString += ",";
                }
                transformsString += "]";

                return "{\"transforms\":" + transformsString + "}";
			}
		}
	}
}