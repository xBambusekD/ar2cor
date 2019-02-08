using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FakeFeederObjectsPositions {

    public class FeederObjects {
        public const string Stretcher = "Stretcher";
        public const string Spojka = "Spojka";
        public const string ShortLeg = "ShortLeg";
        public const string Kratka_noha = "Kratka_noha";
        public const string LongLeg = "LongLeg";
        public const string Dlouha_noha = "Dlouha_noha";
    }

    public enum FeederType {
        left_feeder,
        right_feeder
    }


    public static Vector3 GetObjectPositionInFeeder(string objectType, FeederType feeder) {
        Vector3 position = Vector3.zero;
        if(feeder == FeederType.right_feeder) {
            position.x = 1.69f;
        }
        else {
            position.x = -0.165f;
        }

        switch(objectType) {
            case FeederObjects.Stretcher:
            case FeederObjects.Spojka:
                //position.y = (0.7f + (0.7f - 0.18f)) / 2.0f + 0.046f;
                //position.y = 0.61f;
                position.y = 0.656f;
                //position.z = 0.19f + 0.015f;
                position.z = 0.205f;
                break;
            case FeederObjects.ShortLeg:
            case FeederObjects.Kratka_noha:
                //position.y = ((0.7f - 0.495f) + (0.7f - 0.18f)) / 2.0f + 0.046f;
                //position.y = 0.3625f;
                position.y = 0.4085f;
                //position.z = 0.19f + 0.015f;
                position.z = 0.205f;
                break;
            case FeederObjects.LongLeg:
            case FeederObjects.Dlouha_noha:
                //position.y = (0.7f + (0.7f - 0.18f)) / 2.0f + 0.046f;
                //position.y = 0.61f;
                position.y = 0.656f;
                //position.z = 0.39f + 0.015f;
                position.z = 0.405f;
                break;
        }

        return position;
    }

    public static Quaternion GetObjectOrientationInFeeder(string objectType, FeederType feeder) {
        Quaternion orientation = new Quaternion();

        if (feeder == FeederType.right_feeder) {
            switch (objectType) {
                case FeederObjects.Stretcher:
                case FeederObjects.Spojka:
                    orientation.x = 0.6f;
                    orientation.y = 0.395f;
                    orientation.z = -0.373f;
                    orientation.w = 0.586f;
                    break;
                case FeederObjects.ShortLeg:
                case FeederObjects.Kratka_noha:
                    orientation.x = 0.5705f;
                    orientation.y = 0.4311f;
                    orientation.z = -0.4121f;
                    orientation.w = 0.5645f;
                    break;
                case FeederObjects.LongLeg:
                case FeederObjects.Dlouha_noha:
                    orientation.x = 0.5831f;
                    orientation.y = 0.4178f;
                    orientation.z = -0.3825f;
                    orientation.w = 0.5822f;
                    break;
            }
        }
        else {
            switch (objectType) {
                case FeederObjects.Stretcher:
                case FeederObjects.Spojka:
                    orientation.x = -0.4094f;
                    orientation.y = 0.5576f;
                    orientation.z = 0.5858f;
                    orientation.w = 0.4222f;
                    break;
                case FeederObjects.ShortLeg:
                case FeederObjects.Kratka_noha:
                    orientation.x = -0.408f;
                    orientation.y = 0.5753f;
                    orientation.z = 0.5737f;
                    orientation.w = 0.4161f;
                    break;
                case FeederObjects.LongLeg:
                case FeederObjects.Dlouha_noha:
                    orientation.x = -0.387f;
                    orientation.y = 0.5832f;
                    orientation.z = 0.6023f;
                    orientation.w = 0.3834f;
                    break;
            }
        }

        return orientation;
    }
}
