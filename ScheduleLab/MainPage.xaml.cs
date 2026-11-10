using ScheduleLab.ViewModels;

namespace ScheduleLab;


public partial class MainPage : ContentPage
{
    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
