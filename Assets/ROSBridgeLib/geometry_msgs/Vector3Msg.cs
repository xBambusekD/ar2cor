using System.Collections;
using System.Text;
using SimpleJSON;
using ROSBridgeLib.geometry_msgs;
using UnityEngine;

/* 
 * @brief ROSBridgeLib
 * @author Michael Jenkin, Robert Codd-Downey, Andrew Speers and Miquel Massot Campos
 */

namespace ROSBridgeLib {
	namespace geometry_msgs {
		public class Vector3Msg : ROSBridgeMsg {
			private float _x;
			private float _y;
			private float _z;
			
			public Vector3Msg(JSONNode msg) {
				_x = float.Parse(msg["x"]);
				_y = float.Parse(msg["y"]);
				_z = float.Parse(msg["z"]);
			}
			
			public Vector3Msg(float x, float y, float z) {
				_x = x;
				_y = y;
				_z = z;
			}

            public Vector3Msg(Vector3 vec) {
                _x = vec.x;
                _y = vec.y;
                _z = vec.z;
            }

            public static string GetMessageType() {
				return "geometry_msgs/Vector3";
			}
			
			public double GetX() {
				return _x;
			}
			
			public double GetY() {
				return _y;
			}
			
			public double GetZ() {
				return _z;
			}
			
            public Vector3 GetVector() {
                return new Vector3(_x, _y, _z);
            }

			public override string ToString() {
				return "Vector3 [x=" + _x + ",  y="+ _y + ",  z=" + _z + "]";
			}
			
			public override string ToYAMLString() {
				return "{\"x\" : " + _x + ", \"y\" : " + _y + ", \"z\" : " + _z + "}";
			}
		}
	}
}