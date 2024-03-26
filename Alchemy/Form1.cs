using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace Alchemy
{
    public partial class Form1 : Form
    {
        List<Element> elements = new List<Element>();
        List<Element> activeElements = new List<Element>();
        Element SelectedElement;
        Element CreatedElement, CreatedElement_2;
        int indexValue;
        int yPos = 5;
        int scrollDistance = 0;
        List<string> imageLocation = new List<string>();
        int elementNumber = -1;
        int totalElements = 0;
        int lineAnimation = 0;
        int activeNumber = 0;
        int lineAnimation_2 = 0;
        SoundPlayer rightSound = new SoundPlayer(@"sounds\right.wav");
        SoundPlayer wrongSound = new SoundPlayer(@"sounds\wrong.wav");

        public Form1()
        {
            FormBorderStyle = FormBorderStyle.FixedSingle;
            string[] lines = File.ReadAllLines("Elements.txt");
            this.elementChart = new int[lines.Length, lines[0].Split(' ').Length];
            for (int i = 0; i < lines.Length; i++)
            {
                string[] temp = lines[i].Split(' ');
                for (int j = 0; j < temp.Length; j++)
                    elementChart[i, j] = Convert.ToInt32(temp[j]);
            }
            InitializeComponent();
            SetUpApp();
            vScrollBar1.Minimum = 0;
            vScrollBar1.Maximum = totalElements+8;
        }

        private void SetUpApp()
        {
            imageLocation = Directory.GetFiles("elements", "*_*.png").ToList();
            totalElements = imageLocation.Count;

            for (int i = 0; i < 4; i++) 
            {
                MakeElements(true);
            }
            for (int i = 4; i < totalElements; i++)
            {
                MakeElements(false);
            }
        }

        private void MakeElements(bool isElementDiscovered)
        {
            elementNumber++;
            Element newElement;
            if (isElementDiscovered)
                newElement = new Element(imageLocation[elementNumber], isElementDiscovered, elementNumber);
            else
                newElement = new Element(Directory.GetFiles("elements", "Unavailable.png").ToList()[0], isElementDiscovered, elementNumber);
            newElement.position.X = 500;
            newElement.position.Y = yPos;
            newElement.rect.X = newElement.position.X;
            newElement.rect.Y = newElement.position.Y;
            yPos += 75;
            elements.Add(newElement);
        }

        private void MakeActiveElement(int index)
        {
            Element newElement = new Element(imageLocation[index], true, index);
            newElement.position.X = 500;
            newElement.position.Y = 5 + (index - scrollDistance) * 75;
            newElement.rect.X = newElement.position.X;
            newElement.rect.Y = newElement.position.Y;
            activeElements.Add(newElement);
        }

        private void DeleteActiveElement(int index)
        {
            activeElements.RemoveAt(index);
        }

        private void FormMouseDown(object sender, MouseEventArgs e)
        {
            Point mousePosition = new Point(e.X, e.Y);
            foreach (Element newElement in elements)
            {
                if (SelectedElement == null)
                {
                    if (newElement.rect.Contains(mousePosition) && newElement.discovered && activeNumber < 100)
                    {
                        MakeActiveElement(elements.IndexOf(newElement));
                        SelectedElement = activeElements[activeElements.Count - 1];
                        activeElements[activeElements.Count - 1].active = true;
                        if (newElement.position.X < 1000 && newElement.position.X > 400)
                            activeNumber++;
                        if (activeNumber > 100)
                            activeNumber = 100;
                    }
                }
            }
            foreach (Element newElement in activeElements)
            {
                if (SelectedElement == null)
                {
                    if (newElement.rect.Contains(mousePosition) && 
                        !(newElement.position.X < 1000 && newElement.position.X > 400))
                    {
                        SelectedElement = newElement;
                        newElement.active = true;
                    }
                }
            }
        }

        private void FormMouseMove(object sender, MouseEventArgs e)
        {
            if (SelectedElement != null)
            {
                SelectedElement.position.X = e.X - (SelectedElement.width / 2);
                SelectedElement.position.Y = e.Y - (SelectedElement.height / 2);
            }
        }

        private void FormMouseUp(object sender, MouseEventArgs e)
        {
            bool flag = false;
            foreach (Element tempElement in activeElements)
            {
                if (tempElement.active && tempElement.position.X < 1000 && tempElement.position.X > 400)
                {
                    activeNumber--;
                    DeleteActiveElement(activeElements.IndexOf(tempElement));
                    break;
                }
                else
                {
                    foreach(Element secondElement in activeElements)
                    {
                        if (secondElement != tempElement && secondElement.rect.IntersectsWith(tempElement.rect))
                            if (elementChart[secondElement.index, tempElement.index] != -1)
                            {
                                activeNumber--;

                                indexValue = elementChart[secondElement.index, tempElement.index];
                                if (elements[indexValue].discovered == false)
                                    elements[indexValue].elementPic = Image.FromFile(imageLocation[indexValue]);
                                elements[indexValue].discovered = true;

                                MakeActiveElement(indexValue);
                                CreatedElement = elements[indexValue];
                                CreatedElement_2 = activeElements[activeElements.Count - 1];
                                activeElements[activeElements.Count - 1].rect.X = tempElement.rect.X;
                                activeElements[activeElements.Count - 1].rect.Y = tempElement.rect.Y;
                                activeElements[activeElements.Count - 1].position.X = tempElement.position.X;
                                activeElements[activeElements.Count - 1].position.Y = tempElement.position.Y;

                                DeleteActiveElement(activeElements.IndexOf(tempElement));
                                DeleteActiveElement(activeElements.IndexOf(secondElement));
                                rightSound.Play();

                                flag = true;
                                break;
                            }
                            else 
                            {
                                wrongSound.Play(); 
                            }
                    }
                    if (flag)
                        break;
                }
                tempElement.active = false;
            }
            SelectedElement = null;
            lineAnimation = 0;
        }

        private void FormPaintEvent(object sender, PaintEventArgs e)
        {
            foreach (Element element in elements)
            {
                e.Graphics.DrawImage(element.elementPic, element.position.X, element.position.Y, element.width, element.height);
                Pen outline, blackOutline;
                blackOutline = new Pen(Color.Black, 1);
                if (element.active)
                {
                    outline = new Pen(Color.Blue, lineAnimation);
                }
                else if (element == CreatedElement)
                {
                    if (lineAnimation_2 < 5)
                        outline = new Pen(Color.Green, lineAnimation_2);
                    else
                        outline = new Pen(Color.Green, 10 - lineAnimation_2);
                }
                else
                {
                    outline = new Pen(Color.Transparent, 1);
                }
                e.Graphics.DrawRectangle(blackOutline, element.rect);
                e.Graphics.DrawRectangle(outline, element.rect);
            }
            foreach (Element element in activeElements)
            {
                e.Graphics.DrawImage(element.elementPic, element.position.X, element.position.Y, element.width, element.height);
                Pen outline, blackOutline;
                blackOutline = new Pen(Color.Black, 1);
                if (element.active)
                {
                    outline = new Pen(Color.Blue, lineAnimation);
                }
                else if (element == CreatedElement_2)
                {
                    if (lineAnimation_2 < 5)
                        outline = new Pen(Color.Green, lineAnimation_2);
                    else 
                        outline = new Pen(Color.Green, 10 - lineAnimation_2);
                }
                else
                {
                    outline = new Pen(Color.Transparent, 1);
                }
                e.Graphics.DrawRectangle(blackOutline, element.rect);
                e.Graphics.DrawRectangle(outline, element.rect);
            }
            if (SelectedElement != null)
            {
                e.Graphics.DrawImage(SelectedElement.elementPic, SelectedElement.position.X, SelectedElement.position.Y, SelectedElement.width, SelectedElement.height);
            }
        }
        private void FormTimerEvent(object sender, EventArgs e)
        {
            foreach (Element element in elements)
            {
                element.rect.X = element.position.X;
                element.rect.Y = element.position.Y;
            }
            foreach (Element element in activeElements)
            {
                element.rect.X = element.position.X;
                element.rect.Y = element.position.Y;
            }
            if (SelectedElement != null)
            {
                if (lineAnimation < 5)
                {
                    lineAnimation++;
                }
            }
            if (CreatedElement != null)
            {
                if (lineAnimation_2 < 10)
                {
                    lineAnimation_2++;
                }
                else
                {
                    lineAnimation_2 = 0;
                CreatedElement = null;
                CreatedElement_2 = null;
                }
            }
            this.Invalidate();
        }
        private void button1_Click_1(object sender, EventArgs e)
        {
            int count = activeElements.Count;
            for (int i = 0; i < count; i++)
            {
                DeleteActiveElement(0);
                activeNumber--;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            foreach (Element tempElement in elements)
            {
                if (tempElement.position.X < 1000 && tempElement.position.X > 400)
                {
                    scrollDistance = vScrollBar1.Value - 1;
                    if (scrollDistance < 0)
                    { scrollDistance = 0; }
                    tempElement.position.X = 500;
                    tempElement.position.Y = 5 + (elements.IndexOf(tempElement) - scrollDistance) * 75;
                    tempElement.rect.X = tempElement.position.X;
                    tempElement.rect.Y = tempElement.position.Y;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int count = activeElements.Count;
            for (int i = 0; i < count; i++)
            {
                DeleteActiveElement(0);
                activeNumber--;
            }
            for (int i = 4; i < elements.Count; i++)
            {
                elements[i].discovered = false;
                elements[i].elementPic = Image.FromFile(Directory.GetFiles("elements", "Unavailable.png").ToList()[0]);
            }
        }
    }
}
