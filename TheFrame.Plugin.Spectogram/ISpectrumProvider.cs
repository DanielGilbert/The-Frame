using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheFrame.Plugin.Spectogram
{
    public interface ISpectrumProvider
    {
        bool GetFftData(float[] fftBuffer, object context);
        int GetFftBandIndex(float frequency);
    }
}
