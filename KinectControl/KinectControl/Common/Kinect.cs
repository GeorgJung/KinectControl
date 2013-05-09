using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.Kinect;

namespace KinectControl.Common
{
    /// <summary>
    /// Author : Microsoft
    /// </summary>
    public class Kinect
    {
        #region Gesture's variables
        private GestureController gestureController;
        private string _gesture;
        public event PropertyChangedEventHandler PropertyChanged;
        public int framesCount;
        public int FramesCount
        {
            get { return framesCount; }
            set { framesCount = value; }
        }
        public String Gesture
        {
            get { return _gesture; }

            private set
            {
                if (_gesture == value)
                    return;

                _gesture = value;

                Debug.WriteLine("Gesture = " + _gesture);

                if (this.PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Gesture"));
            }
        }
        #endregion

        private Skeleton[] skeletons;
        public KinectSensor nui;

        //Tracked Skeleton
        public Skeleton trackedSkeleton;
        private int ScreenWidth, ScreenHeight;
        //Used for scaling
        private const float SkeletonMaxX = 0.60f;
        private const float SkeletonMaxY = 0.40f;


        public Kinect(int screenWidth, int screenHeight)
        {
            skeletons = new Skeleton[0];
            trackedSkeleton = null;
            //swapHand = new SwapHand();
            ScreenHeight = screenHeight;
            ScreenWidth = screenWidth; 
            this.InitializeNui();
        }


        /// <summary>
        /// Handle insertion of Kinect sensor.
        /// </summary>
        private void InitializeNui()
        {
            _gesture = "";
            var index = 0;
            while (this.nui == null && index < KinectSensor.KinectSensors.Count)
            {
                this.nui = KinectSensor.KinectSensors[index];
                this.nui.Start();
            }
            try
            {
                this.skeletons = new Skeleton[this.nui.SkeletonStream.FrameSkeletonArrayLength];
                var parameters = new TransformSmoothParameters
                {
                    Smoothing = 0.75f,
                    Correction = 0.0f,
                    Prediction = 0.0f,
                    JitterRadius = 0.05f,
                    MaxDeviationRadius = 0.04f
                };
                this.nui.SkeletonStream.Enable(parameters);
                this.nui.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
            }
            catch (Exception)
            { return; }
            this.nui.SkeletonFrameReady += this.OnSkeletonFrameReady;
            gestureController = new GestureController();
            InitializeGestures();
            gestureController.GestureRecognized += OnGestureRecognized;
        }

        /// <summary>
        /// Handler for skeleton ready handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void OnSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            // Get the frame.
            using (SkeletonFrame frame = e.OpenSkeletonFrame())
            {
                if (frame != null)
                {
                    frame.CopySkeletonDataTo(this.skeletons);
                    for (int i = 0; i < this.skeletons.Length; i++)
                    {
                        Skeleton skeleton = this.skeletons[i];
                        if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            this.trackedSkeleton = skeleton;
                        }
                    }
                    if (trackedSkeleton != null)
                    {
                        if (framesCount == 1)
                            Constants.oldSkeleton = trackedSkeleton;
                        else if (framesCount % (40) == 0)
                        {
                            Constants.oldSkeleton = trackedSkeleton;
                        }
                        if (GenerateDepth() > 120)
                        {
                            framesCount++;
                            gestureController.UpdateAllGestures(trackedSkeleton);
                        }
                    }
                }
            }
        }

        public Skeleton[] requestSkeleton()
        {
            return skeletons;
        }

        /// <summary>
        /// Returns right hand position scaled to screen.
        /// </summary>
        public Joint GetCursorPosition()
        {
            if (trackedSkeleton != null)
                return trackedSkeleton.Joints[JointType.HandRight].ScaleTo(ScreenWidth, ScreenHeight, SkeletonMaxX, SkeletonMaxY);
            else
                return new Joint();
        }
     
        private void OnGestureRecognized(object sender, GestureEventArgs e)
        {
            Debug.WriteLine(e.GestureType);

            switch (e.GestureType)
            {
                case GestureType.WaveLeft:
                    Gesture = "WaveLeft";
                    break;
                case GestureType.PunchGesture:
                    Gesture = "PunchGesture";
                    break;
            }
             
        }

