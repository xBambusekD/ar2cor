using System.Collections;
using System.Text;
using SimpleJSON;
using System;
using System.Collections.Generic;
using UnityEngine;
using ROSBridgeLib.geometry_msgs;
using ROSBridgeLib.diagnostic_msgs;

/* 
* @brief ARTable - VUT FIT
* @author Daniel Bambušek
*/

namespace ROSBridgeLib {
    namespace art_msgs {
        public enum program_type : UInt16 {
            GET_READY = 0,
            NOP = 1,
            WAIT_FOR_USER = 50,
            WAIT_UNTIL_USER_FINISHES = 51,
            PICK_FROM_POLYGON = 100,
            PICK_FROM_FEEDER = 101,
            PICK_OBJECT_ID = 102,
            PICK_TOOL = 103,
            PLACE_TO_POSE = 200,
            PLACE_TO_GRID = 201,
            PLACE_TOOL = 202,
            PATH_THROUGH_POINTS = 300,
            PATH_THROUGH_TRAJECTORY = 301,
            WELDING_POINTS = 400,
            WELDING_SEAM = 401,
            DRILL_POINTS = 600
        }

        public class ProgramItemMsg : ROSBridgeMsg {
            private UInt16 _id;     //id of program
            private UInt16 _on_success;
            private UInt16 _on_failure;
            private string _type;   //type of current operation eg. PICK_FROM_POLYGON=100, PLACE_TO_POSE=200
            private List<string> _object = new List<string>();
            private List<PoseStampedMsg> _pose = new List<PoseStampedMsg>();
            private List<PolygonStampedMsg> _polygon = new List<PolygonStampedMsg>();
            private List<UInt16> _ref_id = new List<UInt16>();
            private List<KeyValueMsg> _flags = new List<KeyValueMsg>();
            private List<string> _do_not_clear = new List<string>();
            private List<SceneLabelMsg> _labels = new List<SceneLabelMsg>();

            public ProgramItemMsg(JSONNode msg) {
                _id = UInt16.Parse(msg["id"]);
                _on_success = UInt16.Parse(msg["on_success"]);
                _on_failure = UInt16.Parse(msg["on_failure"]);
                _type = msg["type"];
                foreach (JSONNode item in msg["object"].AsArray) {
                    _object.Add(item);
                }
                foreach (JSONNode item in msg["pose"].AsArray) {
                    _pose.Add(new PoseStampedMsg(item));
                }
                foreach (JSONNode item in msg["polygon"].AsArray) {
                    _polygon.Add(new PolygonStampedMsg(item));
                }
                foreach (JSONNode item in msg["ref_id"].AsArray) {
                    _ref_id.Add(UInt16.Parse(item));
                }
                foreach (JSONNode item in msg["flags"].AsArray) {
                    _flags.Add(new KeyValueMsg(item));
                }
                foreach (JSONNode item in msg["do_not_clear"].AsArray) {
                    _do_not_clear.Add(item);
                }
                foreach (JSONNode item in msg["labels"].AsArray) {
                    _labels.Add(new SceneLabelMsg(item));
                }
            }
			
			public ProgramItemMsg(UInt16 id, UInt16 on_success, UInt16 on_failure, string type, List<string> obj, List<PoseStampedMsg> pose, List<PolygonStampedMsg> polygon, List<UInt16> ref_id, List<KeyValueMsg> flags, List<string> do_not_clear, List<SceneLabelMsg> labels) {
                _id = id;
                _on_success = on_success;
                _on_failure = on_failure;
                _type = type;
                _object = obj;
                _pose = pose;
                _polygon = polygon;
                _ref_id = ref_id;
                _flags = flags;
                _do_not_clear = do_not_clear;
                _labels = labels;
            }
			
			public static string GetMessageType() {
				return "art_msgs/ProgramItem";
			}
			
			public UInt16 GetID() {
                return _id;
            }

            public UInt16 GetOnSuccess() {
                return _on_success;
            }

            public UInt16 GetOnFailure() {
                return _on_failure;
            }

            public string GetIType() {
                return _type;
            }
            
            public List<string> GetObject() {
                return _object;
            }

            public List<PoseStampedMsg> GetPose() {
                return _pose;
            }

