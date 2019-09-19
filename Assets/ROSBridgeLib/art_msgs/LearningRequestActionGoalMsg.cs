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
		public class LearningRequestActionGoalMsg : ROSBridgeMsg {
            private HeaderMsg _header;
            private GoalIDMsg _goal_id;
            private LearningRequestGoalMsg _goal;

			public LearningRequestActionGoalMsg(JSONNode msg) {
                _header = new HeaderMsg(msg["header"]);
                _goal_id = new GoalIDMsg(msg["goal_id"]);
                _goal = new LearningRequestGoalMsg(msg["goal"]);
            }
			
			public LearningRequestActionGoalMsg(HeaderMsg header, GoalIDMsg goal_id, LearningRequestGoalMsg goal) {
                _header = header;
                _goal_id = goal_id;
                _goal = goal;
			}

            public static string GetMessageType() {
				return "art_msgs/LearningRequestActionGoal";
			}
		
            public HeaderMsg GetHeader() {
                return _header;
            }

            public GoalIDMsg GetGoalID() {
                return _goal_id;
            }

            public LearningRequestGoalMsg GetGoal() {
                return _goal;
            }


            public override string ToString() {
                return "LearningRequestActionGoal [header=" + _header.ToString() +
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