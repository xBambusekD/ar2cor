using System.Collections;
using System.Text;
using SimpleJSON;
using System;
using System.Collections.Generic;

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

            public HololensStateMsg(JSONNode msg) {
                _hololens_state = (hololens_state)Int16.Parse(msg["hololens_state"]);
                _visualization_state = (visualization_state)Int16.Parse(msg["visualization_state"]);
            }
			
			public HololensStateMsg(hololens_state hololens_state, visualization_state visualization_state) {
                _hololens_state = hololens_state;
                _visualization_state = visualization_state;
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

            public override string ToString() {
                return "HololensState [hololens_state=" + (Int16)_hololens_state +
                    ", visualization_state=" + (Int16)_visualization_state + "]";
			}
            
            public override string ToYAMLString() {
                return "{\"hololens_state\":" + (Int16)_hololens_state +
                    ", \"visualization_state\":" + (Int16)_visualization_state + "}";
            }
		}
	}
}