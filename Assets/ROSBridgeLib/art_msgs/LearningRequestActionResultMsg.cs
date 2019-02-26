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
	namespace art_msgs {
		public class LearningRequestActionResultMsg : ROSBridgeMsg {
            private HeaderMsg _header;
            private GoalStatusMsg _status;
            private LearningRequstResultMsg _result;

			public LearningRequestActionResultMsg(JSONNode msg) {
                _header = new HeaderMsg(msg["header"]);
                _status = new GoalStatusMsg(msg["status"]);
                _result = new LearningRequstResultMsg(msg["result"]);
            }
			
			public LearningRequestActionResultMsg(HeaderMsg header, GoalStatusMsg status, LearningRequstResultMsg result) {
                _header = header;
                _status = status;
                _result = result;
            }

            public static string GetMessageType() {
				return "art_msgs/LearningRequestActionResult";
			}
		
            public HeaderMsg GetHeader() {
                return _header;
            }

            public GoalStatusMsg GetStatus() {
                return _status;
            }

            public LearningRequstResultMsg GetResult() {
                return _result;
            }

            public override string ToString() {
                return "LearningRequestActionResult [header=" + _header.ToString() +
                    ", status=" + _status.ToString() +
                    ", result=" + _result.ToString() + "]";
			}
            
            public override string ToYAMLString() {
                return "{\"header\":" + _header.ToYAMLString() +
                    ", \"status\":" + _status.ToYAMLString() +
                    ", \"result\":" + _status.ToYAMLString() + "}";
            }
		}
	}
}