        public static List<double> Fill_Joint_Pos(Skeleton skeleton, Joint joint, string dimension)
        {
            double min = 0;
            double max = 0;
            switch (dimension)
            {
                case "x":
                    List<double> xPos = new List<double>();
                    for (int i = 0; i < 9; i++)
                    {
                        xPos.Add((double)Math.Round((skeleton.Joints[joint.JointType].Position.X)));
                        if (xPos.Count == 1)
                        {
                            min = xPos[xPos.Count - 1];
                            max = xPos[xPos.Count - 1];
                        }
                        else if (xPos.Count > 1)
                        {
                            if (xPos[xPos.Count - 1] == xPos[xPos.Count - 2])
                                xPos.RemoveAt(xPos.Count - 2);
                            if (xPos[xPos.Count - 1] > max)
                                max = xPos[xPos.Count - 1];
                            if (xPos[xPos.Count - 1] < min)
                                min = xPos[xPos.Count - 1];
                        }
                        if (xPos.Count == 10)
                            xPos.RemoveAt(0);
                        xPos.Add((double)Math.Round((skeleton.Joints[joint.JointType].Position.X)));

                    }
                    //Constants.minmax = ToString(min, max);
                    return xPos;
                case "y":
                    List<double> yPos = new List<double>();
                    for (int i = 0; i < 9; i++)
                    {
                        yPos.Add((double)Math.Round((skeleton.Joints[joint.JointType].Position.Y)));
                        if (yPos.Count == 1)
                        {
                            min = yPos[yPos.Count - 1];
                            max = yPos[yPos.Count - 1];
                        }
                        else if (yPos.Count > 1)
                        {
                            if (yPos[yPos.Count - 1] == yPos[yPos.Count - 2])
                                yPos.RemoveAt(yPos.Count - 2);
                            if (yPos[yPos.Count - 1] > max)
                                max = yPos[yPos.Count - 1];
                            if (yPos[yPos.Count - 1] < min)
                                min = yPos[yPos.Count - 1];
                        }

                        if (yPos.Count == 10)
                            yPos.RemoveAt(0);
                        yPos.Add((double)Math.Round((skeleton.Joints[joint.JointType].Position.Y)));

                    }
                    //Constants.minmax = ToString(min, max);
                    return yPos;

                case "z":
                    List<double> zPos = new List<double>();
                    for (int i = 0; i < 9; i++)
                    {
                        zPos.Add((double)Math.Round((skeleton.Joints[joint.JointType].Position.Z)));
                        if (zPos.Count == 1)
                        {
                            min = zPos[zPos.Count - 1];
                            max = zPos[zPos.Count - 1];
                        }
                        else if (zPos.Count > 1)
                        {
                            if (zPos[zPos.Count - 1] == zPos[zPos.Count - 2])
                                zPos.RemoveAt(zPos.Count - 2);
                            if (zPos[zPos.Count - 1] > max)
                                max = zPos[zPos.Count - 1];
                            if (zPos[zPos.Count - 1] < min)
                                min = zPos[zPos.Count - 1];
                        }

                        if (zPos.Count == 10)
                            zPos.RemoveAt(0);
                        zPos.Add((double)Math.Round((skeleton.Joints[joint.JointType].Position.Z)));

                    }
                    //Constants.minmax = ToString(min, max);
                    return zPos;
                default: return new List<double>();

            }
        }

        public static string ToString(double min, double max)
        {
            return min + "," + max;
        }
        public void InitializeGestures()
        {
            IRelativeGestureSegment[] waveLeftSegments = new IRelativeGestureSegment[6];
            WaveLeftSegment1 waveLeftSegment1 = new WaveLeftSegment1();
            WaveLeftSegment2 waveLeftSegment2 = new WaveLeftSegment2();
            waveLeftSegments[0] = waveLeftSegment1;
            waveLeftSegments[1] = waveLeftSegment2;
            waveLeftSegments[2] = waveLeftSegment1;
            waveLeftSegments[3] = waveLeftSegment2;
            waveLeftSegments[4] = waveLeftSegment1;
            waveLeftSegments[5] = waveLeftSegment2;
            this.gestureController.AddGesture(GestureType.WaveLeft, waveLeftSegments);
            IRelativeGestureSegment[] PunchSegments = new IRelativeGestureSegment[1];
            PunchGesture1 punchGesture1 = new PunchGesture1();
            PunchSegments[0] = punchGesture1;
            this.gestureController.AddGesture(GestureType.PunchGesture, PunchSegments);
        }

        
        /// <returns>
        /// Int number which is the calculated depth.
        /// </returns>
        public int GenerateDepth()
        {
            try
            {
                return (int)(100 * this.trackedSkeleton.Joints[JointType.HipCenter].Position.Z);
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}
