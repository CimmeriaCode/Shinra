﻿using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ShinraCo.Properties;

namespace ShinraCo.Settings.Forms
{
    public partial class ShinraOverlay : Form
    {
        private readonly Image _shinraLogo = Resources.ShinraLogo;

        public ShinraOverlay()
        {
            InitializeComponent();
        }

        #region Draggable

        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;
        private const int WM_EXITSIZEMOVE = 0x0232;

        private void ShinraOverlay_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }
            ReleaseCapture();
            SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_EXITSIZEMOVE)
            {
                Shinra.Settings.OverlayLocation = Location;
                Shinra.Settings.Save();
            }
            base.WndProc(ref m);
        }

        #endregion

        #region Always On Top

        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

        #endregion

        private void ShinraOverlay_Load(object sender, EventArgs e)
        {
            ShinraLogo.Image = _shinraLogo;
            SetWindowPos(Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
            Location = Shinra.Settings.OverlayLocation;
        }

        public void UpdateText()
        {
            RotationModeLabel.Text = $@"[Rotation] {Convert.ToString(Shinra.Settings.RotationMode)}";
            CooldownModeLabel.Text = $@"[Cooldown] {Convert.ToString(Shinra.Settings.CooldownMode)}";
            TankModeLabel.Text = $@"[Tank] {Convert.ToString(Shinra.Settings.TankMode)}";
        }
    }
}