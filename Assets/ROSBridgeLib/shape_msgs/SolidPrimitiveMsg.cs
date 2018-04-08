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
	namespace shape_msgs {
        public enum primitive_type : UInt16 {
            BOX = 1,
            SPHERE = 2,
            CYLINDER = 3,
            CONE = 4,
        }
        public class SolidPrimitiveMsg : ROSBridgeMsg {           

            private primitive_type _type;
            private List<float> _dimensions = new List<float>();

            public SolidPrimitiveMsg(JSONNode msg) {
                _type = (primitive_type)UInt16.Parse(msg["type"]);
                foreach (JSONNode item in msg["dimensions"].AsArray) {
                    _dimensions.Add(float.Parse(item));
                }
            }
			
			public SolidPrimitiveMsg(primitive_type type, List<float> dimensions) {
                _type = type;
                _dimensions = dimensions;
			}

            public static string GetMessageType() {
				return "shape_msgs/SolidPrimitive";
			}
			
            public primitive_type GetPrimitiveType() {
                return _type;
            }

            public List<float> GetDimesions() {
                return _dimensions;
            }

            public override string ToString() {
                return "SolidPrimitive [type=" + (UInt16)_type +
                    ", dimensions=" + _dimensions.ToArray() + "]";
			}
            
            public override string ToYAMLString() {
                return "{\"type\":" + (UInt16)_type +
                    ", \"dimensions\":" + _dimensions.ToArray() + "}";
            }
		}
	}
}