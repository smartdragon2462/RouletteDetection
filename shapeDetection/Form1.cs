using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.CvEnum;
using System.IO;
using System.Threading;
using System.Windows.Media.Imaging;
using System.Net;

namespace shapeDetection
{

    public partial class Roulette_main : Form
    {

        /// <summary>
        /// ////////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        public class old_data_info
        {
            public PointF centerOfClr = new PointF();
            public double radiusOfClr = 0;
            public double criteraAngle = 0;

            public old_data_info(PointF _center, double _radius, double _angle)
            {
                centerOfClr = _center;
                radiusOfClr = _radius;
                criteraAngle = _angle;
            }

            public void set(PointF _center, double _radius, double _angle)
            {
                centerOfClr = _center;
                radiusOfClr = _radius;
                criteraAngle = _angle;
            }

        }

        ////////////////////////////////////////////////////////////////////////////////////////////
        public class ball_info
        {
            public double angle = 0;
            public int frame_index = 0;
            public double changedAngle = 0;
            public double initial_LineAngle = 0;
            public double current_LineAngle = 0;

            public ball_info(double _angle, int _index)
            {
                changedAngle = _angle;
                frame_index = _index;
            }


            public ball_info(double _angle, int _index, double _criteraAng, double initeAng, double currentAng)
            {
                angle = _angle;
                frame_index = _index;
                initial_LineAngle = initeAng;
                current_LineAngle = currentAng;

                changedAngle = angle_calculte(_angle, _criteraAng, initeAng, currentAng);
            }

            public double angle_calculte(double _angle, double _criteraAng, double initeAng, double currentAng)
            {
                double real_Angle = 0;
                double delta = initeAng - currentAng;

                if (_angle - (_criteraAng- delta) < 0)
                {
                    real_Angle = 360 + (_angle - (_criteraAng- delta));
                }else
                {
                    real_Angle = _angle - (_criteraAng- delta);
                }

                return real_Angle;
            }

