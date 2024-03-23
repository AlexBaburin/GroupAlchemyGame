using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alchemy
{
    internal class Element
    {
        public Image elementPic;
        public int width;
        public int height;
        public int index;
        public Point position = new Point();
        public bool active = false;
        public bool discovered = false;
        public Rectangle rect;

        public Element(string imageLocation, bool isDiscovered, int index) 
        {
            this.index = index;
            discovered = isDiscovered;
            elementPic = Image.FromFile(imageLocation);
            width = 100;
            height = 50;
            rect = new Rectangle(position.X, position.Y, width, height);
        }
    }
}
