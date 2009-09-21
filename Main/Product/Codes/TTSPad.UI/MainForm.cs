﻿//-----------------------------------------------------------------------
// <copyright file="MainForm.cs" company="Contoso Corporation">
//     Copyright (c) Contoso Corporation. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace GY.TTSPad.UI
{
    using System;
    using System.Drawing;
    using System.Drawing.Printing;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Speech.Synthesis;
    using System.Windows.Forms;

    /// <summary>
    ///
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        ///
        /// </summary>
        private SpeechSynthesizer speechSynthesizer;

        /// <summary>
        ///
        /// </summary>
        private OpenFileDialog openFileDialog;

        /// <summary>
        ///
        /// </summary>
        private SaveFileDialog saveFileDialog;

        /// <summary>
        ///
        /// </summary>
        private PageSetupDialog pageSetupDialog;

        /// <summary>
        ///
        /// </summary>
        private PrintDialog printDialog;

        /// <summary>
        ///
        /// </summary>
        private PrintDocument printDocument;

        /// <summary>
        ///
        /// </summary>
        private PageSettings pageSettings;

        /// <summary>
        ///
        /// </summary>
        private string currentFile;

        /// <summary>
        ///
        /// </summary>
        private int wordOffset;

        /// <summary>
        ///
        /// </summary>
        private string textToSpeech;

        /// <summary>
        ///
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            this.Text = String.Format(CultureInfo.CurrentCulture, "{0}", AssemblyTitle);
            this.speechSynthesizer = new SpeechSynthesizer();
            this.speechSynthesizer.SpeakProgress += SpeakProgress;
            this.speechSynthesizer.SpeakCompleted += SpeakCompleted;
        }

        /// <summary>
        /// Gets
        /// </summary>
        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (!string.IsNullOrEmpty(titleAttribute.Title))
                    {
                        return titleAttribute.Title;
                    }
                }

                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            this.textBoxMainText.Select(0, 0);
            this.toolStripProgressBarTime.Value = 100;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SpeakProgress(object sender, SpeakProgressEventArgs e)
        {
            this.toolStripStatusLabelTime.Text = string.Format(CultureInfo.CurrentCulture, "{0}:{1}", e.AudioPosition.Minutes, e.AudioPosition.Seconds);
            this.toolStripProgressBarTime.Value = (int)(100 * ((double)e.CharacterPosition / (double)this.textToSpeech.Length));
            this.textBoxMainText.Select(e.CharacterPosition + this.wordOffset, e.Text.Length);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemAbout_Click(object sender, EventArgs e)
        {
            new AboutForm().Show();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonStop_Click(object sender, EventArgs e)
        {
            if (SynthesizerState.Speaking == this.speechSynthesizer.State)
            {
                this.speechSynthesizer.SpeakAsyncCancelAll();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonRead_Click(object sender, EventArgs e)
        {
            this.speechSynthesizer.SelectVoiceByHints(this.radioButtonMale.Checked ? VoiceGender.Male : VoiceGender.Female, VoiceAge.Adult);
            if (SynthesizerState.Speaking == this.speechSynthesizer.State)
            {
                this.speechSynthesizer.SpeakAsyncCancelAll();
            }

            if (this.textBoxMainText.SelectionLength > 0)
            {
                this.textToSpeech = this.textBoxMainText.SelectedText;
                this.wordOffset = this.textBoxMainText.SelectionStart;
            }
            else
            {
                this.textToSpeech = this.textBoxMainText.Text;
                this.wordOffset = 0;
            }

            if (!string.IsNullOrEmpty(this.textToSpeech))
            {
                this.speechSynthesizer.SpeakAsync(new Prompt(this.textToSpeech));
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxMainText_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '')
            {
                this.textBoxMainText.SelectAll();
                e.Handled = true;
            }
        }

        /// <summary>
        ///
        /// </summary>
        private void SaveTextToFile()
        {
            saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog.ShowDialog();
            this.currentFile = saveFileDialog.FileName;
            if (!string.IsNullOrEmpty(this.currentFile))
            {
                using (var file = new StreamWriter(this.currentFile))
                {
                    file.Write(textBoxMainText.Text);
                }

                this.Text = String.Format(CultureInfo.CurrentCulture, "{0} - {1}", AssemblyTitle, this.currentFile);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.textBoxMainText.Clear();
            this.currentFile = string.Empty;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemExit_Click(object sender, EventArgs e)
        {
            if (!this.Disposing)
            {
                this.Dispose();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemOpen_Click(object sender, EventArgs e)
        {
            openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog.ShowDialog();
            this.currentFile = openFileDialog.FileName;
            if (!string.IsNullOrEmpty(this.currentFile))
            {
                using (var file = File.OpenText(this.currentFile))
                {
                    this.textBoxMainText.Text = file.ReadToEnd();
                }
            }

            this.Text = String.Format(CultureInfo.CurrentCulture, "{0} - {1}", AssemblyTitle, this.currentFile);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemSave_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.currentFile))
            {
                using (var file = new StreamWriter(saveFileDialog.FileName))
                {
                    file.Write(textBoxMainText.Text);
                }
            }
            else
            {
                this.SaveTextToFile();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemSaveas_Click(object sender, EventArgs e)
        {
            this.SaveTextToFile();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemPageSetup_Click(object sender, EventArgs e)
        {
            this.pageSetupDialog = new PageSetupDialog();
            this.pageSetupDialog.Document = null == this.printDocument ? new PrintDocument() : this.printDocument;
            this.pageSetupDialog.ShowDialog();
            this.pageSettings = this.pageSetupDialog.PageSettings.Clone() as PageSettings;
            this.printDocument = this.pageSetupDialog.Document;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemPrint_Click(object sender, EventArgs e)
        {
            if (null == this.printDocument)
            {
                this.printDocument = new PrintDocument();
            }

            if (null == this.pageSettings)
            {
                this.pageSettings = new PageSettings();
            }

            this.printDocument.PrintPage += PrintPage;
            this.printDialog = new PrintDialog();
            this.printDialog.ShowDialog();
            this.printDocument.Print();
            this.printDocument.PrintPage -= PrintPage;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PrintPage(object sender, PrintPageEventArgs e)
        {
            e.Graphics.DrawString(this.textBoxMainText.Text, this.textBoxMainText.Font, Brushes.Black, this.pageSettings.Bounds);
        }
    }
}