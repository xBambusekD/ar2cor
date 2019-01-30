using System.Collections;
using System.Text;
using SimpleJSON;
using ROSBridgeLib.geometry_msgs;

/* 
 * @brief ROSBridgeLib
 * @author Daniel Bambusek
 */

namespace ROSBridgeLib {
	namespace geometry_msgs {
		public class TransformMsg : ROSBridgeMsg {
            private Vector3Msg _translation;
            private QuaternionMsg _rotation;
			
			public TransformMsg(JSONNode msg) {
                _translation = new Vector3Msg(msg["translation"]);
                _rotation = new QuaternionMsg(msg["rotation"]);
			}
			
			public TransformMsg(Vector3Msg translation, QuaternionMsg rotation) {
                _translation = translation;
                _rotation = rotation;
			}
			
			public static string GetMessageType() {
				return "geometry_msgs/Transform";
			}
			
			public Vector3Msg GetTranslation() {
                return _translation;
            }

            public QuaternionMsg GetRotation() {
                return _rotation;
            }
			
			public override string ToString() {
				return "Transform [translation=" + _translation.ToString() + 
                    ",  rotation="+ _rotation.ToString() + "]";
			}
			
			public override string ToYAMLString() {
				return "{\"translation\":" + _translation.ToYAMLString() + 
                    ", \"rotation\":" + _rotation.ToYAMLString() + "}";
			}
		}
	}
}