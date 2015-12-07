using Emo.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Emo.Data.DTO;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using Emotiv;
using System.Diagnostics;

namespace EmotivController.Data
{
    // This class represents the state of the device
    public class EmotivDevice : IEmotiv
    {
        private EmoEngine _engine; // Access to the EDK is viaa the EmoEngine 
        private int _userID = -1; // userID is used to uniquely identify a user's headset
        private const double TARGET_SMILE_VALUE = 0.1; //change 50
        private const double TARGET_FROWN_VALUE = 0.2;
        private EmotivEmotion _emotion = EmotivEmotion.NEUTRAL;


        public EmotivDevice()
        {
            // create the engine
            _engine = EmoEngine.Instance;
            _engine.UserAdded += new EmoEngine.UserAddedEventHandler(engine_UserAdded_Event);

            _engine.EmoEngineConnected += (s, e) =>
            {
                Debug.WriteLine("emotiv connected");
            };

            _engine.EmoEngineDisconnected += (s, e) =>
            {
                Debug.WriteLine("emotiv disconnected");
            };

            // emotiv state has changed
            _engine.EmoStateUpdated += (s, e) =>
            {
                               

                var smileExtent = e.emoState.ExpressivGetLowerFaceActionPower();
                var frownExtent = e.emoState.ExpressivGetUpperFaceActionPower();
                if (smileExtent > frownExtent)
                {
                    if (smileExtent >= TARGET_SMILE_VALUE)
                    {
                        Magnitude = smileExtent;
                        Emotion = EmotivEmotion.HAPPY;
                    }
                    else if (frownExtent >= TARGET_FROWN_VALUE)
                    {
                        Magnitude = frownExtent;
                        Emotion = EmotivEmotion.ANGRY;

                    }
                    else
                    {
                        Magnitude = 0;
                        Emotion = EmotivEmotion.NEUTRAL;
                    }
                }
                else if (smileExtent <= frownExtent)
                {
                    if (frownExtent >= TARGET_FROWN_VALUE)
                    {
                        Magnitude = frownExtent;
                        Emotion = EmotivEmotion.ANGRY;

                    }
                    else if (smileExtent >= TARGET_SMILE_VALUE)
                    {
                        Magnitude = smileExtent;
                        Emotion = EmotivEmotion.HAPPY;
                    }
                    
                    else
                    {
                        Magnitude = 0;
                        Emotion = EmotivEmotion.NEUTRAL;
                    }
                }
                //var smileExtent = e.emoState.ExpressivGetLowerFaceActionPower();
                //var frownExtent = e.emoState.ExpressivGetUpperFaceActionPower();
                //if (smileExtent >= TARGET_SMILE_VALUE)
                //{
                //        Magnitude = smileExtent;
                //        Emotion = EmotivEmotion.HAPPY;
                //}
                //else if (frownExtent >= TARGET_FROWN_VALUE)
                //{
                //        Magnitude = frownExtent;
                //        Emotion = EmotivEmotion.ANGRY;

                //}
                //else
                //{
                //        Magnitude = 0;
                //        Emotion = EmotivEmotion.NEUTRAL;
                //}
            };


        }

        // This is just a test object, once we setup the emotiv we won't need it
        private Timer _testTimer = null;

        // This is a test object
        private Random _rnd = new Random();

        // A property that is our emotion at any given time
        public EmotivEmotion Emotion
        {
            get
            {
                return _emotion;
            }
            set
            {
                if(_emotion == value)
                {
                    return;
                }
                _emotion = value;
                OnEmotionChanged();
            }
        }

        public double Magnitude { get; set; }

        // An event to raise when our emotion changes.
        public event EventHandler<EmotionChangedEventArgs> EmotionChanged = (s, e) => { };


        // This will just kick off a timer for now and raise an event with a random emotion.
        // TODO: Add actual listening to emotiv here and set the right emotion.
        public void Start()
        {
            // connect to EmoEngine
            _engine.Connect();

            // For now, just kick our our timer with a random tick
            _testTimer = new Timer(
                o =>
                {
                    _engine.ProcessEvents();
                },
                null, 0, 1000
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

        // code from console app
        void engine_UserAdded_Event(object sender, EmoEngineEventArgs e)
        {
            Debug.WriteLine("User Added Event has occured");

            // record the user 
            _userID = (int)e.userId;

            // TODO: figure out why userID always needs to be 0
            // HACK
            _userID = 0; 

            // enable data aquisition for this user.
            _engine.DataAcquisitionEnable((uint)_userID, true);

            // ask for up to 1 second of buffered data
            _engine.EE_DataSetBufferSizeInSec(1);
        }
    }
}
