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
		public class TFSubscriptionGoalMsg : ROSBridgeMsg {
            private List<string> _source_frames = new List<string>();
            private string _target_frame;
            private float _angular_thres;
            private float _trans_thres;
            private float _rate;

			public TFSubscriptionGoalMsg(JSONNode msg) {
                foreach (JSONNode item in msg["source_frames"].AsArray) {
                    _source_frames.Add(item);
                }
                _target_frame = msg["target_frame"];
                _angular_thres = float.Parse(msg["angular_thres"]);
                _trans_thres = float.Parse(msg["trans_thres"]);
                _rate = float.Parse(msg["rate"]);
            }
			
			public TFSubscriptionGoalMsg(List<string> source_frames, string target_frame, float angular_thres, float trans_thres, float rate) {
                _source_frames = source_frames;
                _target_frame = target_frame;
                _angular_thres = angular_thres;
                _trans_thres = trans_thres;
                _rate = rate;
            }

            public static string GetMessageType() {
				return "tf2_web_republisher/TFSubscriptionGoal";
			}
		
            public List<string> GetSourceFrames() {
                return _source_frames;
            }

            public string GetTargetFrame() {
                return _target_frame;
            }

            public float GetAngularThres() {
                return _angular_thres;
            }

            public float GetTransThres() {
                return _trans_thres;
            }

            public float GetRate() {
                return _rate;
            }


            public override string ToString() {
                string sourceFramesString = "[";
                for (int i = 0; i < _source_frames.Count; i++) {
                    sourceFramesString = sourceFramesString + "\"" + _source_frames[i] + "\"";
                    if (_source_frames.Count - i > 1) sourceFramesString += ",";
                }
                sourceFramesString += "]";

                return "TFSubscriptionGoal [source_frames=" + sourceFramesString +
                    ", target_frame=\"" + _target_frame + "\"" +
                    ", angular_thres=" + _angular_thres.ToString() +
                    ", trans_thres=" + _trans_thres.ToString() +
                    ", rate=" + _rate.ToString() + "]";
			}
            
            public override string ToYAMLString() {
                string sourceFramesString = "[";
                for (int i = 0; i < _source_frames.Count; i++) {
                    sourceFramesString = sourceFramesString + "\"" + _source_frames[i] + "\"";
                    if (_source_frames.Count - i > 1) sourceFramesString += ",";
                }
                sourceFramesString += "]";

                return "{\"source_frames\":" + sourceFramesString +
                    ", \"target_frame\":\"" + _target_frame + "\"" +
                    ", \"angular_thres\":" + _angular_thres.ToString() +
                    ", \"trans_thres\":" + _trans_thres.ToString() +
                    ", \"rate\":" + _rate.ToString() + "}";
            }
		}
	}
}