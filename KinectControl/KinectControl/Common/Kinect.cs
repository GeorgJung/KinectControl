using System;
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.Kinect;
using System.Threading;

namespace KinectControl.Common
{
   
    public class Kinect
    {
        #region Gesture's variables
        private GestureController gestureController;
        private string _gesture;
        public Device[] devices;
        private string[] commands;
        private VoiceCommands _voiceCommands;
        public event PropertyChangedEventHandler PropertyChanged;
        private static int framesCount;
        public CommunicationManager comm;
        public static int FramesCount
        {
            get { return framesCount; }
            set { framesCount = value; }
        }
        public VoiceCommands voiceCommands
        {
            get { return _voiceCommands; }
            set { _voiceCommands = value; }
        }
        public String Gesture
        {
            get { return _gesture; }

            set
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
            InitializeDevices();
            gestureController.GestureRecognized += OnGestureRecognized;
            InitializeVoiceGrammar();
        }
        public void InitializeDevices()
        {
            devices = new Device[2];
            devices[0] = new Device("LED1", "1", "0");
            devices[1] = new Device("LED2", "2", "9");
            foreach(Device d in devices)
            {
                d.switchOff(comm);
            }
        }
        public void InitializeVoiceGrammar()
        {
            commands = new string[10];
            commands[0] = "play mediaplayer";
            commands[1] = "stop";
            commands[2] = "next";
            commands[3] = "previous";
            commands[4] = "mute";
            commands[5] = "unmute";
            commands[6] = "resume";
            commands[7] = "play project music";
            commands[8] = "device one";
            commands[9] = "device two";
            _voiceCommands = new VoiceCommands(nui, commands);
            var voiceThread = new Thread(_voiceCommands.StartAudioStream);
            voiceThread.Start();
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
                    framesCount++;
                    if (trackedSkeleton != null)
                    {
                        if (GenerateDepth() > 140)
                        {
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
            framesCount=0;
            switch (e.GestureType)
            {
                case GestureType.WaveLeft:
                    Gesture = "WaveLeft";
                    //1,0
                    if (devices[0].IsSwitchedOn)
                        devices[0].switchOff(comm);
                    else
                        devices[0].switchOn(comm);
                    break;
                case GestureType.RaiseHand:
                    Gesture = "RaiseHand";
                    //2,9
                    if (devices[1].IsSwitchedOn)
                        devices[1].switchOff(comm);
                    else
                        devices[1].switchOn(comm);
                        break;
            }
        }
        public void InitializeGestures()
        {
            nui.ElevationAngle = 15;
            comm = new CommunicationManager("9600");
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
            IRelativeGestureSegment[] raiseHandSegments = new IRelativeGestureSegment[2];
            RaiseHandSegment1 raiseHandSegment1 = new RaiseHandSegment1();
            RaiseHandSegment2 raiseHandSegment2 = new RaiseHandSegment2();
            raiseHandSegments[0]=raiseHandSegment1;
            raiseHandSegments[1]=raiseHandSegment2;
            this.gestureController.AddGesture(GestureType.RaiseHand,raiseHandSegments);
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
