/*using System;
using System.Linq;
using Microsoft.Kinect;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;
using System.IO;


namespace Mechanect.Common
{
    /// <summary>
    /// Voice Command Class allow you to check if certain word was said or not by user.
    /// </summary>
    /// <remarks>
    /// <para>AUTHOR: Tamer Nabil </para>
    /// </remarks>
    public class VoiceCommands
    {

        KinectAudioSource kinectAudio;
        SpeechRecognitionEngine speechRecognitionEngine;
        Stream stream;
        readonly KinectSensor kinect;
        string heardString = " ";

        /// <summary>
        /// Constructor takes as input Kinect Sensor and use it to initialize the instance variable 
        ///"Kinect" and call InitalizeKinectAudio() to initiate the audio and string Command contains commands.
        ///Seperated By ",".
        /// </summary>
        /// <remarks>
        /// <para>AUTHOR: Tamer Nabil </para>
        /// </remarks>
        /// <param name="kinect">kinect sensor</param>
        /// <param name="commands">commands</param>

        public VoiceCommands(KinectSensor kinect, string commands)
        {
            this.kinect = kinect;
            InitalizeKinectAudio(commands);
        }
        /// <summary>
        /// InitalizeKinectAudio()   Get called by the constructor to initialize current Kinect audio Source and 
        /// add grammers which can be accepted.
        /// </summary>
        /// <remarks>
        /// <para>AUTHOR: Tamer Nabil </para>
        /// </remarks>
        /// <param name="commands">list of commands separated by ","</param>

        private void InitalizeKinectAudio(string commands)
        {
            string[] arrayOfCommands = commands.Split(',');
            RecognizerInfo recognizerInfo = GetKinectRecognizer();
            speechRecognitionEngine = new SpeechRecognitionEngine(recognizerInfo.Id);
            var choices = new Choices();
            foreach (var command in arrayOfCommands)
            {
                choices.Add(command);
            }
            var grammarBuilder = new GrammarBuilder { Culture = recognizerInfo.Culture };
            grammarBuilder.Append(choices);
            var grammar = new Grammar(grammarBuilder);
            speechRecognitionEngine.LoadGrammar(grammar);
            speechRecognitionEngine.SpeechRecognized += SpeechRecognitionEngineSpeechRecognized;
        }
        /// <summary>
        /// StartAudioStream is the method that starts the Engine so the user can give VoiceCommands .
        /// </summary>
        /// <remarks>
        /// <para>AUTHOR: Tamer Nabil </para>
        /// </remarks>

        public void StartAudioStream()
        {
            try
            {
                kinectAudio = kinect.AudioSource;
                stream = kinectAudio.Start();
                speechRecognitionEngine.SetInputToAudioStream(stream,
                                                              new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1,
                                                                                        32000, 2, null));
                speechRecognitionEngine.RecognizeAsync(RecognizeMode.Multiple);
            }
            catch
            {

            }

        }

        /// <summary>
        /// GetHeard take expectedString as input and compare it with the Heard string from kinect and returns true
        /// if equal  and false otherwise.
        /// </summary>
        /// <remarks>
        /// <para>AUTHOR: Tamer Nabil </para>
        /// </remarks>
        /// <param name="expectedString">Expected String</param>
        /// <returns>returns boolean ,true if he heard expectedString and false otherwise</returns>

        public bool GetHeard(string expectedString)
        {
            return expectedString.Equals(heardString);
        }

        [Obsolete("GetHeared is deprecated, please use GetHeard instead.")]
        public bool GetHeared(string expectedString)
        {
            return expectedString.Equals(heardString);
        }


        /// <summary>
        /// This method store value of what is said to  kinect in the instance variable 
        /// "heardString"
        /// </summary>
        /// <remarks>
        /// <para>AUTHOR: Tamer Nabil </para>
        /// </remarks>
        /// <param name="sender">sender</param>
        /// <param name="e">Event argument</param>
        private void SpeechRecognitionEngineSpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Confidence >= 0.55)
                heardString = e.Result.Text;
        }

        /// <summary>
        /// Static method that returns a list of speech recognition engines on the system. 
        /// Speech uses a Language-Integrated Query (LINQ) to obtain the ID of the first recognizer in the list and 
        /// returns the results as a RecognizerInfo object. Speech 
        /// then uses RecognizerInfo.Id to create a SpeechRecognitionEngine object.
        /// </summary>
        /// <remarks>
        /// <para>AUTHOR: Tamer Nabil </para>
        /// </remarks>
        /// <returns>returns RecognizerInfo</returns>
        private static RecognizerInfo GetKinectRecognizer()
        {
            Func<RecognizerInfo, bool> matchingFunc = matchFunction =>
            {
                string value;
                matchFunction.AdditionalInfo.TryGetValue("Kinect", out value);
                return "True".Equals(value, StringComparison.InvariantCultureIgnoreCase) && "en-US".Equals(matchFunction.Culture.Name, StringComparison.InvariantCultureIgnoreCase);
            };
            return SpeechRecognitionEngine.InstalledRecognizers().Where(matchingFunc).FirstOrDefault();
        }

    }
}
*/