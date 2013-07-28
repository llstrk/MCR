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
        }

        Blob[] blobs = null;
        Pen pen = new Pen(Color.Red, 2);
        Graphics g;
        Bitmap filtered;
        Bitmap bm;

        private void button1_Click(object sender, EventArgs e)
        {
            picTopBar.Image = null;
            picTypeBar.Image = null;
            picArt.Image = null;
            picFull.Image = null;

            Bitmap bitmap = new Bitmap(txtFilename.Text);
            filtered = Grayscale.CommonAlgorithms.BT709.Apply(bitmap);
            BlobCounter bc = new BlobCounter();

            SobelEdgeDetector edgeFilter = new SobelEdgeDetector();
            edgeFilter.ApplyInPlace(filtered);

            // Threshhold filter
            Threshold threshholdFilter = new Threshold(int.Parse(txtThreshold.Text));
            threshholdFilter.ApplyInPlace(filtered);

            bc.ProcessImage(filtered);
            blobs = bc.GetObjectsInformation();

            SimpleShapeChecker shapechecker = new SimpleShapeChecker();

            bm = new Bitmap(filtered.Width, filtered.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            g = Graphics.FromImage(bm);
            g.DrawImage(filtered, 0, 0, filtered.Width, filtered.Height);
            pictureBox1.Image = bm;
            picOrg.Image = bitmap;

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
                            if (areal < 1000)
                            {
                                continue;
                            }

                            // Flip card if it's the wrong way
                            if (chkWidth > chkHeight)
                            {
                                corners.Insert(4, corners[0]);
                                corners.RemoveAt(0);
                            }

                            QuadrilateralTransformation quad = new QuadrilateralTransformation(corners, 241, 346);
                            Bitmap full = quad.Apply(bitmap);

                            List<IntPoint> pointsTopBar = new List<IntPoint>();
                            pointsTopBar.Add(new IntPoint(10, 10));
                            pointsTopBar.Add(new IntPoint(230, 10));
                            pointsTopBar.Add(new IntPoint(230, 25));
                            pointsTopBar.Add(new IntPoint(10, 25));

                            QuadrilateralTransformation quadTopBar = new QuadrilateralTransformation(pointsTopBar, 220, 16);
                            Bitmap topBar = quadTopBar.Apply(full);

                            List<IntPoint> pointsTypeBar = new List<IntPoint>();
                            pointsTypeBar.Add(new IntPoint(10, 198));
                            pointsTypeBar.Add(new IntPoint(230, 198));
                            pointsTypeBar.Add(new IntPoint(230, 213));
                            pointsTypeBar.Add(new IntPoint(10, 213));

                            QuadrilateralTransformation quadTypeBar = new QuadrilateralTransformation(pointsTypeBar, 220, 16);
                            Bitmap typeBar = quadTypeBar.Apply(full);

                            List<IntPoint> pointsArt = new List<IntPoint>();
                            pointsArt.Add(new IntPoint(10, 31));
                            pointsArt.Add(new IntPoint(230, 31));
                            pointsArt.Add(new IntPoint(230, 193));
                            pointsArt.Add(new IntPoint(10, 193));

                            QuadrilateralTransformation quadArt = new QuadrilateralTransformation(pointsArt, 220, 162);
                            Bitmap art = quadArt.Apply(full);

                            g.DrawPolygon(pen, ToPointsArray(corners));

                            pictureBox1.Image = bm;

                            picTopBar.Image = topBar;
                            picTypeBar.Image = typeBar;
                            picArt.Image = art;
                            picFull.Image = full;
                        }
                    }
                }
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {

            // Threshhold filter
            Threshold threshholdFilter = new Threshold(190);
            threshholdFilter.ApplyInPlace(filtered);
            g.DrawImage(filtered, 0, 0, filtered.Width, filtered.Height);
            pictureBox1.Image = bm;
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
    }
}
