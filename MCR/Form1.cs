using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math.Geometry;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using DirectX.Capture;

namespace MCR
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Bitmap bitmap = new Bitmap("test3.jpg");
            pictureBox1.Image = bitmap;

            capture = new DirectX.Capture.Capture(cameraFilters.VideoInputDevices[1], null);
            capture.FrameSize = new Size(640,480);
            capture.PreviewWindow = picPreview;
            capture.FrameEvent2 += capture_FrameEvent2;
            capture.GrapImg();
            
        }

        private void capture_FrameEvent2(Bitmap e)
        {
            Bitmap full = null;

            lock (doTempImg)
            {
                webcamBitmap = (Bitmap)e.Clone();

                try
                {
                    full = GetCard(webcamBitmap);
                }
                catch { }

                if (full != null)
                {

                    try
                    {
                        full.Save("temp.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                    catch { }
                    picFull.Image = full;
                }
            }
        }

        object doTempImg = new object();
        Bitmap webcamBitmap = null;
        Blob[] blobs = null;
        Pen pen = new Pen(Color.Red, 2);
        Graphics g;
        Bitmap filtered;
        Bitmap bm;
        int lastResult;
        private DirectX.Capture.Capture capture = null;
        private Filters cameraFilters = new Filters();

        string[] m14hashes = File.ReadAllLines(@"m14hashes.txt");

        private List<IntPoint> DetectQuad(Bitmap sourceBitmap, int threshold)
        {
            filtered = Grayscale.CommonAlgorithms.BT709.Apply(sourceBitmap);
            BlobCounter bc = new BlobCounter();

            SobelEdgeDetector edgeFilter = new SobelEdgeDetector();
            edgeFilter.ApplyInPlace(filtered);

            // Threshhold filter
            Threshold threshholdFilter = new Threshold(threshold);
            threshholdFilter.ApplyInPlace(filtered);

            bc.ProcessImage(filtered);
            blobs = bc.GetObjectsInformation();

            SimpleShapeChecker shapechecker = new SimpleShapeChecker();

            bm = new Bitmap(filtered.Width, filtered.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            g = Graphics.FromImage(bm);
            g.DrawImage(filtered, 0, 0, filtered.Width, filtered.Height);
            pictureBox1.Image = bm;
            picOrg.Image = sourceBitmap;

            for (int i = 0; i < blobs.Length; i++)
            {
                List<IntPoint> edges = bc.GetBlobsEdgePoints(blobs[i]);
                List<IntPoint> corners;
                if (edges.Count > 3)
                {
                    if (shapechecker.IsConvexPolygon(edges, out corners))
                    {
                        // get sub-type
                        PolygonSubType subType = shapechecker.CheckPolygonSubType(corners);

                        // Only return 4 corner rectanges
                        if ((subType == PolygonSubType.Parallelogram || subType == PolygonSubType.Rectangle) && corners.Count == 4)
                        {
                            float chkWidth = corners[0].DistanceTo(corners[1]);
                            float chkHeight = corners[1].DistanceTo(corners[2]);
                            float areal = chkWidth * chkHeight;

                            // Hack to avoid small blobs inside the card
                            if (areal < 20000)
                            {
                                continue;
                            }
                            else
                            {
                            }

                            // Flip card if it's the wrong way
                            if (chkWidth > chkHeight)
                            {
                                corners.Insert(4, corners[0]);
                                corners.RemoveAt(0);
                            }

                            return corners;
                        }
                    }
                }
            }
            
            return null;
        }

        // Simple chceks to find the best possible quads
        private int CheckQuad(Bitmap sourceBitmap, List<IntPoint> corners)
        {
            if (corners == null || corners.Count != 4)
            {
                return 999;
            }

            int match = 0;
            double correctCardRatio = 0.716;

            float w1 = corners[0].DistanceTo(corners[1]);
            float w2 = corners[2].DistanceTo(corners[3]);
            float h1 = corners[0].DistanceTo(corners[3]);
            float h2 = corners[1].DistanceTo(corners[2]);

            double cardRatio1 = w1 / h1;
            double cardRatio2 = w2 / h2;
            double cardRatio = (cardRatio1 + cardRatio2) / 2;

            IntPoint d1 = corners[1] - corners[0];
            IntPoint d2 = corners[2] - corners[3];
            IntPoint d3 = corners[3] - corners[0];
            IntPoint d4 = corners[2] - corners[1];
            IntPoint median = corners[2] - corners[0];

            double x1Diff = corners[0].X - corners[3].X;
            double x2Diff = corners[1].X - corners[2].X;
            double y1Diff = corners[0].Y - corners[1].Y;
            double y2Diff = corners[2].Y - corners[3].Y;

            if (x1Diff < 0) { x1Diff = x1Diff * -1; }
            if (x2Diff < 0) { x2Diff = x2Diff * -1; }
            if (y1Diff < 0) { y1Diff = y1Diff * -1; }
            if (y2Diff < 0) { y2Diff = y2Diff * -1; }

            double topDrop = ((double)d1.Y + 1) / ((double)d1.X + 1);
            double bottomDrop = ((double)d2.Y + 1) / ((double)d2.X + 1);
            double leftDrop = ((double)d3.Y + 1) / ((double)d3.X + 1);
            double rightDrop = ((double)d4.Y + 1) / ((double)d4.X + 1);

            // Card ratio must be very close
            if (cardRatio > correctCardRatio + 0.1 || cardRatio < correctCardRatio - 0.1)
            {
                match += 10;
            }
            if (cardRatio > correctCardRatio + 0.15 || cardRatio < correctCardRatio - 0.15)
            {
                match += 20;
            }

            // Card shouldn't be close to the borders
            if (w1 >= sourceBitmap.Width - 100 || w2 >= sourceBitmap.Width - 100)
            {
                match += 10;
            }
            if (h1 >= sourceBitmap.Height - 50 || h2 >= sourceBitmap.Height - 50)
            {
                match += 10;
            }

            // The difference betweem x and y coordinates should be close to each other
            if (x1Diff >= x2Diff + 15 || x1Diff <= x2Diff - 15)
            {
                match += 8;
            }
            if (y1Diff >= y2Diff + 15 || y1Diff <= y2Diff - 15)
            {
                match += 8;
            }

            // The drop between top and bottom should never go in different directions
            if (d1.Y > 0 && d2.Y < 0)
            {
                match += 20;
            }

            // The drop between the top left corner and top right corner, should be fairly close to the drop between the bottom left and right corner
            if (topDrop > bottomDrop + 0.05 || topDrop < bottomDrop - 0.05)
            {
                match += 10;
            }
            // The drop between the top left and bottom right corner, should be fairly close to each other, IF the length between top left and top right corner is less than between bottom left and right corner
            if (leftDrop > rightDrop + 0.05 || leftDrop < rightDrop - 0.05)
            {
                if (w1 >= w2)
                {
                    match += 20;
                }
            }

            // Cards should be either horizontal or vertical
            if (median.Y < 150 && median.Y > -150)
            {
                match += 10;
            }


            return match;
        }

        private Bitmap GetCard(Bitmap sourceBitmap)
        {
            int threshold = 0;
            int.TryParse(txtThreshold.Text, out threshold);
            int bestMatch = 999;
            int bestMatchThreshold = 0;
            List<IntPoint> bestMatchCorners = null;

            List<IntPoint> corners = DetectQuad(sourceBitmap, threshold);

            if (threshold != 0)
            {
                corners = DetectQuad(sourceBitmap, threshold);
                int result = CheckQuad(sourceBitmap, corners);
                txtOutput.Text += "Match is " + result + "\r\n";
            }
            else
            {
                threshold = 210;
                while (threshold > 10)
                {
                    threshold -= 10;
                    corners = DetectQuad(sourceBitmap, threshold);
                    int result = CheckQuad(sourceBitmap, corners);
                    if (result < bestMatch)
                    {
                        //txtOutput.Text += "Image found with threshold " + threshold + "\r\n";
                        bestMatch = result;
                        bestMatchCorners = corners;
                        bestMatchThreshold = threshold;
                        if (result <= 20)
                        {
                            capture.FrameEvent2 -= capture_FrameEvent2;
                            break;
                        }
                    }
                }
            }

            if (bestMatchCorners != null)
            {
                corners = DetectQuad(sourceBitmap, bestMatchThreshold);
                //txtOutput.Text += "Best threshold is " + bestMatchThreshold + "\r\n";
                //txtOutput.Text += "Best match is " + bestMatch + "\r\n";
            }

            // Hack to show what was detected
            try
            {
                g.DrawPolygon(pen, ToPointsArray(corners));

                pictureBox1.Image = bm;

                QuadrilateralTransformation quad = new QuadrilateralTransformation(corners, 241, 346);
                return quad.Apply(sourceBitmap);
            }
            catch
            {
                return null;
            }
        }

        private Bitmap GetCardTopBar(Bitmap cardBitmap)
        {
            List<IntPoint> pointsTopBar = new List<IntPoint>();
            pointsTopBar.Add(new IntPoint(10, 10));
            pointsTopBar.Add(new IntPoint(230, 10));
            pointsTopBar.Add(new IntPoint(230, 25));
            pointsTopBar.Add(new IntPoint(10, 25));

            QuadrilateralTransformation quadTopBar = new QuadrilateralTransformation(pointsTopBar, 220, 16);
            return quadTopBar.Apply(cardBitmap);
        }

        private Bitmap GetCardTypeBar(Bitmap cardBitmap)
        {
            List<IntPoint> pointsTypeBar = new List<IntPoint>();
            pointsTypeBar.Add(new IntPoint(10, 198));
            pointsTypeBar.Add(new IntPoint(230, 198));
            pointsTypeBar.Add(new IntPoint(230, 213));
            pointsTypeBar.Add(new IntPoint(10, 213));

            QuadrilateralTransformation quadTypeBar = new QuadrilateralTransformation(pointsTypeBar, 220, 16);
            return quadTypeBar.Apply(cardBitmap);
        }

        private Bitmap GetCardArt(Bitmap cardBitmap)
        {
            List<IntPoint> pointsArt = new List<IntPoint>();
            pointsArt.Add(new IntPoint(10, 31));
            pointsArt.Add(new IntPoint(230, 31));
            pointsArt.Add(new IntPoint(230, 193));
            pointsArt.Add(new IntPoint(10, 193));

            QuadrilateralTransformation quadArt = new QuadrilateralTransformation(pointsArt, 220, 162);
            return quadArt.Apply(cardBitmap);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            picTopBar.Image = null;
            picTypeBar.Image = null;
            picArt.Image = null;
            picFull.Image = null;

            Bitmap full = GetCard(new Bitmap(txtFilename.Text));

            if (full != null)
            {
                picTopBar.Image = GetCardTopBar(full);
                picTypeBar.Image = GetCardTypeBar(full);
                picArt.Image = GetCardArt(full);
                picFull.Image = full;

                full.Save("temp.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            UInt64 test = Phash.ImageHash("temp.jpg");

            txtOutput.Text += test;
        }

        // Conver list of AForge.NET's points to array of .NET points
        private System.Drawing.Point[] ToPointsArray(List<IntPoint> points)
        {
            System.Drawing.Point[] array = new System.Drawing.Point[points.Count];

            for (int i = 0, n = points.Count; i < n; i++)
            {
                array[i] = new System.Drawing.Point(points[i].X, points[i].Y);
            }

            return array;
        }
        
        public class Phash
        {

            [DllImport("pHash.dll", CallingConvention = CallingConvention.Cdecl)]
            private static extern int ph_dct_imagehash(string file_name, ref UInt64 hash);

            public static UInt64 ImageHash(string fileName)
            {
                UInt64 _result = 0;
                ph_dct_imagehash(fileName, ref _result);

                return _result;
            }

            private static UInt64 m1 = 0x5555555555555555;
            private static UInt64 m2 = 0x3333333333333333;
            private static UInt64 h01 = 0x0101010101010101;
            private static UInt64 m4 = 0x0f0f0f0f0f0f0f0f;

            // Calculate the similarity between two hashes
            public static int HammingDistance(UInt64 hash1, UInt64 hash2)
            {
                UInt64 x = hash1 ^ hash2;

                x -= (x >> 1) & m1;
                x = (x & m2) + ((x >> 2) & m2);
                x = (x + (x >> 4)) & m4;
                UInt64 res = (x * h01) >> 56;

                return (int)res;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            lock (doTempImg)
            {
                UInt64 _card = Phash.ImageHash("temp.jpg");
                int _bestMatch = 999;
                string _bestMatchFilename = "";

                foreach (string str in m14hashes)
                {
                    string[] s = str.Split('|');

                    int _result = Phash.HammingDistance(_card, UInt64.Parse(s[1]));

                    if (_result < _bestMatch)
                    {
                        _bestMatch = _result;
                        _bestMatchFilename = s[0];
                    }
                }
                pictureBox1.Image = new Bitmap(_bestMatchFilename);
                if (_bestMatch > 9 && _bestMatch < 17)
                {
                    txtOutput.Text = string.Format("Best match: {0} (shaky match)\r\nFilename: {1}", _bestMatch, _bestMatchFilename);
                }
                else if (_bestMatch >= 17)
                {
                    txtOutput.Text = string.Format("Best match: {0} (crap match)\r\nFilename: {1}", _bestMatch, _bestMatchFilename);
                }
                else
                {
                    txtOutput.Text = string.Format("Best match: {0}\r\nFilename: {1}", _bestMatch, _bestMatchFilename);
                }
            }
        }

        private void DownloadCards()
        {
            Regex regex = new Regex(@"(?<==.)(.*)(?=.jpg.>).jpg");
            List<string> images = new List<string>();

            WebClient wc = new WebClient();
            string site = wc.DownloadString("http://mythicspoiler.com/m14/cards/");

            string[] result = site.Split('\n');

            foreach (string r in result)
            {
                Match test = regex.Match(r);
                if (test.Success)
                {
                    images.Add(test.Value);
                    wc.DownloadFile(string.Format("http://mythicspoiler.com/m14/cards/{0}", test.Value), @"C:\Users\Bruger\Documents\Visual Studio 2010\Projects\MCR\MCR\bin\Debug\temp\" + test.Value);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string[] files = Directory.GetFiles("M14");

            foreach (string f in files)
            {
                Bitmap card = GetCard(new Bitmap(f));
                pictureBox1.Image = card;
                
                card.Save("temp.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                txtOutput.Text += f + "|" + Phash.ImageHash("temp.jpg") + "\r\n";
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            capture.FrameEvent2 += capture_FrameEvent2;

            Bitmap full = GetCard(webcamBitmap);

            if (full != null)
            {
                picFull.Image = full;

                full.Save("temp.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
            }
        }
    }
}
