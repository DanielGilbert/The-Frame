using CSCore.DSP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace TheFrame.Plugin.Spectogram
{
    public class MatrixSpectrumProvider : SpectrumBase
    {
        private int _matrixWidth;
        private int _matrixHeight;
        private byte[] _oldData;
        public int MatrixWidth
        {
            get
            {
                return _matrixWidth;
            }
            set
            {
                _matrixWidth = value;
            }
        }

        public int MatrixHeight
        {
            get
            {
                return _matrixHeight;
            }
            set
            {
                _matrixHeight = value;
            }
        }

        public MatrixSpectrumProvider(FftSize fftSize, int matrixWidth, int matrixHeight)
        {
            FftSize = fftSize;
            MatrixHeight = matrixHeight;
            MatrixWidth = matrixWidth;
            _oldData = new byte[3 * _matrixWidth * _matrixHeight];
        }

        public byte[] GetDisplayData()
        {
            byte[] data = new byte[3 * _matrixWidth * _matrixHeight];
            
            var fftBuffer = new float[(int)FftSize];

            if (SpectrumProvider.GetFftData(fftBuffer, this))
            {
                SpectrumPointData[] spectrumPoints = CalculateSpectrumPoints(_matrixHeight, fftBuffer);

                foreach (SpectrumPointData spectrumPointData in spectrumPoints)
                {
                    int barIndex = spectrumPointData.SpectrumPointIndex;
                    int value = Convert.ToInt32(Math.Round(spectrumPointData.Value));
                    value = Math.Min(Math.Max(1, value), 7);

                    if (barIndex > _matrixWidth) break;
                    if (value == 0) continue;

                    for (int n = 8; n > 0; n--)
                    {
                        if (n <= value)
                            if (n > 4)
                                data[((n * (-24) + (192)) + (3 * barIndex)) + 1] = 0x07;
                            else
                                data[(n * (-24) + (192)) + (3 * barIndex)] = 0x07;
                        else
                            if (_oldData[((n * (-24) + (192)) + (3 * barIndex)) + 1] > 0x00)
                                data[((n * (-24) + (192)) + (3 * barIndex)) + 1] = (byte)((byte)_oldData[((n * (-24) + (192)) + (3 * barIndex)) + 1] - (byte)0x01);
                    }

                    
                }


                data.CopyTo(_oldData,0);
            }

            return data;
        }
    }
}
