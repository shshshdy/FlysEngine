﻿using FlysEngine;
using FlysEngine.Desktop;
using FlysEngine.Managers;
using Vortice.Direct2D1;
using Vortice.DirectWrite;
using Vortice.Mathematics;
using FlowDirection = Vortice.DirectWrite.FlowDirection;
using FontStyle = Vortice.DirectWrite.FontStyle;

namespace FlysTest
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var res = new XResource())
            using (var form = new Form() { Text = "Hello World", MaximizeBox = false, Size = new System.Drawing.Size(800, 600) })
            {
                var timer = new RenderTimer();
                IDWriteTextFormat bottomRightFont = res.DWriteFactory.CreateTextFormat("Consolas", 16.0f);
                bottomRightFont.FlowDirection = FlowDirection.BottomToTop;
                bottomRightFont.TextAlignment = TextAlignment.Trailing;

                IDWriteTextFormat bottomLeftFont = res.DWriteFactory.CreateTextFormat("Consolas", FontWeight.Normal, FontStyle.Italic, FontStretch.Normal, 24.0f);
                bottomLeftFont.FlowDirection = FlowDirection.BottomToTop;
                bottomLeftFont.TextAlignment = TextAlignment.Leading;

                form.Resize += (o, e) =>
                {
                    if (form.WindowState != FormWindowState.Minimized && res.DeviceAvailable)
                    {
                        res.Resize();
                    }
                };

                var manager = new BitmapManager(new Vortice.WIC.IWICImagingFactory());

                RenderLoop.Run(form, () => Render());

                void Render()
                {
                    if (!res.DeviceAvailable) res.InitializeDevice(form.Handle);

                    var target = res.RenderTarget;

                    timer.BeginFrame();
                    target.BeginDraw();
                    manager.SetRenderTarget(target);
                    Draw(target);
                    target.EndDraw();
                    res.SwapChain.Present(1, 0);
                    timer.EndFrame();
                }

                void Draw(ID2D1DeviceContext target)
                {
                    target.Clear(Colors.CornflowerBlue);
                    Rect rectangle = new(0, 0, target.Size.Width, target.Size.Height);

                    target.DrawRectangle(
                        new Rect(10, 10, target.Size.Width - 20, target.Size.Height - 20),
                        res.GetColor(Colors.Blue));

                    target.DrawText("😀😁😂🤣😃😄😅😆😉😊😋😎",
                        res.TextFormats[36], rectangle, res.GetColor(Colors.Blue),
                        DrawTextOptions.EnableColorFont);

                    target.DrawText("FPS: " + timer.FramesPerSecond.ToString("F1"),
                        bottomRightFont, rectangle, res.GetColor(Colors.Red));

                    target.DrawText("Hello World",
                        bottomLeftFont, rectangle, res.GetColor(Colors.Purple));
                    
                    var bitmap = manager[Path.Combine(AppContext.BaseDirectory, "temp", "test.png")];
                    target.DrawBitmap(bitmap);

                }
            }
        }
    }
}