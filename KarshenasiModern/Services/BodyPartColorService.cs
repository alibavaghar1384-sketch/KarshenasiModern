using System.Windows.Media;
using KarshenasiModern.Models;

namespace KarshenasiModern.Services
{
    public static class BodyPartColorService
    {
        public static Brush GetColor(BodyPartStatus status)
        {
            return status switch
            {
                BodyPartStatus.Healthy => Brushes.White,

                BodyPartStatus.Painted => Brushes.Red,

                BodyPartStatus.Replaced => Brushes.LimeGreen,

                BodyPartStatus.TouchUp => Brushes.DodgerBlue,

                BodyPartStatus.Sunburned => Brushes.Gold,

                BodyPartStatus.Damaged => Brushes.Gray,

                BodyPartStatus.Repaired => Brushes.MediumPurple,
                    
                BodyPartStatus.PDR => Brushes.DarkTurquoise,

                BodyPartStatus.Scratched => Brushes.SaddleBrown,

                _ => Brushes.White
            };
        }
    }
}