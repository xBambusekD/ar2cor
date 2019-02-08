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
		public class LearningRequstActionGoalMsg : ROSBridgeMsg {
            private HeaderMsg _header;
            private GoalIDMsg _goal_id;
            private LearningRequstGoalMsg _goal;

			public LearningRequstActionGoalMsg(JSONNode msg) {
                _header = new HeaderMsg(msg["header"]);
                _goal_id = new GoalIDMsg(msg["goal_id"]);
                _goal = new LearningRequstGoalMsg(msg["goal"]);
            }
			
			public LearningRequstActionGoalMsg(HeaderMsg header, GoalIDMsg goal_id, LearningRequstGoalMsg goal) {
                _header = header;
                _goal_id = goal_id;
                _goal = goal;
			}

            public static string GetMessageType() {
				return "art_msgs/LearningRequstActionGoal";
			}
		
            public HeaderMsg GetHeader() {
                return _header;
            }

            public GoalIDMsg GetGoalID() {
                return _goal_id;
            }

            public LearningRequstGoalMsg GetGoal() {
                return _goal;
            }


            public override string ToString() {
                return "LearningRequstActionGoal [header=" + _header.ToString() +
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