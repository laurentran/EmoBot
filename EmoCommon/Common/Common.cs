using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emo.Common
{
    public enum EmotivEmotion { NEUTRAL, ANGRY, HAPPY };

    public enum SpheroColor { RED, GREEN, WHITE };

    public static class Common
    {
    }

    public class EmotionChangedEventArgs : EventArgs
    {
        public EmotivEmotion Emotion { get; set; }
    }
}
