﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ARKBreedingStats.ocr
{
    class OCRLetterEdit : PictureBox
    {
        private uint[] letterArray;
        private uint[] letterArrayComparing;
        public bool drawingEnabled;
        private bool overlay;

        public OCRLetterEdit()
        {
            letterArray = new uint[21];
            letterArrayComparing = new uint[21];
            drawingEnabled = false;
            BorderStyle = BorderStyle.FixedSingle;
            overlay = false;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.FillRectangle(Brushes.Black, 0, 0, 100, 100);
            int pxSize = 5;
            for (int y = 0; y < letterArray.Length - 1; y++)
            {
                uint row = letterArray[y + 1];
                uint rowC = letterArrayComparing[y + 1];
                int x = 0;
                while (row > 0 || rowC > 0)
                {
                    if (!overlay)
                    {
                        if ((row & 1) == 1)
                            e.Graphics.FillRectangle(Brushes.White, x * pxSize, y * pxSize, pxSize, pxSize);
                    }
                    else
                    {
                        if ((row & 1) == 1 && (rowC & 1) == 1)
                            e.Graphics.FillRectangle(Brushes.White, x * pxSize, y * pxSize, pxSize, pxSize);
                        else if ((row & 1) == 1 && (rowC & 1) == 0)
                            e.Graphics.FillRectangle(Brushes.LightGreen, x * pxSize, y * pxSize, pxSize, pxSize);
                        else if ((row & 1) == 0 && (rowC & 1) == 1)
                            e.Graphics.FillRectangle(Brushes.DarkRed, x * pxSize, y * pxSize, pxSize, pxSize);
                    }
                    row = row >> 1;
                    rowC = rowC >> 1;
                    x++;
                }
            }
        }

        public uint[] LetterArray
        {
            set
            {
                if (value != null)
                {
                    for (int y = 0; y < 21; y++)
                    {
                        if (y < value.Length)
                            letterArray[y] = value[y];
                        else letterArray[y] = 0;
                    }
                    Invalidate();
                }
            }
            get
            {
                int l = 0;
                for (int y = 0; y < 21; y++)
                    if (letterArray[y] > 0)
                        l = y;
                l++;
                uint[] lArray = new uint[l];
                for (int y = 0; y < l; y++)
                    lArray[y] = letterArray[y];
                return lArray;
            }
        }

        public uint[] LetterArrayComparing
        {
            set
            {
                if (value != null)
                {
                    overlay = false;
                    for (int y = 0; y < 20; y++)
                    {
                        if (y < value.Length)
                        {
                            letterArrayComparing[y] = value[y];
                            if (value[y] > 0)
                                overlay = true;
                        }
                        else letterArrayComparing[y] = 0;
                    }
                    Invalidate();
                }
            }
        }

        internal void Clear()
        {
            LetterArrayComparing = new uint[0];
            LetterArray = new uint[0];
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (drawingEnabled)
            {
                Point p = e.Location;
                int x = p.X / 5;
                int y = p.Y / 5 + 1; // first row is array-length
                while (letterArray.Length < y) letterArray[letterArray.Length] = 0;
                // toggle pixel
                letterArray[y] ^= (uint)(1 << x);
                Invalidate();
            }
        }
    }
}
