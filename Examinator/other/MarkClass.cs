using System.Xml.Linq;

namespace Examinator.other
{
    public class MarkClass
    {
        public static string DeffautBlockName = "Mark";
        public static string DeffautFileName = "marks.db";

        public int FivePercent { get; private set; }
        public int FourPercent { get; private set; }
        public int ThreePercent { get; private set; }

        public void SetAllParam(int paramFive, int paramFour, int paramThree)
        {
            if ((paramFive <= paramFour) || (paramFour <= paramThree) || (paramFive <= 0) || (paramFour <= 0) || (paramThree <= 0))
            {
                throw new TestException("Параметры для оценки указаны неверно");
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

        public static XDocument toXML(MarkClass marks)
        {
            var xdoc = new XDocument();

            var test = new XElement(DeffautBlockName);
            var testFive = new XAttribute("FiveMark", marks.FivePercent);
            var testFour = new XAttribute("FourMark", marks.FourPercent);
            var testThree = new XAttribute("ThreeMark", marks.ThreePercent);
            test.Add(testFive);
            test.Add(testFour);
            test.Add(testThree);

            xdoc.Add(test);

            return xdoc;
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
