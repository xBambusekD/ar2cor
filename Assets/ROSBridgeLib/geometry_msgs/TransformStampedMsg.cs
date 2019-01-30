using System.Collections;
using System.Text;
using SimpleJSON;
using ROSBridgeLib.geometry_msgs;
using ROSBridgeLib.std_msgs;

/* 
 * @brief ROSBridgeLib
 * @author Daniel Bambusek
 */

namespace ROSBridgeLib {
	namespace geometry_msgs {
		public class TransformStampedMsg : ROSBridgeMsg {
            private HeaderMsg _header;
            private string _child_frame_id;
            private TransformMsg _transform;
			
			public TransformStampedMsg(JSONNode msg) {
                _header = new HeaderMsg(msg["header"]);
                _child_frame_id = msg["child_frame_id"];
                _transform = new TransformMsg(msg["transform"]);
			}
			
			public TransformStampedMsg(HeaderMsg header, string child_frame_id, TransformMsg transform) {
                _header = header;
                _child_frame_id = child_frame_id;
                _transform = transform;
            }
			
			public static string GetMessageType() {
				return "geometry_msgs/TransformStamped";
			}
			
			public HeaderMsg GetHeader() {
                return _header;
            }

            public string GetChildFrameId() {
                return _child_frame_id;
            }

            public TransformMsg GetTransform() {
                return _transform;
            }

            public override string ToString() {
				return "TransformStamped [header=" + _header.ToString() +
                    ",  child_frame_id=\"" + _child_frame_id + "\"" +
                    ",  transform=" + _transform.ToString() + "]";
			}
			
			public override string ToYAMLString() {
				return "{\"header\":" + _header.ToYAMLString() +
                    ", \"child_frame_id\":\"" + _child_frame_id + "\"" +
                    ", \"transform\":" + _transform.ToYAMLString() + "}";
			}
		}
	}
}