            public void set(double _angle, int _index)
            {
                angle = _angle;
                frame_index = _index; ;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////
        public class ball_history
        {
            public List<ball_info> ball;
            public List<double> angle_history;
            public List<int> frameCount_history;

            public ball_history()
            {
                ball = new List<ball_info>();
            }

            public void historyData_process()
            {
                angle_history.Clear();
                frameCount_history.Clear();

                for (int n = 0; n< ball.Count; n++)
                {
                    angle_history.Add(ball[n].angle);
                    frameCount_history.Add(ball[n].frame_index);
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////
        bool roulette_processing_flag = false;
        double initial_criteraAngle = 0;
        double St_Angle = 0;
        //int f_count = 0;
        double old_greenAngle = 0;
        double green_deltaAngle = 0;
        bool first_frame_flag = false;
        //int time_count = 0;

        private int _X, _Y, _W, _H;
        Form fm2;

       // ball_history ball_Obj = new ball_history();
        old_data_info old_data;
    
        //List<double> green_angles = new List<double>();
        //List<int> frame_count = new List<int>();

        Image<Bgr, Byte> old_frame;
        Image<Bgr, byte> display_img = new Image<Bgr, byte>(0, 0, new Bgr(0, 0, 0));

        ////////////////////////////////////////////////////////////////////////////////////////////
        public Roulette_main()
        {
            InitializeComponent();
        }

  
        ///////////////////// initialize the pictureBox to display angle ////////////////////////////////////////////////
        private void initial_displayAngle()
        {
            //-----------------------------------------------------------------
            display_img = new Image<Bgr, byte>(500, 500, new Bgr(255, 255, 255));
            display_img.Draw(new CircleF(new PointF(250, 250), 150), new Bgr(100, 0, 255), 3);
            display_img.Draw(new LineSegment2DF(new PointF(250.0f, 50.0f), new PointF(250, 450)), new Bgr(10, 10, 10), 1);
            display_img.Draw(new LineSegment2DF(new PointF(250.0f, 250.0f), new PointF(250.0f, 100.0f)), new Bgr(50, 255, 50), 10);

            MCvFont f = new MCvFont(Emgu.CV.CvEnum.FONT.CV_FONT_HERSHEY_COMPLEX, 1.2, 1.2);
            display_img.Draw("0.0", ref f, new Point(250, 90), new Bgr(255, 0, 255));
            CvInvoke.cvCircle(display_img, new Point(250, 250), 3, new MCvScalar(255, 0, 100), -1, LINE_TYPE.CV_AA, 0);

            pic_visualAngle.Image = display_img.ToBitmap();
        }

       
        /////////////////////////// to detect the roulette ////////////////////////////////////////////////////////////////////
        private void Roulett_recognition(Image<Bgr, byte> frame)
        {
            //account a frame count
            //f_count ++;

            // convert rgb to gray
            Image<Gray, byte> grayImg = frame.Convert<Gray, byte>();
            Image<Bgr, byte> resize_RGB_img = frame.Resize(frame.Width / 3, frame.Height / 3, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);
            Image<Bgr, byte> out_frame = resize_RGB_img.Copy();

            // convert gray to bw
            Image<Gray, byte> bwImg = grayImg.CopyBlank();
            CvInvoke.cvThreshold(grayImg, bwImg, 135, 255, Emgu.CV.CvEnum.THRESH.CV_THRESH_BINARY);

            //----------------------------------------------------------------------------
            Image<Gray, byte> resize_bwImg = bwImg.Resize(bwImg.Width / 3, bwImg.Height / 3, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);
            //pic_videoDisplay.Image = bwImg.ToBitmap();

            //----------------------------------------------------------------------------
            if (!first_frame_flag)
            {
                first_frame_flag = true;
                old_frame = resize_RGB_img.Copy();
                //----------------------------------------------------------------------------
                double criteraAngle = 0;
                PointF centerOfCircle;
                double radius = 0;

                find_circle(resize_RGB_img, resize_bwImg, out centerOfCircle, out radius);
                find_CriteriaAngle(resize_bwImg, centerOfCircle, radius, out criteraAngle);
                initial_criteraAngle = criteraAngle;
                lbl_statue.Text = "Starting ...";

                old_data = new old_data_info(centerOfCircle, radius, initial_criteraAngle);
            }
            else
            {
                //----------------------------------------------------------------------------
                double criteraAngle = 0;
                PointF centerOfCircle;
                double radius = 0;

                //----------------------------------------------------------------------------
                find_circle(resize_RGB_img, resize_bwImg, out centerOfCircle, out radius);
                find_CriteriaAngle(resize_bwImg, centerOfCircle, radius, out criteraAngle);

                display_ballInitialLine(out_frame, centerOfCircle, radius, St_Angle, initial_criteraAngle, criteraAngle, out out_frame);

                //------------------------------------------------------------------------
                CvInvoke.cvCircle(out_frame, new Point((int)centerOfCircle.X, (int)centerOfCircle.Y), 5, new MCvScalar(255, 0, 255), -1, LINE_TYPE.CV_AA, 0);

                if (!double.IsNaN(radius))
                {
                    CvInvoke.cvCircle(out_frame, new Point((int)centerOfCircle.X, (int)centerOfCircle.Y), (int)radius, new MCvScalar(0, 255, 255), 2, LINE_TYPE.CV_AA, 0);
                }

                St_Angle = 270;

                double greenAng = 0;
                double delta = initial_criteraAngle - criteraAngle;
                Image<Bgr, byte> display_img1 = display_img.Copy();

                //------------------------------------------------------------------------------------
                if (radius != 0)
                {
                    diplay_Angle(resize_RGB_img, St_Angle, delta, old_data.centerOfClr, old_data.radiusOfClr, out display_img1, out greenAng);
                    //Application.DoEvents();
                    //------------------------------------------------------------------------------------
                    if (greenAng != -100000)
                    {
                        //green_angles.Add(greenAng);
                        display_img = display_img1.Copy();
                        green_deltaAngle = greenAng - old_greenAngle;

                        //if (old_greenAngle - greenAng < -10 && Math.Abs(old_greenAngle - greenAng) < 200)
                        //{
                        //    //Application.DoEvents();
                        //    int ii = 1;
                        //}

                        old_greenAngle = greenAng;
                    }
                    else
                    {
                        diplay_Angle1(resize_RGB_img, St_Angle, delta, old_greenAngle, green_deltaAngle, out display_img1, out greenAng);
                        old_greenAngle = greenAng;
                    }

                    old_data.set(centerOfCircle, radius, St_Angle);
                }
                else
                {
                    diplay_Angle1(resize_RGB_img, St_Angle, delta, old_greenAngle, green_deltaAngle, out display_img1, out greenAng);

                    //green_deltaAngle = greenAng - old_greenAngle;
                    old_greenAngle = greenAng;
                }
            }

            old_frame = resize_RGB_img.Copy();
            pic_visualAngle.Image = display_img.ToBitmap();
            pic_videoDisplay.Image = out_frame.ToBitmap();
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void green_criteriaStart_Angle(Image<Bgr, byte> frame, PointF centerOfCircle, double radius, out double real_Angle)
        {
            real_Angle = 0;

            if ((int)(centerOfCircle.X - radius * 1.6) + (int)(radius * 2 * 1.6) >= frame.Width || (int)(centerOfCircle.Y - radius * 1.6) + (int)(radius * 2 * 1.6) >= frame.Height)
                return;

            Image<Bgr, byte> crop_BGR_plate = frame.Copy(new Rectangle((int)(centerOfCircle.X - radius * 1.6), (int)(centerOfCircle.Y - radius * 1.6), (int)(radius * 2 * 1.6), (int)(radius * 2 * 1.6)));
            Image<Gray, byte> green_img = crop_BGR_plate.InRange(new Bgr(50, 100, 110), new Bgr(110, 255, 120));

            //---------------------------------------------------------
            green_img = bwareaopen(green_img, 10);
            green_img._Dilate(2);
            green_img = FillHoles(green_img);

            //pic_visualAngle.Image = green_img.ToBitmap();
            //Application.DoEvents();

            //**************************************************************************
            double max_area = 0;
            PointF green_Obj_center = new PointF();
            using (MemStorage storage = new MemStorage())
            {
                for (Contour<Point> contours = green_img.Copy().FindContours(); contours != null; contours = contours.HNext)
                {
                    double area = contours.Area;
                    if (max_area < area)
                    {
                        max_area = area;
                        green_Obj_center = contours.GetMinAreaRect().center;
                    }
                }
            }


            //--------------------------------------------------------------------
            if (max_area < 10)    {       return;         }

            double tmp_ang = 180.0 / Math.PI * Math.Atan2((green_Obj_center.Y - (int)(radius * 1.6)), (green_Obj_center.X - (int)(radius * 1.6)));

            if (tmp_ang >= 0 && tmp_ang <= 180)
            {
                real_Angle = tmp_ang ;
            }
            else if (tmp_ang < 0)
                real_Angle = 360  + tmp_ang ;

        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void diplay_Angle(Image<Bgr, byte> frame,double initeAngle, double delta, PointF centerOfCircle, double radius, out Image<Bgr, byte> display_img, out double greenAng)
        {
            greenAng = 0;

            display_img = new Image<Bgr, byte>(500, 500, new Bgr(255, 255, 255));
            if ((int)(centerOfCircle.X - radius * 1.6) + (int)(radius * 2 * 1.6) >= frame.Width || (int)(centerOfCircle.Y - radius * 1.6) + (int)(radius * 2 * 1.6) >= frame.Height) return;

            Image<Bgr, byte> crop_BGR_plate = frame.Copy(new Rectangle((int)(centerOfCircle.X - radius * 1.6), (int)(centerOfCircle.Y - radius * 1.6), (int)(radius * 2 * 1.6), (int)(radius * 2 * 1.6)));
            Image<Gray, byte> green_img = crop_BGR_plate.InRange(new Bgr(0, 100, 0), new Bgr(220, 255, 105));

            //---------------------------------------------------------
            green_img = bwareaopen(green_img, 20);
            //green_img._Dilate(2);
            //green_img = FillHoles(green_img);
            //green_img.Save("1.png");
            //pic_videoDisplay.Image = green_img.ToBitmap();
            //Application.DoEvents();

            //**************************************************************************

            double max_area = 0;
            PointF green_Obj_center = new PointF();
            using (MemStorage storage = new MemStorage())
            {
                for (Contour<Point> contours = green_img.Copy().FindContours(); contours != null; contours = contours.HNext)
                {
                    double area = contours.Area;
                    PointF tmp = contours.GetMinAreaRect().center;
                    double m_distance = Math.Sqrt(Math.Pow(tmp.X - (int)(radius * 1.6), 2) + Math.Pow(tmp.Y - (int)(radius * 1.6), 2));
                    if (max_area < area )
                    {
                        max_area = area;
                        green_Obj_center = contours.GetMinAreaRect().center;
                    }
                }
            }


            //--------------------------------------------------------------------
            if (max_area < 10 || max_area > 300)
            {
                display_img = new Image<Bgr, byte>(500, 500, new Bgr(255, 255, 255));
                display_img.Draw(new CircleF(new PointF(250, 250), 150), new Bgr(100, 0, 255), 3);
                display_img.Draw(new LineSegment2DF(new PointF(250.0f, 50.0f), new PointF(250, 450)), new Bgr(10, 10, 10), 1);
                //display_img.Draw(new LineSegment2DF(new PointF(250.0f, 250.0f), new PointF(250.0f, 100.0f)), new Bgr(50, 255, 50), 10);

                //MCvFont f1 = new MCvFont(Emgu.CV.CvEnum.FONT.CV_FONT_HERSHEY_COMPLEX, 0.5, 0.5);
                //display_img.Draw("0.0", ref f1, new Point(250, 90), new Bgr(255, 0, 255));
                CvInvoke.cvCircle(display_img, new Point(250, 250), 3, new MCvScalar(255, 0, 100), -1, LINE_TYPE.CV_AA, 0);
                greenAng = -100000;
                return;
            }
            //double tmp_ang1 = 180.0 / Math.PI * Math.Atan2(-Math.Sqrt(3),-1);


            double tmp_ang = 180.0 / Math.PI * Math.Atan2((green_Obj_center.Y - (int)(radius * 1.6)), (green_Obj_center.X - (int)(radius * 1.6)));

            double real_Angle = 0;
            if (tmp_ang >= 0 && tmp_ang <= 180)
                real_Angle = tmp_ang;
            else if (tmp_ang < 0)
                real_Angle = 360 + tmp_ang;


            real_Angle = real_Angle - initeAngle + delta;
            if (real_Angle < 0) real_Angle = 360 + real_Angle;
            //else if(real_Angle > 0) real_Angle = real_Angle - 360;

            if (real_Angle + 90 < 0) greenAng = real_Angle + 360;
            else greenAng = real_Angle;

            //**************************************************************************
            display_img.Draw(new CircleF(new PointF(250, 250), 150), new Bgr(100, 0, 255), 3);
            display_img.Draw(new LineSegment2DF(new PointF(250.0f, 50.0f), new PointF(250, 450)), new Bgr(10, 10, 10), 1);

            PointF line_endPos = new PointF((float)(250 + 180.0f * Math.Cos((greenAng - 90) * Math.PI / 180.0)), (float)(250 + 180.0f * Math.Sin((greenAng - 90) * Math.PI / 180.0)));
            display_img.Draw(new LineSegment2DF(new PointF(250.0f, 250.0f), line_endPos), new Bgr(50, 255, 50), 10);

            MCvFont f = new MCvFont(Emgu.CV.CvEnum.FONT.CV_FONT_HERSHEY_COMPLEX, 1.2, 1.2);  
            display_img.Draw((greenAng).ToString("#.##"), ref f, new Point((int)line_endPos.X - 10, (int)line_endPos.Y - 10), new Bgr(255, 0, 0));


            CvInvoke.cvCircle(display_img, new Point(250, 250), 3, new MCvScalar(255, 0, 100), -1, LINE_TYPE.CV_AA, 0);

           //pic_visualAngle.Image = display_img.ToBitmap();
           // Application.DoEvents();

        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void diplay_Angle1(Image<Bgr, byte> frame, double initeAngle, double delta, double green_oldAngle, double green_delta, out Image<Bgr, byte> display_img, out double greenAng)
        {
            greenAng = 0;
            display_img = new Image<Bgr, byte>(500, 500, new Bgr(255, 255, 255));


            //double tmp_ang = 180.0 / Math.PI * Math.Atan2((green_Obj_center.Y - (int)(radius * 1.6)), (green_Obj_center.X - (int)(radius * 1.6)));
            double tmp_ang = green_oldAngle + green_delta;
            if (tmp_ang > 360)
                greenAng = tmp_ang - 360;
            else if (tmp_ang < 0)
                greenAng = 360 + tmp_ang;
            else
                greenAng = tmp_ang;

            //real_Angle = real_Angle - initeAngle + delta;
            //if (real_Angle < 0) real_Angle = 360 + real_Angle;

            //if (real_Angle + 90 < 0) greenAng = real_Angle + 360;
            //else greenAng = real_Angle;

            //**************************************************************************
            display_img.Draw(new CircleF(new PointF(250, 250), 150), new Bgr(100, 0, 255), 3);
            display_img.Draw(new LineSegment2DF(new PointF(250.0f, 50.0f), new PointF(250, 450)), new Bgr(10, 10, 10), 1);

            PointF line_endPos = new PointF((float)(250 + 180.0f * Math.Cos((greenAng - 90) * Math.PI / 180.0)), (float)(250 + 180.0f * Math.Sin((greenAng - 90) * Math.PI / 180.0)));
            display_img.Draw(new LineSegment2DF(new PointF(250.0f, 250.0f), line_endPos), new Bgr(50, 255, 50), 10);

            MCvFont f = new MCvFont(Emgu.CV.CvEnum.FONT.CV_FONT_HERSHEY_COMPLEX, 1.2, 1.2);
            display_img.Draw((greenAng).ToString("#.##"), ref f, new Point((int)line_endPos.X - 10, (int)line_endPos.Y - 10), new Bgr(255, 0, 0));


            CvInvoke.cvCircle(display_img, new Point(250, 250), 3, new MCvScalar(255, 0, 100), -1, LINE_TYPE.CV_AA, 0);

            //pic_visualAngle.Image = display_img.ToBitmap();
            // Application.DoEvents();

        }

        /////////////////////////////////////////////////////////////////////
        private void find_circle(Image<Bgr, Byte> frame, Image<Gray, byte> grayImg, out PointF centerOfCircle, out double radius)
        {
            centerOfCircle = new PointF();
            radius = 0;

            Image<Bgr, byte> frame1 = frame.Copy();             
            Image<Gray, byte> resizedImage = grayImg.Copy();   

            CircleF[] circles = resizedImage.HoughCircles(new Gray(200), new Gray(190), 1.9, 1, 30, 150)[0];
            IntPtr storage = CvInvoke.cvCreateMemStorage(0);
            //CvInvoke.cvHoughCircles(resizedImage, storage, HOUGH_TYPE.CV_HOUGH_GRADIENT, 1.7, resizedImage.Height / 10, 100, 150, 50, 150);

            //pictureBox1.Image = frame1.ToBitmap();

            if (circles.Length > 1)
            {
                //---------------------------------------------------------------------
                Matrix<Single> center;
                int[] label_hist;

                kmeans(circles, out center, out label_hist);

                //---------------------------------------------------------------------
                double m_distance = Math.Sqrt(Math.Pow(center[0, 0] - center[1, 0], 2) + Math.Pow(center[0, 1] - center[1, 1], 2));
                Matrix<Single> centers;
                bool merge_flag = false;
                bool first_flag = false;

                //---------------------------------------------------------------------
                if (m_distance < 30)
                {
                    centers = new Matrix<Single>(1, 2);
                    centers[0, 0] = (center[0, 0] * label_hist[0] + center[1, 0] * label_hist[1]) / (label_hist[0] + label_hist[1]);
                    centers[0, 1] = (center[0, 1] * label_hist[0] + center[1, 1] * label_hist[1]) / (label_hist[0] + label_hist[1]);
                    merge_flag = true;
                    // CvInvoke.cvCircle(frame1, new Point((int)centers[0, 0], (int)centers[0, 1]), 5, new MCvScalar(0, 0, 255), -1, LINE_TYPE.CV_AA, 0);
                }
                else
                {
                    if (label_hist[0] > label_hist[1])
                    {
                        centers = new Matrix<Single>(1, 2);
                        centers[0, 0] = center[0, 0];                       centers[0, 1] = center[0, 1];
                        first_flag = true;
                    }
                    else
                    {
                        centers = new Matrix<Single>(1, 2);
                        centers[0, 0] = center[1, 0];                        centers[0, 1] = center[1, 1];
                    }
                }

                //---------------------------------------------------
                List<PointF> vect_circle_edge;
                bool flag = false;
                get_circleEdge(frame1, resizedImage, centers, out vect_circle_edge, out flag);

                // repeat processing----------------------------------------------
                if (!merge_flag && !flag)
                {
                    if (first_flag) {
                        centers = new Matrix<Single>(1, 2);
                        centers[0, 0] = center[1, 0];   centers[0, 1] = center[1, 1];
                        get_circleEdge(frame1, resizedImage, centers, out vect_circle_edge, out flag);
                    } else {
                        centers = new Matrix<Single>(1, 2);
                        centers[0, 0] = center[0, 0];  centers[0, 1] = center[0, 1];
                        get_circleEdge(frame1, resizedImage, centers, out vect_circle_edge, out flag);
                    }
                }
                else if (merge_flag && !flag)  {
                    return;
                }

                //***************************************************************************
                if (!flag) return;

                //------------------------------------------------------------------------
                centerOfCircle = new PointF();
                radius = 0;
                find_Center_R_Ofcircle(vect_circle_edge, out centerOfCircle, out radius);
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void display_ballInitialLine(Image<Bgr, byte> out_frame, PointF centerOfCircle, double radius, double St_Angle, double initial_criteraAngle, double criteraAngle, out Image<Bgr, byte> out_frame1)
        {
            out_frame1 = out_frame.Copy();
            double real_Angle = 0;
            double delta = initial_criteraAngle - criteraAngle;

            if (St_Angle - delta < 0)          {
                real_Angle = 360 + (St_Angle - delta);
            }
            else if (St_Angle - delta > 360)
            {
                real_Angle = (St_Angle - delta) - 360;
            }
            else
            {
                real_Angle = St_Angle - delta;
            }

            //double alpha = m_info.angle_calculte(St_Angle, St_Angle, initial_criteraAngle, criteraAngle);
            PointF line_endPos = new PointF((float)(centerOfCircle.X + 1.8 * radius * Math.Cos(real_Angle * Math.PI / 180.0)), (float)(centerOfCircle.Y + 1.8 * radius * Math.Sin(real_Angle * Math.PI / 180.0)));
            CvInvoke.cvLine(out_frame1, new Point((int)centerOfCircle.X, (int)centerOfCircle.Y), new Point((int)line_endPos.X, (int)line_endPos.Y), new MCvScalar(200, 150, 150), 2, LINE_TYPE.CV_AA, 0);
        }

        ////////////////////////////////////////////////////////////////////////////
        private void find_CriteriaAngle(Image<Gray, byte> resizedImag, PointF centerOfCircle, double radius, out double criteraAngle)
        {
            criteraAngle = 0;
            double radius1 = radius * 0.5;
            for (double alpha = Math.PI; alpha < Math.PI * 2.0; alpha += 0.05)
            {
                int X = (int)(centerOfCircle.X + radius1 * Math.Cos(alpha));
                int Y = (int)(centerOfCircle.Y + radius1 * Math.Sin(alpha));
                if (resizedImag.Data[Y, X, 0] > 100)
                {
                    criteraAngle = alpha * 180.0 / Math.PI; ;
                    break;
                }
            }

            //resizedImag.Save("2.png");
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void get_circleEdge(Image<Bgr, byte> RGB_img, Image<Gray, byte> BW_img, Matrix<Single> centers_Clr, out List<PointF> list_circle_edge, out bool flag)
        {
            list_circle_edge = new List<PointF>();
            double max_length = 0, min_length = 10e10;

            //*********************************************************
            for (int alpha = 0; alpha < 200; alpha += 10)
            {
                bool flag_st_p = false;
                PointF p_st = new PointF();
                PointF p_end = new PointF();

                //---------------------------------------------------------------------
                for (int r = 30; r < 200; r++)
                {
                    int x = (int)(centers_Clr[0, 0] + r * Math.Cos((float)alpha * Math.PI / 180.0));
                    int y = (int)(centers_Clr[0, 1] + r * Math.Sin((float)alpha * Math.PI / 180.0));

                    if (x < 0 || x >= BW_img.Width || y < 0 || y >= BW_img.Height)    continue;

                    if (flag_st_p == false && BW_img.Data[y, x, 0] <= 50) {
                        flag_st_p = true;
                        p_st.X = x; p_st.Y = y;
                        p_end.X = x; p_end.Y = y;
                    } else if (flag_st_p == true && BW_img.Data[y, x, 0] >= 200)   {
                        flag_st_p = false;
                        p_end.X = x; p_end.Y = y;

                        //---------------------------------------------------------------------
                        double dist = Math.Sqrt(Math.Pow(p_st.X - p_end.X, 2) + Math.Pow(p_st.Y - p_end.Y, 2));
                        if (dist < 10) {
                            flag_st_p = false;   continue;
                        }
                        else {
                            if (list_circle_edge.Count < 10)
                            {
                                if (dist < min_length) min_length = dist;
                                if (dist > max_length) max_length = dist;
                            }

                            list_circle_edge.Add(p_end);
                            break;
                        }
                    }
                }

                ////---------------------------------------------------------------------
                //if (p_st != null && p_end != null)
                //    CvInvoke.cvLine(RGB_img, new Point((int)p_st.X, (int)p_st.Y), new Point((int)p_end.X, (int)p_end.Y), new MCvScalar(0, 0, 255), 2, LINE_TYPE.CV_AA, 0);
            }

            //*********************************************************
            if (max_length < 1.3 * min_length)
                flag = true;
            else
                flag = false;
        }

        ////////////////////////////////////////////////////////////////////////////
        private void find_Center_R_Ofcircle(List<PointF> vect_circle_edge, out PointF centerOfCircle, out double radius)
        {
            List<double> vect_radius = new List<double>();
            List<PointF> centers = new List<PointF>();

            //*********************************************************
            for (int n = 0; n < vect_circle_edge.Count - 8; n += 1)
            {
                PointF A = vect_circle_edge[n];
                PointF B = vect_circle_edge[n + 4];
                PointF C = vect_circle_edge[n + 8];

                PointF mid_AB = new PointF((float)(A.X + B.X) / 2.0f, (float)(A.Y + B.Y) / 2.0f);
                double grad_AB = (B.Y - A.Y) / (B.X - A.X);
                double perp_grad_AB = -1 / grad_AB;

                PointF mid_BC = new PointF((float)(C.X + B.X) / 2.0f, (float)(C.Y + B.Y) / 2.0f);
                double grad_BC = (C.Y - B.Y) / (C.X - B.X);
                double perp_grad_BC = -1 / grad_BC;

                double X = ((mid_BC.Y - mid_AB.Y) + perp_grad_AB * mid_AB.X - perp_grad_BC * mid_BC.X) / (perp_grad_AB - perp_grad_BC);
                double Y = perp_grad_AB * (X - mid_AB.X) + mid_AB.Y;

                radius = Math.Sqrt(Math.Pow(A.X - X, 2) + Math.Pow(A.Y - Y, 2));
                centerOfCircle = new PointF((float)X, (float)Y);

                centers.Add(centerOfCircle);
                vect_radius.Add(radius);
            }

            //*********************************************************
            radius = 0;
            centerOfCircle = new PointF(0.0f, 0.0f);
            int count = 0;
            for (int n = 0; n < centers.Count; n++)
            {
                if (double.IsNaN(vect_radius[n])) continue;

                count++;
                radius += vect_radius[n];
                centerOfCircle.X += centers[n].X;
                centerOfCircle.Y += centers[n].Y;
            }
            radius /= count;
            centerOfCircle.X /= count;
            centerOfCircle.Y /= count;
        }

        ////////////////////////////////////////////////////////////////////////////
        private void kmeans(CircleF[] circles, out Matrix<Single> centers, out int[] label_hist)
        {
            //*********************************************************
            float[,] data = new float[circles.Length, 2];
            for (int i = 0; i < circles.Length; i++)
            {
                data[i, 0] = (float)circles[i].Center.X;
                data[i, 1] = (float)circles[i].Center.Y;
            }

            //*********************************************************
            MCvTermCriteria term = new MCvTermCriteria();
            Matrix<Single> samplesMatrix = new Matrix<float>(data);
            Matrix<Int32> labels = new Matrix<Int32>(circles.Length, 1);

            int clusterCount = 2;
            centers = new Matrix<Single>(clusterCount, 2);

            //*********************************************************

            CvInvoke.cvKMeans2(samplesMatrix, clusterCount, labels, term, 2, IntPtr.Zero, 0, centers, IntPtr.Zero);

            //*********************************************************
            label_hist = new int[clusterCount];
            for (int i = 0; i < circles.Length; i++)
            {
                label_hist[labels[i, 0]] += 1;
            }
        }

        ////////////////////////////////////////////////////////////////////////////
        private void btn_close_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            roulette_processing_flag = false;
            if (timer1.Enabled)
            {
                timer1.Enabled = false;
                button2.Text = "Start";

                //frame_count = new List<int>();
                //green_angles = new List<double>();

                //-----------------------------------------------------------------
                //f_count = 0;
                initial_criteraAngle = 0;
                St_Angle = 0;
                old_greenAngle = 0;
                green_deltaAngle = 0;
                //ball_Obj = new ball_history();
                //-----------------------------------------------------------------
                lbl_statue.Text = "Ready ...";
                lbl_round.Text = "0: ROUND";
            }
            else
            {
                first_frame_flag = false;
                timer1.Enabled = true;
                button2.Text = "Stop";
                lbl_statue.Text = "finishing ...";
                //lbl_round.Text = round_number.ToString() + " : ROUND";

                //-----------------------------------------------------------------
                initial_displayAngle();
                
            }
        }

        private void Roulette_main_Load(object sender, EventArgs e)
        {
            fm2 = new Form2(this);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            get_screenImg();
            //Thread thread_screenCapture = new Thread(get_screenImg);
            //thread_screenCapture.Start();
            textBox1.Text = _W.ToString() + " , " + _H.ToString();
            //Thread.Sleep(0);
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            int m_now = DateTime.Now.Year;
            if (m_now > 2020) Application.Exit();
        }

        ////////////////////////////////////////////////////////////////////////////
        public void get_screenImg()
        {
            try
            {
                if (!roulette_processing_flag)
                {
                    roulette_processing_flag = true;

                    // Calculate rectangle cords/size
                    BitmapSource bSource = ScreenCapturer.CaptureRegion(_X, _Y, _W, _H);

                    System.Drawing.Bitmap bitmap;
                    using (MemoryStream outStream = new MemoryStream())
                    {
                        BitmapEncoder enc = new BmpBitmapEncoder();
                        enc.Frames.Add(BitmapFrame.Create(bSource));
                        enc.Save(outStream);
                        bitmap = new System.Drawing.Bitmap(outStream);
                    }

                    Image<Bgr, byte> img = new Image<Bgr, Byte>(bitmap);

                    img = img.Resize(2356, 2356 * img.Height / img.Width, INTER.CV_INTER_LINEAR);

                    Roulett_recognition(img);
                    //Thread.Sleep(1);
                    roulette_processing_flag = false;
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message.ToString());
            }
        }

        ////////////////////////////////////////////////////////////////////////////
        private void button1_Click(object sender, EventArgs e)
        {
            
            fm2.Show();

        }

        ////////////////////////////////////////////////////////////////////////////
        private Image<Gray, byte> FillHoles(Image<Gray, byte> image)
        {
            var resultImage = image.CopyBlank();
            Gray gray = new Gray(255);
            using (var mem = new MemStorage())
            {
                for (var contour = image.FindContours(Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE,
                   Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_CCOMP,
                    mem); contour != null; contour = contour.HNext)
                {
                    resultImage.Draw(contour, gray, -1);
                }
            }

            return resultImage;
        }

        ////////////////////////////////////////////////////////////////////////////
        Image<Gray, Byte> bwareaopen(Image<Gray, Byte> binimg, int size)
        {
            try
            {
                Image<Gray, Byte> input = binimg.Clone();

                MemStorage storage = new MemStorage();
                IntPtr contour1 = new IntPtr();

                CvInvoke.cvFindContours(input.Ptr, storage, ref contour1, StructSize.MCvContour, Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_LIST, Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE, new Point(0, 0));
                Seq<Point> contour = new Seq<Point>(contour1, null);

                double area;
                while (contour != null && contour.Ptr.ToInt32() != 0)
                {
                    area = CvInvoke.cvContourArea(contour, Emgu.CV.Structure.MCvSlice.WholeSeq, 1);

                    if (-size <= area && area <= 0)
                    {
                        // removes white dots
                        CvInvoke.cvDrawContours(binimg.Ptr, contour.Ptr, new MCvScalar(0, 0, 0), new MCvScalar(0, 0, 0), -1, -1, Emgu.CV.CvEnum.LINE_TYPE.EIGHT_CONNECTED, new Point(0, 0));
                    }
                    else if (0 < area && area <= size)
                    {
                        // fills in black holes
                        CvInvoke.cvDrawContours(binimg.Ptr, contour.Ptr, new MCvScalar(0xff, 0xff, 0xff), new MCvScalar(0xff, 0xff, 0xff), -1, -1, LINE_TYPE.EIGHT_CONNECTED, new Point(0, 0));
                    }
                    contour = contour.HNext;
                }

                return binimg;
            }catch
            {
                return binimg;
            }
        }

        public void set_Screenshot_param(int X, int Y, int W, int H)
        {
            _X = X;
            _Y = Y;
            _W = W;
            _H = H;
        }

    }
}
