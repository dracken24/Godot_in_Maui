using PlatformGame.ViewModels;

namespace PlatformGame;

public partial class MainPage : ContentPage
{
    private readonly MainPageViewModel _viewModel;

    public MainPage(MainPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
        
        // Charger les résultats au démarrage
        _ = _viewModel.LoadResultsAsync();
    }

}
