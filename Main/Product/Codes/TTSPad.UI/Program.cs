//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="Contoso Corporation">
//     Copyright (c) Contoso Corporation. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TTSPad.UI
{
    using System;
    using System.Windows.Forms;
    using GY.TTSPad.UI;

    /// <summary>
    ///
    /// </summary>
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
