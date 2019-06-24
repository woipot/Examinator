using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Examinator.other
{
    class MarkClass
    {
        public static string DeffautBlockName = "Mark";

        public int FivePercent { get; private set; }
        public int FourPercent { get; private set; }
        public int ThreePercent { get; private set; }

        public void SetAllParam(int paramFive, int paramFour, int paramThree)
        {
            if ((paramFive <= paramFour) || (paramFour <= paramThree) || (paramFive <= 0) || (paramFour <= 0) || (paramThree <= 0))
            {
                throw new TestException("Параметры для оценки указанны не верно");
            } else
            {
                FivePercent = paramFive;
                FourPercent = paramFour;
                ThreePercent = paramThree;
            }
        }

        public MarkClass ()
        {
            SetAllParam(100, 80, 70);
        }

        public static MarkClass fromXML(XDocument doc, string documentName)
        {
            var mainElement = doc.Element(documentName);
            if (mainElement == null)
                throw new TestException("Файл с оценками испорчен");

            var fiveMark = mainElement.Attribute("FiveMark");
            var fourMark = mainElement.Attribute("FourMark");
            var threeMark = mainElement.Attribute("ThreeMark");

            MarkClass markClass = new MarkClass();
            int five = -1;
            int four = -1;
            int three = -1;
            if (fiveMark != null) {
                if (int.TryParse(fiveMark.Value, out var fiveint))
                    five = fiveint;
                else
                    throw new TestException("Некорректный параметр оценки"); 
            }
            if (fourMark != null)
            {
                if (int.TryParse(fourMark.Value, out var fourint))
                    four = fourint;
                else
                    throw new TestException("Некорректный параметр оценки");
            }
            if (threeMark != null)
            {
                if (int.TryParse(threeMark.Value, out var threeint))
                    three = threeint;
                else
                    throw new TestException("Некорректный параметр оценки");
            }

            markClass.SetAllParam(five, four, three);
            return markClass;
        }
    }
}
