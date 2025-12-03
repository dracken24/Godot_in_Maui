using PlatformGame.ViewModels;

namespace PlatformGame.Views;

public partial class SignUpPage : ContentPage
{
    private readonly SignUpPageViewModel _viewModel;

    public SignUpPage(SignUpPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }
}
