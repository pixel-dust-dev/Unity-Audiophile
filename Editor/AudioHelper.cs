using UnityEngine;

namespace PixelDust.Audiophile
{
    public class AudioHelper
    {
        public static float LinearToDecibel(float linear)
        {
            float dB;

            if (linear != 0)
            {
                dB = 20.0f * Mathf.Log10(linear);
            }
            else
            {
                dB = -144.0f;
            }

            return dB;
        }

        public static float DecibelToLinear(float dB)
        {
            float linear;

            if (dB == -144.0f)
            {
                linear = 0;
            }
            else
            {
                linear = Mathf.Pow(10.0f, dB / 20.0f);
            }

            return linear;
        }
    }
}