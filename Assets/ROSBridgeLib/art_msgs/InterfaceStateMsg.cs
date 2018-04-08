using System.Collections;
using System.Text;
using SimpleJSON;
using System;
using System.Collections.Generic;
using ROSBridgeLib.std_msgs;
using ROSBridgeLib.diagnostic_msgs;

/* 
 * @brief ARTable - VUT FIT
 * @author Daniel Bambušek
 */

namespace ROSBridgeLib {
	namespace art_msgs {
		public class InterfaceStateMsg : ROSBridgeMsg {
			private string _interface_id;
            private Int16 _system_state;
            private TimeMsg _timestamp; //0 - secs, 1 - nsecs
            private UInt16 _program_id;
            private UInt16 _block_id;
            private ProgramItemMsg _program_current_item;
            private List<KeyValueMsg> _flags = new List<KeyValueMsg>();
            private bool _edit_enabled;
            private Int32 _error_severity;
            private Int32 _error_code;

			public InterfaceStateMsg(JSONNode msg) {
                _interface_id = msg["interface_id"];
                _system_state = Int16.Parse(msg["system_state"]);
                _timestamp = new TimeMsg(msg["timestamp"]);
                _program_id = UInt16.Parse(msg["program_id"]);
                _block_id = UInt16.Parse(msg["block_id"]);
                _program_current_item = new ProgramItemMsg(msg["program_current_item"]);
                foreach (JSONNode item in msg["flags"].AsArray) {
                    _flags.Add(new KeyValueMsg(item));
                }
                _edit_enabled = bool.Parse(msg["edit_enabled"]);
                _error_severity = Int32.Parse(msg["error_severity"]);
                _error_code = Int32.Parse(msg["error_code"]);
            }
			
			public InterfaceStateMsg(string interface_id, Int16 system_state, TimeMsg timestamp, UInt16 program_id, UInt16 block_id, ProgramItemMsg program_current_item, List<KeyValueMsg> flags, bool edit_enabled, Int32 error_severity, Int32 error_code) {
                _interface_id = interface_id;
                _system_state = system_state;
                _timestamp = timestamp;
                _program_id = program_id;
                _block_id = block_id;
                _program_current_item = program_current_item;
                _flags = flags;
                _edit_enabled = edit_enabled;
                _error_severity = error_severity;
                _error_code = error_code;
			}

            public static string GetMessageType() {
				return "art_msgs/InterfaceState";
			}
			
			public string GetInterfaceID() {
				return _interface_id;
			}
			
            public Int16 GetSystemState() {
                return _system_state;
            }

            public TimeMsg GetTimestamp() {
                return _timestamp;
            }

            public UInt16 GetProgramID() {
                return _program_id;
            }

            public UInt16 GetBlockID() {
                return _block_id;
            }

            public ProgramItemMsg GetProgramCurrentItem() {
                return _program_current_item;
            }

            public List<KeyValueMsg> GetFlags() {
                return _flags;
            }

            public bool GetEditEnabled() {
                return _edit_enabled;
            }

            public Int32 GetErrorSeverity() {
                return _error_severity;
            }

            public Int32 GetErrorCode() {
                return _error_code;
            }
            
            
            public override string ToString() {
                string flagsString = "[";
                for (int i = 0; i < _flags.Count; i++) {
                    flagsString = flagsString + _flags[i].ToString();
                    if (_flags.Count - i > 1) flagsString += ",";
                }
                flagsString += "]";

                return "InterfaceState [interface_id=" + _interface_id +
                    ", system_state=" + _system_state +
                    ", timestamp=" + _timestamp.ToString() +
                    ", program_id=" + _program_id +
                    ", block_id=" + _block_id +
                    ", program_current_item=" + _program_current_item.ToString() +
                    ", flags=" + flagsString +
                    ", edit_enabled=" + _edit_enabled.ToString().ToLower() +
                    ", error_severity=" + _error_severity +
                    ", error_code=" + _error_code + "]";
			}
            
            public override string ToYAMLString() {
                string flagsString = "[";
                for (int i = 0; i < _flags.Count; i++) {
                    flagsString = flagsString + _flags[i].ToYAMLString();
                    if (_flags.Count - i > 1) flagsString += ",";
                }
                flagsString += "]";

                return "{\"interface_id\":\"" + _interface_id + "\"" +
                    ", \"system_state\":" + _system_state +
                    ", \"timestamp\":" + _timestamp.ToYAMLString() +
                    ", \"program_id\":" + _program_id +
                    ", \"block_id\":" + _block_id +
                    ", \"program_current_item\":" + _program_current_item.ToYAMLString() +
                    ", \"flags\":" + flagsString +
                    ", \"edit_enabled\":" + _edit_enabled.ToString().ToLower() +
                    ", \"error_severity\":" + _error_severity +
                    ", \"error_code\":" + _error_code + "}";
            }
		}
	}
}