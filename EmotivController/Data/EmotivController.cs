using Emo.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EmotivController.Data
{
    public class EmotivController
    {
        public event EventHandler<EmotionChangedEventArgs> EmotionChanged = (s, e) => { };
     
        // This will represent the state of our emotiv
        EmotivDevice _emotiv = new EmotivDevice();

        // This is for testing.
        Random _rnd = new Random();

        // The name of our controller so we can talk about it.
        public string Name { get; set; }

        public EmotivController()
        {
            // Give us a random name.
            Name = "Emotive Controller " + _rnd?.Next(0, 100).ToString() ?? "Ooops! Why'd you do that?";

            // Start listenting to our Emotiv object when it's emotion changes
            if( _emotiv != null )
            {
                _emotiv.EmotionChanged += OnEmotionChanged;
            }
        }

        // Make it start talking.
        public void StartListening()
        {
            _emotiv.Start();
        }

        // Make it stop talking
        public void StopListening()
        {
            _emotiv.Stop();
        }

        // Raise an event when the emotion of the Emotiv changes.
        protected virtual void OnEmotionChanged( object sender, EmotionChangedEventArgs e)
        {
            var source = sender as EmotivDevice;
            if( sender == null )
            {
                Debug.Assert(false);
                return;
            }
            // This is only necessary if want to do some preprocessing on the data (some business logic).
            ProcessEmotiveData( source );

            // Raise the actual event (notify anyone who is listening).
            EmotionChanged.Raise(source, new EmotionChangedEventArgs() { Emotion = _emotiv.Emotion });
        }

        private void ProcessEmotiveData(EmotivDevice dataSource)
        {
            //TODO: add any data processing here.
            // Not necessary for now.
        }
    }
}
