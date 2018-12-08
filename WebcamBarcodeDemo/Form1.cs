using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dynamsoft.Barcode;
using Dynamsoft.TWAIN;
using Dynamsoft.Core;
using Dynamsoft.UVC;
using Dynamsoft.OCR;
using Dynamsoft.PDF;
using Dynamsoft.Core.Annotation;
using Dynamsoft.TWAIN.Interface;
using Dynamsoft.Core.Enums;
using Dynamsoft.Common;
using Dynamsoft.Forms;
using System.Diagnostics;

namespace     WebcamBarcodeDemo
{
    public partial class Form1 : Form, Dynamsoft.PDF.IConvertCallback
    {
        private Dynamsoft.Core.ImageCore m_ImageCore = null;
        private CameraManager m_CameraManager = null;
        private Dynamsoft.PDF.PDFRasterizer m_PDFRasterizer = null;
        //private Camera m_CurrentCamera = null;
        private string m_StrProductKey = "t0068UwAAAD+SNFEX8Hkh70orGGNqrOhwmlShaSU61BvypdRAIC0ebkSO0xF29Pt4XeXGBgWLk3csiAK4k4vMK2rwI/XPqBM=;t0068UwAAAD+SNFEX8Hkh70orGGNqrOhwmlShaSU61BvypdRAIC0ebkSO0xF29Pt4XeXGBgWLk3csiAK4k4vMK2rwI/XPqBM=;t0068UwAAAD+SNFEX8Hkh70orGGNqrOhwmlShaSU61BvypdRAIC0ebkSO0xF29Pt4XeXGBgWLk3csiAK4k4vMK2rwI/XPqBM=;t0068UwAAAD+SNFEX8Hkh70orGGNqrOhwmlShaSU61BvypdRAIC0ebkSO0xF29Pt4XeXGBgWLk3csiAK4k4vMK2rwI/XPqBM=;t0068UwAAAD+SNFEX8Hkh70orGGNqrOhwmlShaSU61BvypdRAIC0ebkSO0xF29Pt4XeXGBgWLk3csiAK4k4vMK2rwI/XPqBM=";


