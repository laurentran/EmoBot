using Emo.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Emo.Data.DTO;
using System.Threading.Tasks;

namespace EmotivController.Data
{
    // This class represens the state of the device
    public class Emotiv : IEmotiv
    {
        // This is just a test object, once we setup the emotiv we won't need it
        private Timer _testTimer;

        // This is a test object
        private Random _rnd = new Random();

        // A property that is our emotion at any given time
        public EmotivEmotion Emotion { get; set; }

        // An event to raise when our emotion changes.
        public event EventHandler<EmotionChangedEventArgs> EmotionChanged = (s, e) => { };

        // This will just kick off a timer for now and raise an event with a random emotion.
        // TODO: Add actual listening to emotiv here and set the right emotion.
        public void Start()
        {
            // For now, just kick our our timer with a random tick
            _testTimer = new Timer(
                o =>
                {
                    this.Emotion = (_rnd.Next(2) == 0) ? EmotivEmotion.ANGRY : EmotivEmotion.HAPPY;
                    OnEmotionChanged();
                },
                null, 0, 2000
                );
        }

        public void Stop()
        {
            // for now, we will just disabel our test timer
            _testTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        // Raise the event.
        protected virtual void OnEmotionChanged()
        {
            EmotionChanged.Raise(this, new EmotionChangedEventArgs() { Emotion = this.Emotion });
        }
    }
}
