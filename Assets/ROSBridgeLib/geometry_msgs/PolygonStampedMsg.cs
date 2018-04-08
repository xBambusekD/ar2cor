using System.Collections;
using System.Text;
using SimpleJSON;
using ROSBridgeLib.std_msgs;

/* 
 * @brief ROSBridgeLib
 * @author Daniel Bambušek
 */

namespace ROSBridgeLib {
	namespace geometry_msgs {
		public class PolygonStampedMsg : ROSBridgeMsg {
			public HeaderMsg _header;
			public PolygonMsg _polygon;
			
			public PolygonStampedMsg(JSONNode msg) {
				_header = new HeaderMsg(msg["header"]);
				_polygon = new PolygonMsg(msg["polygon"]);
			}

            public PolygonStampedMsg(HeaderMsg header, PolygonMsg polygon) {
                _header = header;
                _polygon = polygon;
            }
 			
			public static string GetMessageType() {
				return "geometry_msgs/PolygonStamped";
			}
			
			public HeaderMsg GetHeader() {
				return _header;
			}

			public PolygonMsg GetPolygon() {
				return _polygon;
			}
			
			public override string ToString() {
				return "PolygonStamped [header=" + _header.ToString() + ",  polygon=" + _polygon.ToString() + "]";
			}
			
			public override string ToYAMLString() {
				return "{\"header\":" + _header.ToYAMLString() + ", \"polygon\":" + _polygon.ToYAMLString() + "}";
			}
		}
	}
}