﻿using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Printing;
using System.Text;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace DSoft
{
    /// <summary>
    /// Cette classe permet d'imprimer directement un fichier RDLC en lui fournissant une source de donnée
    /// </summary>
    public class RDLCPrinter : IDisposable
    {
        private int _currentPageIndex;
        private IList<Stream> _streams;
        private LocalReport _report;
        private int _Copies;
        Metafile _pageImage = null;
        Rectangle _adjustedRect = new Rectangle();
        private ReportType _ReportType;
        private string _path;
        private Warning[] _warnings = null;
        private string[] _streamids = null;
        private string _mimeType = String.Empty;
        private string _encoding = String.Empty;
        private string _extension = String.Empty;
        private string _filename = "";
        private PrintDocument _printDoc = null;
        public EventHandler FileSaving;
        public EventHandler FileSaved;
        private int _dataCount;

        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        public RDLCPrinter()
        {

        }
        
        /// <summary>
        /// Initialise un objet d'impression DirectRDLCPrinter avec la source de donne.
        /// </summary>
        /// <param name="report"></param>
        /// <param name="rtype"></param>
        /// <param name="nbrPage"></param>
        /// <param name="path"></param>
        public RDLCPrinter(LocalReport report, ReportType rtype = ReportType.Printer, int nbrPage = 1, string path = "", int DataCount = 0)
        {
            this._report = report;
            this._Copies = nbrPage;
            this._ReportType = rtype;
            this._path = path;
            this._dataCount = DataCount;
        }

        #region Les différente Propriétée de l'objet
        /// <summary>
        /// recoie le nombre de ligne Du DataSource du raport
        /// </summary>
        [DefaultValue(0)]
        public int DataCount
        {
            get
            {
                return _dataCount;
            }
            set
            {
                _dataCount = value;
            }
        }


        /// <summary>
        /// Assigne l'imprimante par default Printer ou Retourne l'imprimante demandé
        /// </summary>        
        public PrintDocument PrintDoc
        {            
            get
            {
                return (this._printDoc != null) ? this._printDoc : GetDefaultPrinter();
            }
            set
            {
                this._printDoc = value;
            }
        }


        /// <summary>
        /// Assigne la valeur par default a Printer ou Retourne le type de raport demandé
        /// </summary>
        [DefaultValue(ReportType.Printer)]
        public ReportType Reporttype
        {
            get
            {
                return this._ReportType;
            }
            set
            {
                this._ReportType = value;
            }
        }

        /// <summary>
        /// Obtient l'orientation par defaut du rapport
        /// </summary>
        public bool? isDefaultLandscape
        {
            get
            {
                if (this._report != null)
                    return _report.GetDefaultPageSettings().IsLandscape;
                else
                    return null;
            }            
        }

        /// <summary>
        /// Assigne la valeur par default a 1 pour le nombre de copies ou Retourne le nombre de copies demandé
        /// </summary>
        [DefaultValue(1)]
        public int NombreCopies
        {
            get
            {
                return this._Copies;
            }
            set
            {
                this._Copies = value;
            }
        }

        /// <summary>
        /// retourne le Path ou enregirtrer les différents type de rapport
        /// </summary>
        [DefaultValue("")]
        public string Path
        {
            get
            {
                return this._path;
            }
            set
            {
                this._path = value;
            }
        }

        /// <summary>
        /// Assigne le raport par default ou retourne le rapport demandé
        /// </summary>        
        public LocalReport Report
        {
            get
            {
                return this._report;
            }
            set
            {
                this._report = value;
            }
        }
        #endregion

        #region Création des stream  et exportation du raport en format EMF (Enhanced Metafile)pour l'impression
        /// <summary>
        /// Routine de fournir au convertisseur de rapport, afin d'enregistrer une image pour chaque page du rapport.
        /// </summary>
        private Stream CreateStream(string name, string fileNameExtension, Encoding encoding, string mimeType, bool willSeek)
        {
            Stream stream = new MemoryStream();
            _streams.Add(stream);
            return stream;
        }


        // Exporter le raport vers le format EMF (Enhanced Metafile).
        private void Export(LocalReport report)
        {
            if (_printDoc == null)
                this._printDoc = GetDefaultPrinter();

            string deviceInfo = GetDeviceInfo();

            Warning[] warnings;
            _streams = new List<Stream>();

            for (int i = 0; i < _Copies; i++)
                report.Render("Image", deviceInfo, CreateStream, out warnings);
            
            foreach (Stream stream in _streams)
                stream.Position = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string GetDeviceInfo(){           

            string deviceinfo;

            if (_report != null)
            {
                if (this._report.GetDefaultPageSettings().IsLandscape)
                    deviceinfo = @"<DeviceInfo>
                        <OutputFormat>EMF</OutputFormat>
                        <StartPage>" + this._printDoc.PrinterSettings.FromPage.ToString() + @"</StartPage>
                        <EndPage>" + this._printDoc.PrinterSettings.ToPage.ToString() + @"</EndPage>
                        <PageWidth>" + ((double)this._report.GetDefaultPageSettings().PaperSize.Height / 100).ToString() + @"in</PageWidth>
                        <PageHeight>" + ((double)this._report.GetDefaultPageSettings().PaperSize.Width / 100).ToString() + @"in</PageHeight>
                        <MarginTop>" + ((double)this._report.GetDefaultPageSettings().Margins.Top / 100).ToString() + @"in</MarginTop>
                        <MarginLeft>" + ((double)this._report.GetDefaultPageSettings().Margins.Left / 100).ToString() + @"in</MarginLeft>
                        <MarginRight>" + ((double)this._report.GetDefaultPageSettings().Margins.Right / 100).ToString() + @"in</MarginRight>
                        <MarginBottom>" + ((double)this._report.GetDefaultPageSettings().Margins.Bottom / 100).ToString() + @"in</MarginBottom>
                    </DeviceInfo>";
                else
                    deviceinfo = @"<DeviceInfo>
                        <OutputFormat>EMF</OutputFormat>
                        <StartPage>" + this._printDoc.PrinterSettings.FromPage.ToString() + @"</StartPage>
                        <EndPage>" + this._printDoc.PrinterSettings.ToPage.ToString() + @"</EndPage>
                        <PageWidth>" + ((double)this._report.GetDefaultPageSettings().PaperSize.Width / 100).ToString() + @"in</PageWidth>
                        <PageHeight>" + ((double)this._report.GetDefaultPageSettings().PaperSize.Height / 100).ToString() + @"in</PageHeight>
                        <MarginTop>" + ((double)this._report.GetDefaultPageSettings().Margins.Top / 100).ToString() + @"in</MarginTop>
                        <MarginLeft>" + ((double)this._report.GetDefaultPageSettings().Margins.Left / 100).ToString() + @"in</MarginLeft>
                        <MarginRight>" + ((double)this._report.GetDefaultPageSettings().Margins.Right / 100).ToString() + @"in</MarginRight>
                        <MarginBottom>" + ((double)this._report.GetDefaultPageSettings().Margins.Bottom / 100).ToString() + @"in</MarginBottom>
                    </DeviceInfo>";
            }
            else
                deviceinfo = "ERROR";

            return deviceinfo;
        }

        #endregion

        #region Method Print Page
        private void PrintPage(object sender, PrintPageEventArgs ev)
        {
            
            _pageImage = new Metafile(_streams[_currentPageIndex]);

            // Ajuster le rectangle au marge de la page
            _adjustedRect = new System.Drawing.Rectangle(
                ev.PageBounds.Left - (int)ev.PageSettings.HardMarginX,
                ev.PageBounds.Top - (int)ev.PageSettings.HardMarginY,
                ev.PageBounds.Width,
                ev.PageBounds.Height);

            // Dessiner un rectangle blanc sur la page
            ev.Graphics.FillRectangle(Brushes.White, _adjustedRect);

            // Dessiner le rapport
            ev.Graphics.DrawImage(_pageImage, _adjustedRect);
            
            // Prepare la prochaine page.
            _currentPageIndex++;
            ev.HasMorePages = (_currentPageIndex < _streams.Count);
        }
        #endregion

        #region Method Print Now
        private void PrintNow()
        {

            if (_streams == null || _streams.Count == 0)
                return;

                     
            if (!_printDoc.PrinterSettings.IsValid)
            {
                MessageBox.Show("Error: cannot find the default printer", "Print Error" );                
                return;
            }

            _printDoc.PrintPage += new PrintPageEventHandler(PrintPage);
            _printDoc.EndPrint += printDoc_EndPrint;

            _printDoc.Print();
        }
        
        private void printDoc_EndPrint(object sender, PrintEventArgs e)
        {
            _currentPageIndex = 0;
            _streams = null;            
            
            _pageImage = null;
            _adjustedRect = new Rectangle();
            _warnings = null;
            _streamids = null;
            _mimeType = String.Empty;
            _encoding = String.Empty;
            _extension = String.Empty;            
            _printDoc = null;            
        }

        /// <summary>
        /// Retourne l'imprimante par défaut
        /// </summary>
        /// <returns></returns>
        public PrintDocument GetDefaultPrinter()
        {
            //obtien le nombre de page du rapport
            BitmapDecoder dec = GetImage();

            PrintDocument pDoc = new PrintDocument();
            
            pDoc.DefaultPageSettings.Landscape = this._report.GetDefaultPageSettings().IsLandscape;
            pDoc.DefaultPageSettings.PaperSize = new PaperSize(this._report.GetDefaultPageSettings().PaperSize.PaperName, _report.GetDefaultPageSettings().PaperSize.Width, _report.GetDefaultPageSettings().PaperSize.Height);
            
            //par default ont imprime toute les pages
            pDoc.PrinterSettings.FromPage = 1;
            pDoc.PrinterSettings.ToPage = dec.Frames.Count;
            return pDoc;
        }


        /// <summary>
        /// Retourne un tableau (Array) d'image sous forme de BitmapDecoder
        /// </summary>
        /// <returns></returns>
        public BitmapDecoder GetImage()
        {            
            Stream mStream = new MemoryStream(GetImageArray());

            return BitmapDecoder.Create(mStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);            
        }

        #endregion

        #region method Print pour les différent Type de Format
        /// <summary>
        /// Lance l'impression vers l'imprimante par défaut
        /// </summary>
        public void Print()
        {
            switch (this._ReportType)
            {
                case ReportType.Printer:
                    Export(_report);
                    PrintNow();
                    break;
                case ReportType.Word:
                    SaveAsWord();
                    break;
                case ReportType.PDF:
                    SaveAsPDF();
                    break;
                case ReportType.Excel:
                    SaveAsExcel(); 
                    break;
                case ReportType.Image:
                    SaveAsImage();
                    break;
            }
        }
        #endregion

        #region Bloc comprenant les différentes méthodes pour Sauvargarder les différents type de rapports

        private bool SaveAsImage()
        {
            byte[] byteViewer = GetImageArray();

            if (byteViewer != null)
            {
                if (System.IO.Path.GetFileName(_path).Contains(".png"))
                    _filename = _path;
                else
                    _filename = _path + ".png";

                if (FileSaving != null)
                    FileSaving(this, new EventArgs());

                FileStream newFile = new FileStream(_filename, FileMode.Create);
                
                newFile.Write(byteViewer, 0, byteViewer.Length);
                newFile.Close();

                if (FileSaved != null)
                    FileSaved(this, new EventArgs());

                return true;
            }
            else
                return false;
        }


        /// <summary>
        /// Retourne le rapport en tableau de bytes
        /// </summary>
        /// <returns></returns>
        public byte[] GetImageArray()
        {
            if (_report != null)
                return _report.Render("Image");
            else
                return null;
        }


        /// <summary>
        /// Retourne le rapport en BitmapImage
        /// </summary>
        /// <returns></returns>
        public BitmapImage GetBitmapImage()
        {
            byte[] img = GetImageArray();

            MemoryStream mStream = new MemoryStream(img);

            BitmapImage reportBitmap = new BitmapImage();
            reportBitmap.BeginInit();
            reportBitmap.StreamSource = mStream;
            reportBitmap.EndInit();
            return reportBitmap;


        }
               

        /// <summary>
        /// Enregistre le rapport en Format PDF
        /// </summary>
        /// <returns>Retourne true si l'opération est un succès</returns>
        private bool SaveAsPDF()
        {
            byte[] byteViewer = GetPDFArray();

            if (byteViewer != null)
            {
                if (System.IO.Path.GetFileName(_path).Contains(".pdf"))
                    _filename = _path;
                else
                    _filename = _path + ".pdf";

                if (FileSaving != null)
                    FileSaving(this, new EventArgs());

                FileStream newFile = new FileStream(_filename, FileMode.Create);
                newFile.Write(byteViewer, 0, byteViewer.Length);
                newFile.Close();

                if (FileSaved != null)
                    FileSaved(this, new EventArgs());

                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Renvoie un tableau de byte représentant le rapport en format PDF
        /// </summary>
        /// <returns></returns>
        public byte[] GetPDFArray()
        {
            if (_report != null)
                return _report.Render("PDF");
            else
                return null;
        }

        /// <summary>
        /// Enregistre le rapport en format Excel
        /// </summary>
        private bool SaveAsExcel()
        {
            byte[] bytesViewer = GetExcelArray();
            
            if (bytesViewer != null)  
            {
                if (System.IO.Path.GetFileName(_path).Contains(".xls"))
                    _filename = _path;
                else
                    _filename = _path + ".xls";

                if (FileSaving != null)
                    FileSaving(this, new EventArgs());

                FileStream newFile = new FileStream(_filename, FileMode.Create);
                newFile.Write(bytesViewer, 0, bytesViewer.Length);
                newFile.Close();

                if (FileSaved != null)
                    FileSaved(this, new EventArgs());

                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Renvoie un tableau de byte représentant le rapport en format Excel (.xsl)
        /// </summary>
        /// <returns></returns>
        public byte[] GetExcelArray()
        {
            if (_report != null)
                return _report.Render("Excel", null, out _mimeType, out _encoding,out _extension,out _streamids, out _warnings);
            else
                return null;
        }

        /// <summary>
        /// Enregistre le rapport en format Word Document
        /// </summary>
        private bool SaveAsWord()
        {
            byte[] bytesViewer = GetWordArray();

            if (bytesViewer != null)
            {
                if (System.IO.Path.GetFileName(_path).Contains(".doc"))
                    _filename = _path;
                else
                    _filename = _path + ".doc";

                if (FileSaving != null)
                    FileSaving(this, new EventArgs());
           
                FileStream newFile = new FileStream(_filename, FileMode.Create);
                newFile.Write(bytesViewer, 0, bytesViewer.Length);
                newFile.Close();

                if (FileSaved != null)
                    FileSaved(this, new EventArgs());

                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Renvoie un tableau de byte représentant le rapport en format Excel (.doc)
        /// </summary>
        /// <returns></returns>
        public byte[] GetWordArray()
        {
            if (_report != null)
                return _report.Render("Word", null, out _mimeType, out _encoding, out _extension, out _streamids, out _warnings);
            else
                return null;
        }
        #endregion

        #region Dipose des Streams
        /// <summary>
        /// Dispose des streams
        /// </summary>
        public void Dispose()
        {
            if (_streams != null)
            {
                foreach (Stream stream in _streams)
                    stream.Close();
                _streams = null;
            }
        }
        #endregion
    }
}
