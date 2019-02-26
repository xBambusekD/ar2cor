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

            public enum SystemState:Int16 {
                STATE_UNKNOWN = -1,
                STATE_INITIALIZING = 0,
                STATE_IDLE = 1,
                STATE_LEARNING = 2,
                STATE_LEARNING_RUNNING = 3,
                STATE_PROGRAM_RUNNING = 4,
                STATE_PROGRAM_STOPPED = 5,
                STATE_PROGRAM_FINISHED = 6,
                STATE_VISUALIZE = 7
            }

            public enum ErrorSeverity:Int32 {
                NONE = 0,
                SEVERE = 1,
                ERROR = 2,
                WARNING = 3,
                INFO = 4
            }

            public enum ErrorCode:Int32 {
                ERROR_UNKNOWN = 0,
                ERROR_ROBOT_HALTED = 1001,
                //program errors
                ERROR_OBJECT_MISSING = 1101,
                ERROR_OBJECT_MISSING_IN_POLYGON = 1102,
                ERROR_NO_GRIPPER_AVAILABLE = 1201,
                ERROR_OBJECT_IN_GRIPPER = 1202,
                ERROR_NO_OBJECT_IN_GRIPPER = 1203,
                ERROR_PICK_FAILED = 1301,
                ERROR_PICK_PLACE_SERVER_NOT_READY = 1302,
                ERROR_PLACE_FAILED = 1401,
                ERROR_GRIPPER_NOT_HOLDING_SELECTED_OBJECT = 1402,
                ERROR_DRILL_FAILED = 1501,
                //learning errors
                ERROR_GRIPPER_MOVE_FAILED = 2101
            }


			private string _interface_id;
            private SystemState _system_state;
            private TimeMsg _timestamp; //0 - secs, 1 - nsecs
            private UInt16 _program_id;
            private UInt16 _block_id;
            private ProgramItemMsg _program_current_item;
            private List<KeyValueMsg> _flags = new List<KeyValueMsg>();
            private bool _edit_enabled;
            private ErrorSeverity _error_severity;
            private ErrorCode _error_code;

			public InterfaceStateMsg(JSONNode msg) {
                _interface_id = msg["interface_id"];
                _system_state = (SystemState) Int16.Parse(msg["system_state"]);
                _timestamp = new TimeMsg(msg["timestamp"]);
                _program_id = UInt16.Parse(msg["program_id"]);
                _block_id = UInt16.Parse(msg["block_id"]);
                _program_current_item = new ProgramItemMsg(msg["program_current_item"]);
                foreach (JSONNode item in msg["flags"].AsArray) {
                    _flags.Add(new KeyValueMsg(item));
                }
                _edit_enabled = bool.Parse(msg["edit_enabled"]);
                _error_severity = (ErrorSeverity) Int32.Parse(msg["error_severity"]);
                _error_code = (ErrorCode) Int32.Parse(msg["error_code"]);
            }
			
			public InterfaceStateMsg(string interface_id, SystemState system_state, TimeMsg timestamp, UInt16 program_id, UInt16 block_id, 
                ProgramItemMsg program_current_item, List<KeyValueMsg> flags, bool edit_enabled, ErrorSeverity error_severity, ErrorCode error_code) {
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
			
            public SystemState GetSystemState() {
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

            public ErrorSeverity GetErrorSeverity() {
                return _error_severity;
            }

            public ErrorCode GetErrorCode() {
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
                    ", system_state=" + (Int16) _system_state +
                    ", timestamp=" + _timestamp.ToString() +
                    ", program_id=" + _program_id +
                    ", block_id=" + _block_id +
                    ", program_current_item=" + _program_current_item.ToString() +
                    ", flags=" + flagsString +
                    ", edit_enabled=" + _edit_enabled.ToString().ToLower() +
                    ", error_severity=" + (Int32) _error_severity +
                    ", error_code=" + (Int32) _error_code + "]";
			}
            
            public override string ToYAMLString() {
                string flagsString = "[";
                for (int i = 0; i < _flags.Count; i++) {
                    flagsString = flagsString + _flags[i].ToYAMLString();
                    if (_flags.Count - i > 1) flagsString += ",";
                }
                flagsString += "]";

                return "{\"interface_id\":\"" + _interface_id + "\"" +
                    ", \"system_state\":" + (Int16) _system_state +
                    ", \"timestamp\":" + _timestamp.ToYAMLString() +
                    ", \"program_id\":" + _program_id +
                    ", \"block_id\":" + _block_id +
                    ", \"program_current_item\":" + _program_current_item.ToYAMLString() +
                    ", \"flags\":" + flagsString +
                    ", \"edit_enabled\":" + _edit_enabled.ToString().ToLower() +
                    ", \"error_severity\":" + (Int32) _error_severity +
                    ", \"error_code\":" + (Int32) _error_code + "}";
            }
		}
	}
}