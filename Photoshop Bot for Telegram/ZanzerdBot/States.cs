using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ZanzerdBot
{
    class States
    {
        private List<Bitmap> data;
        private int pointer;

        public States(Bitmap img)
        {
            data = new List<System.Drawing.Bitmap>();
            data.Add(img);
            pointer = 0;
        }

        public Bitmap Peek()
        {
            return data[pointer];
        }
        public void Add(Bitmap img)
        {
            pointer++;
            if (pointer >= data.Count)
                data.Add(img);
            else
                data[pointer] = img;
        }

        public void Cancel()
        {
            pointer--;
        }

        public bool CanCancel()
        {
            return pointer - 1 >= 0;
        }
        public void Redo()
        {
            pointer++;
        }
    }
}
