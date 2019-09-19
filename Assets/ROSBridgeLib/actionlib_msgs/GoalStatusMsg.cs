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
		public class GoalStatusMsg : ROSBridgeMsg {

            public enum Status : UInt16 {
                PENDING = 0,
                ACTIVE = 1,
                PREEMPTED = 2,
                SUCCEEDED = 3,
                ABORTED = 4,
                REJECTED = 5,
                PREEMPTING = 6,
                RECALLING = 7,
                RECALLED = 8,
                LOST = 9,
            }

            private GoalIDMsg _goal_id;
            private Status _status;
            private string _text;

			public GoalStatusMsg(JSONNode msg) {
                _goal_id = new GoalIDMsg(msg["goal_id"]);
                _status = (Status) UInt16.Parse(msg["status"]);
                _text = msg["text"];
            }
			
			public GoalStatusMsg(GoalIDMsg goal_id, Status status, string text) {
                _goal_id = goal_id;
                _status = status;
                _text = text;
			}

            public static string GetMessageType() {
				return "actionlib_msgs/GoalStatus";
			}
		
            public GoalIDMsg GetGoalID() {
                return _goal_id;
            }
            
            public Status GetStatus() {
                return _status;
            }

            public string GetText() {
                return _text;
            }

            public override string ToString() {
                return "GoalStatus [goal_id=" + _goal_id.ToString() +
                    ", status=" + (UInt16) _status + 
                    ", text=" + _text + "]";
			}
            
            public override string ToYAMLString() {
                return "{\"goal_id\":" + _goal_id.ToYAMLString() +
                    ", \"status\":" + (UInt16) _status +
                    ", \"text\":\"" + _text + "\"}";
            }
		}
	}
}