            public List<PolygonStampedMsg> GetPolygon() {
                return _polygon;
            }

            public List<UInt16> GetRefID() {
                return _ref_id;
            }

            public List<KeyValueMsg> GetFlags() {
                return _flags;
            }

            public List<string> GetDoNotClear() {
                return _do_not_clear;
            }

            public List<SceneLabelMsg> GetLabels() {
                return _labels;
            }
            
            public override string ToString() {
                string labelsString = "[";
                for (int i = 0; i < _labels.Count; i++) {
                    labelsString = labelsString + _labels[i].ToString();
                    if (_labels.Count - i > 1) labelsString += ",";
                }
                labelsString += "]";

                string poseString = "[";
                for (int i = 0; i < _pose.Count; i++) {
                    poseString = poseString + _pose[i].ToString();
                    if (_pose.Count - i > 1) poseString += ",";
                }
                poseString += "]";

                string polygonString = "[";
                for (int i = 0; i < _polygon.Count; i++) {
                    polygonString = polygonString + _polygon[i].ToString();
                    if (_polygon.Count - i > 1) polygonString += ",";
                }
                polygonString += "]";

                string flagsString = "[";
                for (int i = 0; i < _flags.Count; i++) {
                    flagsString = flagsString + _flags[i].ToString();
                    if (_flags.Count - i > 1) flagsString += ",";
                }
                flagsString += "]";

                List<String> _ref_idSTR = new List<String>();
                foreach (UInt16 id in _ref_id) {
                    _ref_idSTR.Add(id.ToString());
                }

                return "ProgramItem [id=" + _id +
                    ", on_success=" + _on_success +
                    ", on_failure=" + _on_failure +
                    ", type=\"" + _type + "\"" +
                    ", object=[\"" + string.Join("\",\"", _object.ToArray()) + "\"]" +
                    ", pose=" + poseString +
                    ", polygon=" + polygonString +
                    ", ref_id=[" + string.Join(",", _ref_idSTR.ToArray()) + "]" +
                    ", flags=" + flagsString +
                    ", do_not_clear=[\"" + string.Join("\",\"", _do_not_clear.ToArray()) + "\"]" +
                    ", labels=" + labelsString + "]";
            }

			public override string ToYAMLString() {
                string labelsString = "[";
                for (int i = 0; i < _labels.Count; i++) {
                    labelsString = labelsString + _labels[i].ToYAMLString();
                    if (_labels.Count - i > 1) labelsString += ",";
                }
                labelsString += "]";

                string poseString = "[";
                for (int i = 0; i < _pose.Count; i++) {
                    poseString = poseString + _pose[i].ToYAMLString();
                    if (_pose.Count - i > 1) poseString += ",";
                }
                poseString += "]";

                string polygonString = "[";
                for (int i = 0; i < _polygon.Count; i++) {
                    polygonString = polygonString + _polygon[i].ToYAMLString();
                    if (_polygon.Count - i > 1) polygonString += ",";
                }
                polygonString += "]";

                string flagsString = "[";
                for (int i = 0; i < _flags.Count; i++) {
                    flagsString = flagsString + _flags[i].ToYAMLString();
                    if (_flags.Count - i > 1) flagsString += ",";
                }
                flagsString += "]";

                List<String> _ref_idSTR = new List<String>();
                foreach (UInt16 id in _ref_id) {
                    _ref_idSTR.Add(id.ToString());
                }

                return "{\"id\":" + _id +
                    ", \"on_success\":" + _on_success +
                    ", \"on_failure\":" + _on_failure +
                    ", \"type\":\"" + _type + "\"" +
                    ", \"object\":[\"" + string.Join("\",\"", _object.ToArray()) + "\"]" +
                    ", \"pose\":" + poseString +
                    ", \"polygon\":" + polygonString +
                    ", \"ref_id\":[" + string.Join(",", _ref_idSTR.ToArray()) + "]" +
                    ", \"flags\":" + flagsString +
                    ", \"do_not_clear\":[\"" + string.Join("\",\"", _do_not_clear.ToArray()) + "\"]" +
                    ", \"labels\":" + labelsString + "}";
            }
		}
	}
}