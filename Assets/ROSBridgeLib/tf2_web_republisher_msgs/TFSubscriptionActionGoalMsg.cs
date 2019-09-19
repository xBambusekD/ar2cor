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
		public class TFSubscriptionActionGoalMsg : ROSBridgeMsg {
            private HeaderMsg _header;
            private GoalIDMsg _goal_id;
            private TFSubscriptionGoalMsg _goal;

			public TFSubscriptionActionGoalMsg(JSONNode msg) {
                _header = new HeaderMsg(msg["header"]);
                _goal_id = new GoalIDMsg(msg["goal_id"]);
                _goal = new TFSubscriptionGoalMsg(msg["goal"]);
            }
			
			public TFSubscriptionActionGoalMsg(HeaderMsg header, GoalIDMsg goal_id, TFSubscriptionGoalMsg goal) {
                _header = header;
                _goal_id = goal_id;
                _goal = goal;
			}

            public static string GetMessageType() {
				return "tf2_web_republisher/TFSubscriptionActionGoal";
			}
		
            public HeaderMsg GetHeader() {
                return _header;
            }

            public GoalIDMsg GetGoalID() {
                return _goal_id;
            }

            public TFSubscriptionGoalMsg GetGoal() {
                return _goal;
            }


            public override string ToString() {
                return "TFSubscriptionActionGoal [header=" + _header.ToString() +
                    ", goal_id=" + _goal_id.ToString() +
                    ", goal=" + _goal.ToString() + "]";
			}
            
            public override string ToYAMLString() {
                return "{\"header\":" + _header.ToYAMLString() +
                    ", \"goal_id\":" + _goal_id.ToYAMLString() +
                    ", \"goal\":" + _goal.ToYAMLString() + "}";
            }
		}
	}
}