using System.Collections;
using System.Text;
using SimpleJSON;
using System;
using System.Collections.Generic;
using ROSBridgeLib.std_msgs;
using ROSBridgeLib.actionlib_msgs;

/* 
 * @brief ARTable - VUT FIT
 * @author Daniel Bambušek
 */

namespace ROSBridgeLib {
	namespace tf2_web_republisher_msgs {
		public class TFSubscriptionActionFeedbackMsg : ROSBridgeMsg {
            private HeaderMsg _header;
            private GoalStatusMsg _status;
            private TFSubscriptionFeedbackMsg _feedback;

			public TFSubscriptionActionFeedbackMsg(JSONNode msg) {
                _header = new HeaderMsg(msg["header"]);
                _status = new GoalStatusMsg(msg["status"]);
                _feedback = new TFSubscriptionFeedbackMsg(msg["feedback"]);
            }
			
			public TFSubscriptionActionFeedbackMsg(HeaderMsg header, GoalStatusMsg status, TFSubscriptionFeedbackMsg feedback) {
                _header = header;
                _status = status;
                _feedback = feedback;
			}

            public static string GetMessageType() {
				return "tf2_web_republisher/TFSubscriptionActionFeedback";
			}
		
            public HeaderMsg GetHeader() {
                return _header;
            }

            public GoalStatusMsg GetStatus() {
                return _status;
            }

            public TFSubscriptionFeedbackMsg GetFeedback() {
                return _feedback;
            }


            public override string ToString() {
                return "TFSubscriptionActionFeedback [header=" + _header.ToString() +
                    ", status=" + _status.ToString() +
                    ", feedback=" + _feedback.ToString() + "]";
			}
            
            public override string ToYAMLString() {
                return "{\"header\":" + _header.ToYAMLString() +
                    ", \"status\":" + _status.ToYAMLString() +
                    ", \"feedback\":" + _feedback.ToYAMLString() + "}";
            }
		}
	}
}