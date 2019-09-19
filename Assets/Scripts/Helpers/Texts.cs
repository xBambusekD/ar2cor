using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Texts {

    public enum Languages:UInt16 {
        ENGLISH = 0,
        CZECH = 1
    }

    public static Languages Language = Languages.ENGLISH;

    public static string OnCalibrationStarts {
        get {
            switch(Language) {
                case Languages.ENGLISH:
                    return "System is going to calibrate. Please put the markers to the corners of the table.";
                case Languages.CZECH:
                    return "Systém se bude kalibrovat. Prosím položte markery do rohů stolu.";
                default:
                    return "";
            }
        }
    }
        
    public static string OnCalibrationContinues {
        get {
            switch (Language) {
                case Languages.ENGLISH:
                    return "Marker detection started. Please look directly at each one of the markers and click on the cubes to confirm calibration.";
                case Languages.CZECH:
                    return "Detekce markerů začala. Prosím, dívejte se přímo na každý z nich a klikněte na virtuální kostky pro potvrzení kalibrace.";
                default:
                    return "";
            }
        }
    }

    public static string OnCalibrationEnd {
        get {
            switch (Language) {
                case Languages.ENGLISH:
                    return "Calibration completed.";
                case Languages.CZECH:
                    return "Kalibrace je hotova.";
                default:
                    return "";
            }
        }
    }

    #region instructions edit
    //public static string PickFromPolygonIE_NoObjectsOnTable {
    //    get {
    //        switch (Language) {
    //            case Languages.ENGLISH:
    //                return "I don't see any objects placed on the table. You have to place the object on the table that you want the robot to lift.";
    //            case Languages.CZECH:
    //                return "Na stole nevidím žádné objekty. Musíte na něj nějaký položit.";
    //            default:
    //                return "";
    //        }
    //    }
    //}

    //public static string PickFromPolygonIE_SelectObjectType {
    //    get {
    //        switch (Language) {
    //            case Languages.ENGLISH:
    //                return "Select object type to be picked up by tapping on its outline.";
    //            case Languages.CZECH:
    //                return "Vyberte typ objektu, kliknutím na jeho obrys";
    //            default:
    //                return "";
    //        }
    //    }
    //}

    //public static string PickFromPolygonIE_AdjustPickArea {
    //    get {
    //        switch (Language) {
    //            case Languages.ENGLISH:
    //                return "Adjust pick area as you want - or you can select another object type. When you are finished, click on done.";
    //            case Languages.CZECH:
    //                return;
    //            default:
    //                return "";
    //        }
    //    }
    //}

    //public static string PickFromPolygonIE_GoodJob {
    //    get {
    //        switch (Language) {
    //            case Languages.ENGLISH:
    //                return "Good job! You have successfully programmed pick from polygon instruction.";
    //            case Languages.CZECH:
    //                return;
    //            default:
    //                return "";
    //        }
    //    }
    //}

    //public static string Perfect {
    //    get {
    //        switch (Language) {
    //            case Languages.ENGLISH:
    //                return "Perfect!";
    //            case Languages.CZECH:
    //                return "Výborně!";
    //            default:
    //                return "";
    //        }
    //    }
    //}


    //public static string PlaceToPoseIE_PickIsNotProgrammed {
    //    get {
    //        switch (Language) {
    //            case Languages.ENGLISH:
    //                return "Robot doesn't know which object you want to place. You have to program picking instruction first.";
    //            case Languages.CZECH:
    //                return;
    //            default:
    //                return "";
    //        }
    //    }
    //}
    //public static string PlaceToPoseIE_DragOBjectOutline {
    //    get {
    //        switch (Language) {
    //            case Languages.ENGLISH:
    //                return "Drag object outline to set place pose and blue point to set orientation. When you are finished, click on done.";
    //            case Languages.CZECH:
    //                return;
    //            default:
    //                return "";
    //        }
    //    }
    //}


    //public static string PlaceToPoseIE_GoodJob {
    //    get {
    //        switch (Language) {
    //            case Languages.ENGLISH:
    //                return "Good job! You have successfully programmed place to pose instruction.";
    //            case Languages.CZECH:
    //                return;
    //            default:
    //                return "";
    //        }
    //    }
    //}

    //public static string PlaceToPoseIE_ForgotPlacePose {
    //    get {
    //        switch (Language) {
    //            case Languages.ENGLISH:
    //                return "You forgot to set the place pose. You have to move with it.";
    //            case Languages.CZECH:
    //                return;
    //            default:
    //                return "";
    //        }
    //    }
    //}
    #endregion

    #region visualization
    public static string Vis_VisualizationAlreadyStopped {
        get {
            switch (Language) {
                case Languages.ENGLISH:
                    return "Visualization is already stopped!";
                case Languages.CZECH:
                    return "Vizualizace už je zastavena!";
                default:
                    return "";
            }
        }
    }

    public static string Vis_ThereIsNothingToStop {
        get {
            switch (Language) {
                case Languages.ENGLISH:
                    return "There is nothing to stop!";
                case Languages.CZECH:
                    return "Není co zastavit!";
                default:
                    return "";
            }
        }
    }

    public static string Vis_YouHaveToStopVis {
        get {
            switch (Language) {
                case Languages.ENGLISH:
                    return "You have to stop the visualization first!";
                case Languages.CZECH:
                    return "Musíte vizualizaci nejprve zastavit!";
                default:
                    return "";
            }
        }
    }

    public static string Vis_NothingToReplay {
        get {
            switch (Language) {
                case Languages.ENGLISH:
                    return "There is nothing to replay!";
                case Languages.CZECH:
                    return "Není co přehrát!";
                default:
                    return "";
            }
        }
    }

    public static string Vis_AlreadyPaused {
        get {
            switch (Language) {
                case Languages.ENGLISH:
                    return "Visualization is already paused!";
                case Languages.CZECH:
                    return "Vizualizace už je pozastavena!";
                default:
                    return "";
            }
        }
    }

    public static string Vis_NotEvenRunning {
        get {
            switch (Language) {
                case Languages.ENGLISH:
                    return "Visualization is not even running!";
                case Languages.CZECH:
                    return "Vizualizace ještě ani neběží!";
                default:
                    return "";
            }
        }
    }
    
    public static string Vis_NothingToPause {
        get {
            switch (Language) {
                case Languages.ENGLISH:
                    return "There is nothing to pause!";
                case Languages.CZECH:
                    return "Není co pozastavit!";
                default:
                    return "";
            }
        }
    }

    public static string Vis_YouHaveToPauseFirst {
        get {
            switch (Language) {
                case Languages.ENGLISH:
                    return "You have to pause the visualization first!";
                case Languages.CZECH:
                    return "Musíte vizualizaci nejprve pozastavit!";
                default:
                    return "";
            }
        }
    }

    public static string Vis_NothingToResume {
        get {
            switch (Language) {
                case Languages.ENGLISH:
                    return "There is nothing to resume!";
                case Languages.CZECH:
                    return "Není v čem pokračovat!";
                default:
                    return "";
            }
        }
    }
    #endregion


    public static string Default {
        get {
            switch (Language) {
                case Languages.ENGLISH:
                    return "";
                case Languages.CZECH:
                    return "";
                default:
                    return "";
            }
        }
    }

    

}
