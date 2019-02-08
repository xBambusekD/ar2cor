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
	namespace actionlib_msgs {
		public class GoalIDMsg : ROSBridgeMsg {
            private TimeMsg _stamp;
            string _id;

			public GoalIDMsg(JSONNode msg) {
                _stamp = new TimeMsg(msg["stamp"]);
                _id = msg["id"];
            }
			
			public GoalIDMsg(TimeMsg stamp, string id) {
                _stamp = stamp;
                _id = id;
			}

            public static string GetMessageType() {
				return "actionlib_msgs/GoalID";
			}
		
            public TimeMsg GetStamp() {
                return _stamp;
            }
            
            public string GetID() {
                return _id;
            }

            public override string ToString() {
                return "GoalID [stamp=" + _stamp.ToString() +
                    ", id=" + _id + "]";
			}
            
            public override string ToYAMLString() {
                return "{\"stamp\":" + _stamp.ToYAMLString() +
                    ", \"id\":\"" + _id + "\"}";
            }
		}
	}
}