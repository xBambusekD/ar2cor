using System.Collections;
using System.Text;
using SimpleJSON;
using System;
using System.Collections.Generic;
using ROSBridgeLib.std_msgs;
using ROSBridgeLib.actionlib_msgs;
using ROSBridgeLib.geometry_msgs;

/* 
 * @brief ARTable - VUT FIT
 * @author Daniel Bambušek
 */

namespace ROSBridgeLib {
	namespace tf2_web_republisher_msgs {
		public class TFSubscriptionFeedbackMsg : ROSBridgeMsg {
            private List<TransformStampedMsg> _transforms = new List<TransformStampedMsg>();

			public TFSubscriptionFeedbackMsg(JSONNode msg) {
                foreach (JSONNode item in msg["transforms"].AsArray) {
                    _transforms.Add(new TransformStampedMsg(item));
                }
            }
			
			public TFSubscriptionFeedbackMsg(List<TransformStampedMsg> transforms) {
                _transforms = transforms;
            }

            public static string GetMessageType() {
				return "tf2_web_republisher/TFSubscriptionFeedback";
			}
		
            public List<TransformStampedMsg> GetTransforms() {
                return _transforms;
            }

            public override string ToString() {
                string transfromsString = "[";
                for (int i = 0; i < _transforms.Count; i++) {
                    transfromsString = transfromsString + "\"" + _transforms[i].ToString() + "\"";
                    if (_transforms.Count - i > 1) transfromsString += ",";
                }
                transfromsString += "]";

                return "TFSubscriptionFeedback [transforms=" + transfromsString + "]";
			}
            
            public override string ToYAMLString() {
                string transfromsString = "[";
                for (int i = 0; i < _transforms.Count; i++) {
                    transfromsString = transfromsString + "\"" + _transforms[i].ToYAMLString() + "\"";
                    if (_transforms.Count - i > 1) transfromsString += ",";
                }
                transfromsString += "]";

                return "{\"transforms\":" + transfromsString + "}";
            }
		}
	}
}