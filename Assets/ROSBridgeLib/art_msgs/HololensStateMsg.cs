using System.Collections;
using System.Text;
using SimpleJSON;
using System;
using System.Collections.Generic;
using ROSBridgeLib.diagnostic_msgs;

/* 
 * @brief ARTable - VUT FIT
 * @author Daniel Bambušek
 */

namespace ROSBridgeLib {
	namespace art_msgs {
        public enum hololens_state:Int16 {
            STATE_IDLE = 0,
            STATE_VISUALIZING = 1,
            STATE_LEARNING_DRILL = 2,
            STATE_LEARNING_PICK_FROM_FEEDER = 3
        }
        public enum visualization_state:Int16 {
            VISUALIZATION_DISABLED = 0,
            VISUALIZATION_RUN = 1,
            VISUALIZATION_PAUSE = 2,
            VISUALIZATION_RESUME = 3,
            VISUALIZATION_STOP = 4,
            VISUALIZATION_REPLAY = 5
        }

        public class HololensStateMsg : ROSBridgeMsg {           

            private hololens_state _hololens_state;
            private visualization_state _visualization_state;
            //private List<KeyValueMsg> _flags = new List<KeyValueMsg>();
            private bool _visualize_whole_program;

            public HololensStateMsg(JSONNode msg) {
                _hololens_state = (hololens_state)Int16.Parse(msg["hololens_state"]);
                _visualization_state = (visualization_state)Int16.Parse(msg["visualization_state"]);
                //foreach (JSONNode item in msg["flags"].AsArray) {
                //    _flags.Add(new KeyValueMsg(item));
                //}
                _visualize_whole_program = bool.Parse(msg["visualize_whole_program"]);
            }
			
			public HololensStateMsg(hololens_state hololens_state, visualization_state visualization_state, bool visualize_whole_program) {
                _hololens_state = hololens_state;
                _visualization_state = visualization_state;
                //_flags = flags;
                _visualize_whole_program = visualize_whole_program;
			}

            public static string GetMessageType() {
				return "art_msgs/HololensState";
			}
			
            public hololens_state GetHololensState() {
                return _hololens_state;
            }

            public visualization_state GetVisualizationState() {
                return _visualization_state;
            }


            public bool GetVisualizeWholeProgram() {
                return _visualize_whole_program;
            }

            //public List<KeyValueMsg> GetFlags() {
            //    return _flags;
            //}

            //return flag which tells if whole progra should be visualized (true) .. or just specific block (false)
            //public bool GetFlagVisualizeWholeProgram() {
            //    foreach(KeyValueMsg flag in _flags) {
            //        if(flag.GetKey().Equals("visualize_whole_program")) {
            //            return bool.Parse(flag.GetValue());
            //        }
            //    }
            //    return true;
            //}

            public override string ToString() {
                //string flagsString = "[";
                //for (int i = 0; i < _flags.Count; i++) {
                //    flagsString = flagsString + _flags[i].ToString();
                //    if (_flags.Count - i > 1) flagsString += ",";
                //}
                //flagsString += "]";

                return "HololensState [hololens_state=" + (Int16)_hololens_state +
                    ", visualization_state=" + (Int16)_visualization_state +
                    ", visualize_whole_program=" + _visualize_whole_program.ToString().ToLower() + "]";
			}
            
            public override string ToYAMLString() {
                //string flagsString = "[";
                //for (int i = 0; i < _flags.Count; i++) {
                //    flagsString = flagsString + _flags[i].ToYAMLString();
                //    if (_flags.Count - i > 1) flagsString += ",";
                //}
                //flagsString += "]";

                return "{\"hololens_state\":" + (Int16)_hololens_state +
                    ", \"visualization_state\":" + (Int16)_visualization_state +
                    ", \"visualize_whole_program\":" + _visualize_whole_program.ToString().ToLower() + "}";
            }
		}
	}
}