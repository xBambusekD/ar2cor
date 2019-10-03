using System.Collections;
using System.Text;
using SimpleJSON;
using System.Collections.Generic;
using UnityEngine;
using System;

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

            public PolygonMsg(List<PointMsg> points) {
                _points = points.ToArray();
            }

            public static string getMessageType() {
				return "geometry_msgs/Polygon";
			}

			public PointMsg[] GetPoints() {
				return _points;
			}

            public List<Vector3> GetPointsVector() {
                List<Vector3> points = new List<Vector3>();
                foreach (PointMsg point in _points) {
                    points.Add(new Vector3(point.GetX(), point.GetY(), point.GetZ()));
                }
                return points;
            }

            public List<Vector2> GetPointsVector2D() {
                List<Vector2> points = new List<Vector2>();
                foreach (PointMsg point in _points) {
                    points.Add(new Vector3(point.GetX(), point.GetY()));
                }
                return points;
            }

            public Vector3 GetCentroid() {
                List<Vector3> poly = GetPointsVector();

                float accumulatedArea = 0.0f;
                float centerX = 0.0f;
                float centerY = 0.0f;

                for (int i = 0, j = poly.Count - 1; i < poly.Count; j = i++) {
                    float temp = poly[i].x * poly[j].y - poly[j].x * poly[i].y;
                    accumulatedArea += temp;
                    centerX += (poly[i].x + poly[j].x) * temp;
                    centerY += (poly[i].y + poly[j].y) * temp;
                }

                if (Math.Abs(accumulatedArea) < 1E-7f)
                    return new Vector2();  // Avoid division by zero

                accumulatedArea *= 3f;
                return new Vector3(centerX / accumulatedArea, centerY / accumulatedArea, 0f);
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
