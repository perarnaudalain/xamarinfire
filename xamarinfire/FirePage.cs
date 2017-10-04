using Xamarin.Forms;

using SkiaSharp.Views.Forms;
using SkiaSharp;
using System;
using System.Diagnostics;

namespace xamarinfire
{
    /// <summary>
    /// Display fire inside the page.
    /// </summary>
    public class FirePage : ContentPage
    {
        // Canvas
        private SKCanvasView _canvasView;

        // Init done ?
        private bool _initPhase = true;

        // Buffer grayscale
        private uint[] _buffer;

        // Bitmap to blit
        private SKBitmap _bitmap;

        // Information about the screen
        private SKImageInfo _info;

        /// <summary>
        /// Constructor.
        /// </summary>
        public FirePage()
        {
            _canvasView = new SKCanvasView();
            _canvasView.PaintSurface += OnCanvasViewPaintSurface;
            Content = _canvasView;
        }

        /// <summary>
        /// Initialisation des buffers.
        /// </summary>
        private void InitBuffer()
        {
            _buffer = new uint[_info.Width * _info.Height];
            _bitmap = new SKBitmap(_info.Width, _info.Height);
        }

        /// <summary>
        /// launch refresh fire.
        /// </summary>
        private void LaunchFireRefresh()
        {
            // Refresh all 33ms
            Device.StartTimer(TimeSpan.FromMilliseconds(33), () =>
            {
                FireRefresh();
                return true;
            });
        }

        /// <summary>
        /// Refesh fire.
        /// </summary>
        private void FireRefresh()
        {
            // Init seed buffer
            InitSeedBuffer();

            // Grayscale on buffer
            ApplyGrayScaleBuffer();

            // Set the lookuptable
            SetLookUptable();

            // Redraw
            _canvasView.InvalidateSurface();
        }

        /// <summary>
        /// Init the seed buffer.
        /// </summary>
        private void InitSeedBuffer()
        {
            // Fill the buffer
            // seed of fire
            Random rand = new Random();
            int start = (_info.Height - 1) * _info.Width + 200;
            int end = _info.Height * _info.Width - 200;
            for (; start < end; ++start)
            {
                _buffer[start] = (uint)rand.Next(200, 0xFF);
            }
        }

        /// <summary>
        /// Grases the scale buffer.
        /// </summary>
        private void ApplyGrayScaleBuffer()
        {
			// Grayscale
			Random rand = new Random();
			int startprevious = (_info.Height - 1) * _info.Width;
            int startcurrent = (_info.Height - 2) * _info.Width;
            for (int j = _info.Height - 2; j >= 0; --j)
            {
                int startprevioussub = startprevious + 2;
                int startcurrentsub = startcurrent + 2;
                for (int i = 2; i < _info.Width - 2; ++i)
                {
                    // Move fire right or left
                    if ((int)rand.Next(0, 2) == 1)
                    {
                        _buffer[startcurrentsub] =
                            (
                                _buffer[startprevioussub + 1]
                                + _buffer[startprevioussub + 2]
                            ) / 2;
                    }
                    else
                    {
                        _buffer[startcurrentsub] =
                            (
                                _buffer[startprevioussub - 1]
                                + _buffer[startprevioussub - 2]
                            ) / 2;
                    }
                    ++startprevioussub;
                    ++startcurrentsub;
                }
                startprevious -= _info.Width;
                startcurrent -= _info.Width;
            }
        }

        /// <summary>
        /// Sets the look uptable.
        /// </summary>
        private void SetLookUptable() {
			unsafe
			{
				System.IntPtr pt = _bitmap.GetPixels();
				uint* ptinit = (uint*)pt.ToPointer();

                int start = 0;
				for (int j = 0; j < _info.Height; ++j)
				{
					for (int i = 0; i < _info.Width; ++i)
					{
						if (_buffer[start] > 30)
						{
							ptinit[start] = 0xFF000000 + (_buffer[start] << 8) + 0xFF;
						}
						else
						{
							ptinit[start] = 0xFFFFFFFF;
						}
                        ++start;
					}
				}
			}
        }

        /// <summary>
        /// Refresh screen
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="args">Arguments.</param>
        private void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            // Get Frame information : Resolution
            _info = args.Info;

            // We get the information about the size screen
            // We could now init buffer and launch refresh fire
            // Just once time
            if (_initPhase)
            {
                // Initiaze buffer
                InitBuffer();

                // Lance refresh fire
                LaunchFireRefresh();

                // Init phase just once
                _initPhase = false;
            }

            // Clear canvas
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;
            canvas.Clear();

            // Draw fire
            canvas.DrawBitmap(_bitmap, new SKRect(0, 0, _info.Width, _info.Height));
        }
    }
}