        public Form1()
        {
            InitializeComponent();
            m_ImageCore = new ImageCore();
            dsViewer1.Bind(m_ImageCore);
            m_CameraManager = new CameraManager(m_StrProductKey);
            m_PDFRasterizer = new Dynamsoft.PDF.PDFRasterizer(m_StrProductKey);

            if (m_CameraManager.GetCameraNames() !=null)
            {
                List<String> tempCameraNames = m_CameraManager.GetCameraNames();
                foreach (string temp in tempCameraNames)
                {
                    comboBox1.Items.Add(temp);
                }

                Camera tempCamera = m_CameraManager.SelectCamera(0);
                comboBox1.SelectedIndex = 0;
                tempCamera.Open();
                tempCamera.SetVideoContainer(this.pictureBox1.Handle);
                ResizeVideoWindow();
                List<CamResolution> listCamResolutions = tempCamera.SupportedResolutions;
                comboBox2.Items.Clear();

                foreach (CamResolution temp in listCamResolutions)
                {
                    string tempHeight = temp.Height.ToString();
                    string tempWidth = temp.Width.ToString();
                    string tempResolution = tempWidth + " X " + tempHeight;
                    comboBox2.Items.Add(tempResolution);
                    comboBox2.SelectedIndex = 0;
                }
                button2.Enabled = true;
            }
            else
            {
                button2.Enabled = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void dsViewer1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Camera tempCamera = m_CameraManager.SelectCamera(m_CameraManager.CurrentSourceName);
            Bitmap tempBit = tempCamera.GrabImage();
            m_ImageCore.IO.LoadImage(tempBit);

            BarcodeReader m_BarcodeReader = new BarcodeReader();
            m_BarcodeReader.LicenseKeys = m_StrProductKey;
            //m_BarcodeReader.ReaderOptions.MaxBarcodesToReadPerPage = 100;
            //m_BarcodeReader.ReaderOptions.BarcodeFormats = Dynamsoft.Barcode.Barcode.BarcodeFormat.OneD | Dynamsoft.Barcode.BarcodeFormat.PDF417 |
            //    Dynamsoft.Barcode.BarcodeFormat.QR_CODE |
            //    Dynamsoft.Barcode.BarcodeFormat.DATAMATRIX;
            Bitmap bmp = null;

            if (m_ImageCore.ImageBuffer.CurrentImageIndexInBuffer < 0)
            {
                MessageBox.Show("Please acquire or load an image before reading barcode.");
                return;
            }

            bmp = (Bitmap)m_ImageCore.ImageBuffer.GetBitmap(this.m_ImageCore.ImageBuffer.CurrentImageIndexInBuffer);
            //BarcodeResult[] aryResult = null;
            //aryResult = m_BarcodeReader.DecodeBitmap(bmp);
            //StringBuilder strText = new StringBuilder();
            //if (aryResult == null)
            //{
            //    //this.txtBarcode.Text = "No barcode found.";
            //    MessageBox.Show("No barcode found.");
            //}
            //else
            //{
            //    strText.AppendFormat(aryResult.Length + " total barcode" + (aryResult.Length == 1 ? ".\r\n" : "s" + " found.\r\n"));
            //    for (int i = 0; i < aryResult.Length; i++)
            //    {
            //        BarcodeResult objResult = aryResult[i];
            //        strText.AppendFormat("      Result " + (i + 1) + ":\r\n");
            //        strText.AppendFormat("      BarcodeFormat: " + objResult.BarcodeFormat.ToString() + "\r\n");
            //        strText.AppendFormat("      Text read: " + objResult.BarcodeText + "\r\n");
            //    }
            //    //this.txtBarcode.Text = strText.ToString();
            //    MessageBox.Show(strText.ToString());
            //}
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog filedlg = new OpenFileDialog();
                filedlg.Filter = "All Support Files|*.JPG;*.JPEG;*.JPE;*.JFIF;*.BMP;*.PNG;*.TIF;*.TIFF;*.PDF;*.GIF|JPEG|*.JPG;*.JPEG;*.JPE;*.Jfif|BMP|*.BMP|PNG|*.PNG|TIFF|*.TIF;*.TIFF|PDF|*.PDF|GIF|*.GIF";
                filedlg.FilterIndex = 0;

                filedlg.Multiselect = true;
                // try to locate images folder
                string imagesFolder = Application.StartupPath;

                // we assume we are running under the DotImage install folder
                imagesFolder = imagesFolder.Replace("/", "\\");
                int pos = imagesFolder.LastIndexOf("\\Samples\\");
                if (pos != -1)
                {
                    //strPDFDllFolder = strPDFDllFolder.Substring(0, strPDFDllFolder.IndexOf(@"\", pos)) + @"\Redistributable\Resources\PDF";
                    imagesFolder = imagesFolder.Substring(0, imagesFolder.IndexOf(@"\", pos)) + @"\Bin\Images\BarcodeImages";
                }
                else
                {
                    pos = imagesFolder.LastIndexOf("\\");
                    imagesFolder = imagesFolder + @"\Bin\Images\BarcodeImages";
                }

                if (filedlg.ShowDialog() == DialogResult.OK)
                {
                    foreach (string strfilename in filedlg.FileNames)
                    {
                        pos = strfilename.LastIndexOf(".");
                        if (pos != -1)
                        {
                            string strSuffix = strfilename.Substring(pos, strfilename.Length - pos).ToLower();
                            if (strSuffix.CompareTo(".pdf") == 0)
                            {
                                m_PDFRasterizer.ConvertMode = Dynamsoft.PDF.Enums.EnumConvertMode.enumCM_RENDERALL;
                                m_PDFRasterizer.ConvertToImage(strfilename, "", 200, this as Dynamsoft.PDF.IConvertCallback);
                                continue;
                            }
                        }
                        m_ImageCore.IO.LoadImage(strfilename);
                    }
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {

            if (m_ImageCore.ImageBuffer.HowManyImagesInBuffer > 0)
            {
                m_ImageCore.ImageBuffer.RemoveAllImages();
            }
        }

        private void ResizeVideoWindow()
        {
            Camera tempCamera = m_CameraManager.SelectCamera(m_CameraManager.CurrentSourceName);
            CamResolution camResolution = tempCamera.CurrentResolution;
            if (camResolution != null && camResolution.Width > 0 && camResolution.Height > 0)
            {
                ////if (iRotate % 2 == 0)
                {
                    int iVideoWidth = pictureBox1.Width;
                    int iVideoHeight = pictureBox1.Width * camResolution.Height / camResolution.Width;
                    int iContentHeight = pictureBox1.Height - pictureBox1.Margin.Top - pictureBox1.Margin.Bottom - pictureBox1.Padding.Top - pictureBox1.Padding.Bottom;
                    if (iVideoHeight < iContentHeight)
                        tempCamera.ResizeVideoWindow(0, (iContentHeight - iVideoHeight) / 2, iVideoWidth, iVideoHeight);
                    else
                        tempCamera.ResizeVideoWindow(0, 0, pictureBox1.Width, pictureBox1.Height);
                }

            }
        }

        #region
        void Dynamsoft.PDF.IConvertCallback.LoadConvertResult(Dynamsoft.PDF.ConvertResult result)
        {
            m_ImageCore.IO.LoadImage(result.Image);
        }
        #endregion

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

            Camera tempCamera = m_CameraManager.SelectCamera(m_CameraManager.CurrentSourceName);
            if (comboBox2.Text != null)
            {
                string[] strResoultion = comboBox2.Text.Split(new char[] { 'X' });
                if (strResoultion.Length == 2)
                {
                    try
                    {
                        tempCamera.CurrentResolution = new CamResolution(
                            int.Parse(strResoultion[0]), int.Parse(strResoultion[1]));
                        ResizeVideoWindow();

                    }
                    catch { }
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_CameraManager != null)
            {
                Camera tempCamera = m_CameraManager.SelectCamera((short)comboBox1.SelectedIndex);
                tempCamera.SetVideoContainer(pictureBox1.Handle);
                tempCamera.Open();
                ResizeVideoWindow();
                List<CamResolution> listCamResolutions = tempCamera.SupportedResolutions;
                comboBox2.Items.Clear();
                foreach (CamResolution temp in listCamResolutions)
                {
                    string tempHeight = temp.Height.ToString();
                    string tempWidth = temp.Width.ToString();
                    string tempResolution = tempWidth + " X " + tempHeight;
                    comboBox2.Items.Add(tempResolution);
                    comboBox2.SelectedIndex = 0;
                }
            }
        }
    }
}
