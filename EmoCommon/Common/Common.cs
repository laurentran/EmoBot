using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emo.Common
{
    public enum EmotivEmotion { ANGRY, HAPPY };
    public static class Common
    {
    }

    public class EmotionChangedEventArgs : EventArgs
    {
        public EmotivEmotion Emotion { get; set; }
    }
}
