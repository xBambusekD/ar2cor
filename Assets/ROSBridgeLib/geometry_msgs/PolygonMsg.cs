using System.Collections;
using System.Text;
using SimpleJSON;
using System.Collections.Generic;

/**
 * Define a geometry_msgs polygon message. This has been hand-crafted from the corresponding
 * geometry_msgs message file.
 * 
 * @author Daniel Bambušek
 */

namespace ROSBridgeLib {
	namespace geometry_msgs {
		public class PolygonMsg : ROSBridgeMsg {
			public PointMsg[] _points;

			public PolygonMsg(JSONNode msg) {
                List<PointMsg> points = new List<PointMsg>();
                foreach (JSONNode item in msg["points"].AsArray) {
                    points.Add(new PointMsg(item));
                }
                _points = points.ToArray();
            }

			public PolygonMsg(PointMsg[] points) {
                _points = points;
            }
			
			public static string getMessageType() {
				return "geometry_msgs/Polygon";
			}

			public PointMsg[] GetPoints() {
				return _points;
			}
            			
			public override string ToString() {
                string array = "[";
                for (int i = 0; i < _points.Length; i++) {
                    array = array + _points[i].ToString();
                    if (_points.Length - i > 1) array += ",";
                }
                array += "]";
                return "geometry_msgs/Polygon [points=" + array + "]";
			}
			
			public override string ToYAMLString() {
                string array = "[";
                for (int i = 0; i < _points.Length; i++) {
                    array = array + _points[i].ToYAMLString();
                    if (_points.Length - i > 1) array += ",";
                }
                array += "]";
                return "{\"points\":" + array + "}";
			}
		}
	}
}
