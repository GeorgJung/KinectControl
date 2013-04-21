using System;
using System.Linq;
using Microsoft.Kinect;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;
using System.IO;


namespace Mechanect.Common
{
    public class VoiceCommands
    {

        KinectAudioSource kinectAudio;
        SpeechRecognitionEngine speechRecognitionEngine;
        Stream stream;
        readonly KinectSensor kinect;
        string heardString = " ";

        public VoiceCommands(KinectSensor kinect, string commands)
        {
            this.kinect = kinect;
            InitalizeKinectAudio(commands);
        }

        private void InitalizeKinectAudio(string commands)
        {
            string[] arrayOfCommands = commands.Split(',');
            RecognizerInfo recognizerInfo = GetKinectRecognizer();
            if(recognizerInfo != null)
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

        public bool GetHeard(string expectedString)
        {
            return expectedString.Equals(heardString);
        }

        private void SpeechRecognitionEngineSpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Confidence >= 0.55)
                heardString = e.Result.Text;
        }

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
