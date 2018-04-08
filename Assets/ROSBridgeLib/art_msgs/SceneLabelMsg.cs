using System.Collections;
using System.Text;
using SimpleJSON;
using System;
using System.Collections.Generic;
using ROSBridgeLib.geometry_msgs;

/* 
 * @brief ARTable - VUT FIT
 * @author Daniel Bambušek
 */

namespace ROSBridgeLib {
	namespace art_msgs {
		public class SceneLabelMsg : ROSBridgeMsg {
            private PoseStampedMsg _pose;
            private float _width;
            private float _height;
            private string _text;
            private string _image_filename;

			public SceneLabelMsg(JSONNode msg) {
                _pose = new PoseStampedMsg(msg["pose"]);
                _width = msg["width"].AsFloat;
                _height = msg["height"].AsFloat;
                _text = msg["test"];
                _image_filename = msg["image_filename"];
            }
			
			public SceneLabelMsg(PoseStampedMsg pose, float width, float height, string text, string image_filename) {
                _pose = pose;
                _width = width;
                _height = height;
                _text = text;
                _image_filename = image_filename;
			}

            public static string GetMessageType() {
				return "art_msgs/SceneLabel";
			}
			
			public PoseStampedMsg GetPose() {
                return _pose;
            }
            
            public float GetWidth() {
                return _width;
            }

            public float GetHeight() {
                return _height;
            }

            public string GetText() {
                return _text;
            }

            public string GetImageFilename() {
                return _image_filename;
            }
            
            public override string ToString() {
                return "SceneLabel [pose=" + _pose.ToString() +
                    ", width=" + _width +
                    ", height=" + _height +
                    ", text=" + _text +
                    ", image_filename=" + _image_filename + "]";
            }

            public override string ToYAMLString() {
                return "{\"pose\":" + _pose.ToYAMLString() +
                    ", \"width\":" + _width +
                    ", \"height\":" + _height +
                    ", \"text\":" + _text +
                    ", \"image_filename\":" + _image_filename + "}";
            }
		}
	}
}