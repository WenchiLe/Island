using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;

namespace Island
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct StPoint
    {
        public Int64 X, Y;
        public StPoint(Int64 _x, Int64 _y) { X = _x; Y = _y; }
    }

    public class ImageWarp
    {
        // using System.Drawing;
        // using System.Drawing.Imaging;

        public Bitmap bmp;
        public Graphics gph;

        public ImageWarp(Int32 width, Int32 height)
        {
            bmp = new Bitmap(width, height);
            gph = Graphics.FromImage(bmp);
            //
            gph.Clear(Color.Empty);
        }

        public void DrawPoint(StPoint pt, Color color, Single thickness = 1.0f)
        {
            gph.DrawEllipse(new Pen(color, thickness), (int)pt.X - thickness * 0.5f, (int)pt.Y - thickness * 0.5f, thickness, thickness);
            //gph.FillEllipse();
        }

        public void DrawLine(StPoint p0, StPoint p1, Color color, Single thickness = 1.0f)
        {
            gph.DrawLine(new Pen(color, thickness), p0.X, p0.Y, p1.X, p1.Y);
        }

        public void Save(string filename)
        {
            gph.Save();
            gph.Dispose();
            bmp.MakeTransparent(Color.Transparent);
            bmp.Save(filename, ImageFormat.Png);
        }

        public static void Demo01()
        {
            ImageWarp image = new ImageWarp(512, 512);

            image.DrawLine(new StPoint(256, 0), new StPoint(256, 511), Color.Blue, 1.0f);
            image.DrawLine(new StPoint(0, 256), new StPoint(511, 256), Color.Red, 1.0f);
            image.DrawPoint(new StPoint(256, 256), Color.Green, 2.0f);
            image.DrawPoint(new StPoint(0, 0), Color.Black, 10.0f);

            image.Save("./image/ImageWarp.Demo01.png");
        }

        public static void DrawIsland(int num,int[,] map)
        {
            ImageWarp image = new ImageWarp(2048, 1024);
            for (int x = 0; x < 2048; x++)
            {
                for (int y = 0; y < 1024; y++)
                {
                    //if (map[x, y] > 0)
                    //{
                        image.DrawPoint(new StPoint(x, y), Color.FromArgb(map[x, y], map[x, y], map[x, y]), 0.3f);
                    //}
                    //else {
                    //    image.DrawPoint(new StPoint(x, y), Color.White, 0.3f);
                    //}
                    
                }
            }
            string filename = "./image/"+ num +".png";
            image.Save(filename);
        }
    }
}
