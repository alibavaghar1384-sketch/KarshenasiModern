using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using KarshenasiModern.Models;
using KarshenasiModern.Services;

namespace KarshenasiModern.Views.Controls
{
    public partial class CarBodyDiagram : UserControl
    {
        public CarBodyDiagram()
        {
            InitializeComponent();
        }

        public void SetPartStatus(string partName, BodyPartStatus status)
        {
            List<Shape> parts = new();

            switch (partName)
            {
                case "سپر جلو":
                    AddShape(parts, FrontBumperPart);
                    AddShape(parts, FrontViewBumperPart);
                    break;

                case "کاپوت":
                    AddShape(parts, HoodPart);
                    AddShape(parts, FrontViewHoodPart);
                    break;

                case "سقف":
                    AddShape(parts, RoofPart);
                    AddShape(parts, FrontViewRoofPart);
                    AddShape(parts, RearViewRoofPart);
                    break;

                case "صندوق":
                case "صندوق عقب":
                    AddShape(parts, TrunkPart);
                    AddShape(parts, RearViewTrunkPart);
                    break;

                case "سپر عقب":
                    AddShape(parts, RearBumperPart);
                    AddShape(parts, RearViewBumperPart);
                    break;

                case "درب جلو راننده":
                case "در جلو راننده":
                    AddShape(parts, FrontLeftDoorPart);
                    break;

                case "درب جلو شاگرد":
                case "در جلو شاگرد":
                    AddShape(parts, FrontRightDoorPart);
                    break;

                case "درب عقب راننده":
                case "در عقب راننده":
                    AddShape(parts, RearLeftDoorPart);
                    break;

                case "درب عقب شاگرد":
                case "در عقب شاگرد":
                    AddShape(parts, RearRightDoorPart);
                    break;

                case "گلگیر جلو راننده":
                    AddShape(parts, FrontLeftFenderPart);
                    break;

                case "گلگیر جلو شاگرد":
                    AddShape(parts, FrontRightFenderPart);
                    break;

                case "گلگیر عقب راننده":
                    AddShape(parts, RearLeftFenderPart);
                    break;

                case "گلگیر عقب شاگرد":
                    AddShape(parts, RearRightFenderPart);
                    break;

                case "رکاب راننده":
                case "رکاب سمت راننده":
                    AddShape(parts, LeftSillPart);
                    break;

                case "رکاب شاگرد":
                case "رکاب سمت شاگرد":
                    AddShape(parts, RightSillPart);
                    break;

                case "شاسی جلو راننده":
                    AddShape(parts, FrontLeftChassisPart);
                    break;

                case "شاسی جلو شاگرد":
                    AddShape(parts, FrontRightChassisPart);
                    break;

                case "شاسی عقب راننده":
                    AddShape(parts, RearLeftChassisPart);
                    break;

                case "شاسی عقب شاگرد":
                    AddShape(parts, RearRightChassisPart);
                    break;

                case "سینی جلو":
                    AddShape(parts, FrontPanelPart);
                    break;

                case "سینی عقب":
                    AddShape(parts, RearPanelPart);
                    break;
            }

            Brush brush = GetColor(status);

            foreach (Shape shape in parts)
            {
                if (shape != null)
                {
                    shape.Fill = brush;
                }
            }
        }

        private static void AddShape(List<Shape> list, Shape shape)
        {
            if (shape != null)
                list.Add(shape);
        }

        private Brush GetColor(BodyPartStatus status)
        {
            // استفاده مستقیم از سرویس متمرکز مدیریت رنگ پروژه
            return BodyPartColorService.GetColor(status);
        }
    }
}