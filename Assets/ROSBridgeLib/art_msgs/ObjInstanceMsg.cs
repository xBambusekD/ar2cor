using System.Collections;
using System.Text;
using SimpleJSON;
using System;
using System.Collections.Generic;
using ROSBridgeLib.shape_msgs;
using ROSBridgeLib.geometry_msgs;
using ROSBridgeLib.diagnostic_msgs;

/* 
 * @brief ARTable - VUT FIT
 * @author Daniel Bambušek
 */

namespace ROSBridgeLib {
	namespace art_msgs {
        public class ObjInstanceMsg : ROSBridgeMsg { 

            private string _object_id;
            private string _object_type;
            private PoseMsg _pose;
            private List<KeyValueMsg> _flags = new List<KeyValueMsg>();
            private bool _on_table;

            public ObjInstanceMsg(JSONNode msg) {
                _object_id = msg["object_id"];
                _object_type = msg["object_type"];
                _pose = new PoseMsg(msg["pose"]);
                foreach (JSONNode item in msg["flags"].AsArray) {
                    _flags.Add(new KeyValueMsg(item));
                }
                _on_table = bool.Parse(msg["on_table"]);
            }
			
			public ObjInstanceMsg(string object_id, string object_type, PoseMsg pose, List<KeyValueMsg> flags, bool on_table) {
                _object_id = object_id;
                _object_type = object_type;
                _pose = pose;
                _flags = flags;
                _on_table = on_table;
            }

            public static string GetMessageType() {
				return "art_msgs/ObjInstance";
			}
			
            public string GetObjectId() {
                return _object_id;
            }

            public string GetObjectType() {
                return _object_type;
            }

            public PoseMsg GetPose() {
                return _pose;
            }

            public List<KeyValueMsg> GetFlags() {
                return _flags;
            }

            public bool GetOnTable() {
                return _on_table;
            }

            public override string ToString() {
                string flagsString = "[";
                for (int i = 0; i < _flags.Count; i++) {
                    flagsString = flagsString + _flags[i].ToString();
                    if (_flags.Count - i > 1) flagsString += ",";
                }
                flagsString += "]";

                return "ObjInstance [object_id=" + _object_id +
                    ", object_type=" + _object_type +
                    ", pose=" + _pose.ToString() +
                    ", flags=" + flagsString + 
                    ", on_table=" + _on_table.ToString().ToLower() + "]";
			}
            
            public override string ToYAMLString() {
                string flagsString = "[";
                for (int i = 0; i < _flags.Count; i++) {
                    flagsString = flagsString + _flags[i].ToYAMLString();
                    if (_flags.Count - i > 1) flagsString += ",";
                }
                flagsString += "]";

                return "{\"object_id\":\"" + _object_id + "\"" +
                    ", \"object_type\":\"" + _object_type + "\"" +
                    ", \"pose\":" + _pose.ToYAMLString() +
                    ", \"flags\":" + flagsString + 
                    ", \"on_table\":" + _on_table.ToString().ToLower() + "}";
            }
		}
	}
}