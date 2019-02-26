using System.Collections;
using System.Text;
using SimpleJSON;
using System;
using System.Collections.Generic;
using ROSBridgeLib.std_msgs;

/* 
 * @brief ARTable - VUT FIT
 * @author Daniel Bambušek
 */

namespace ROSBridgeLib {
	namespace art_msgs {

        public enum learning_request_goal : UInt16 {
            GET_READY = 0,
            GET_READY_WITHOUT_ROBOT = 1,
            EXECUTE_ITEM = 2,
            DONE = 3,
        }

        public class LearningRequestGoalMsg : ROSBridgeMsg {
            private learning_request_goal _request;

			public LearningRequestGoalMsg(JSONNode msg) {
                _request = (learning_request_goal) UInt16.Parse(msg["request"]);
            }
			
			public LearningRequestGoalMsg(learning_request_goal request) {
                _request = request;
			}

            public static string GetMessageType() {
				return "art_msgs/LearningRequestGoal";
			}
		
            public learning_request_goal GetRequest() {
                return _request;
            }
            

            public override string ToString() {
                return "LearningRequestGoal [request=" + (UInt16) _request + "]";
			}
            
            public override string ToYAMLString() {
                return "{\"request\":" + (UInt16) _request + "}";
            }
		}
	}
}