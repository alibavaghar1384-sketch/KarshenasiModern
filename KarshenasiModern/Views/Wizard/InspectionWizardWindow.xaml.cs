using KarshenasiModern.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace KarshenasiModern.Views.Wizard;

public partial class InspectionWizardWindow : Window
{
    public InspectionWizardWindow()
    {
        InitializeComponent();

        // تضمین پاکسازی سشن هنگام بسته شدن پنجره به هر طریقی
        this.Closed += InspectionWizardWindow_Closed;

        ShowStep1();
    }

    private void InspectionWizardWindow_Closed(object? sender, System.EventArgs e)
    {
        // ریست کردن آیدی سشن جاری برای جلوگیری از تداخل در استفاده‌های بعدی
        CurrentSession.CurrentInspectionId = 0;
    }

    public ContentControl WizardHost
        => WizardContent;

    public void ShowStep1()
    {
        SetStep(1);

        WizardContent.Content =
            new Step1CustomerView();
    }

    public void ShowStep2()
    {
        SetStep(2);

        WizardContent.Content =
            new Step2InspectionView();
    }

    public void ShowStep3()
    {
        SetStep(3);

        WizardContent.Content =
            new Step3CameraView();
    }

    public void ShowStep4()
    {
        SetStep(4);

        WizardContent.Content =
            new Step4BodyView();
    }

    public void ShowStep5()
    {
        SetStep(5);

        WizardContent.Content =
            new Step5PreviewView();
    }

    public void SetStep(int step)
    {
        ResetSteps();

        switch (step)
        {
            case 1:
                Step1Text.Foreground =
                    Brushes.DodgerBlue;

                Step1Text.FontWeight =
                    FontWeights.Bold;
                break;

            case 2:
                Step2Text.Foreground =
                    Brushes.DodgerBlue;

                Step2Text.FontWeight =
                    FontWeights.Bold;
                break;

            case 3:
                Step3Text.Foreground =
                    Brushes.DodgerBlue;

                Step3Text.FontWeight =
                    FontWeights.Bold;
                break;

            case 4:
                Step4Text.Foreground =
                    Brushes.DodgerBlue;

                Step4Text.FontWeight =
                    FontWeights.Bold;
                break;

            case 5:
                Step5Text.Foreground =
                    Brushes.DodgerBlue;

                Step5Text.FontWeight =
                    FontWeights.Bold;
                break;
        }
    }

    private void ResetSteps()
    {
        Step1Text.Foreground =
            Brushes.Gray;

        Step2Text.Foreground =
            Brushes.Gray;

        Step3Text.Foreground =
            Brushes.Gray;

        Step4Text.Foreground =
            Brushes.Gray;

        Step5Text.Foreground =
            Brushes.Gray;

        Step1Text.FontWeight =
            FontWeights.Normal;

        Step2Text.FontWeight =
            FontWeights.Normal;

        Step3Text.FontWeight =
            FontWeights.Normal;

        Step4Text.FontWeight =
            FontWeights.Normal;

        Step5Text.FontWeight =
            FontWeights.Normal;
    }
}