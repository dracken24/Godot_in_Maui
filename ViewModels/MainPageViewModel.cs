using System.Collections.ObjectModel;
using System.Windows.Input;
using PlatformGame.Services;

namespace PlatformGame.ViewModels;

public class MainPageViewModel : BaseViewModel
{
    private readonly IGameApiService _gameApiService;
    private readonly IGodotLauncherService _godotLauncherService;
    private readonly IAuthService _authService;
    private readonly INavigationService _navigationService;

    private string _statsText = "Chargement...";
    private string _bestTimeText = "Aucun résultat";
    private string _topResultsText = "Chargement...";
    private bool _isLoading;

    public MainPageViewModel(
        IGameApiService gameApiService,
        IGodotLauncherService godotLauncherService,
        IAuthService authService,
        INavigationService navigationService)
    {
        _gameApiService = gameApiService;
        _godotLauncherService = godotLauncherService;
        _authService = authService;
        _navigationService = navigationService;

        Title = "Platform Game";
        LoadResultsCommand = new Command(async () => await LoadResultsAsync());
        LaunchGodotCommand = new Command(async () => await LaunchGodotAsync(), () => !IsBusy);
    }

    public string StatsText
    {
        get => _statsText;
        set => SetProperty(ref _statsText, value);
    }

    public string BestTimeText
    {
        get => _bestTimeText;
        set => SetProperty(ref _bestTimeText, value);
    }

    public string TopResultsText
    {
        get => _topResultsText;
        set => SetProperty(ref _topResultsText, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            SetProperty(ref _isLoading, value);
            IsBusy = value;
            ((Command)LaunchGodotCommand).ChangeCanExecute();
        }
    }

    public ICommand LoadResultsCommand { get; }
    public ICommand LaunchGodotCommand { get; }

    public async Task LoadResultsAsync()
    {
        try
        {
            IsLoading = true;

            // Charger les statistiques
            try
            {
                var stats = await _gameApiService.GetStatsAsync();
                if (stats != null && stats.TotalAttempts > 0)
                {
                    StatsText = $"Total de tentatives: {stats.TotalAttempts}\n" +
                               $"Temps moyen: {stats.AverageTimeFormatted}\n" +
                               $"Meilleur temps: {stats.BestTimeFormatted}";
                }
                else
                {
                    StatsText = "Aucune statistique disponible\n(Aucun résultat enregistré)";
                }
            }
            catch (Exception)
            {
                StatsText = "Aucune statistique disponible";
            }

            // Charger le meilleur temps
            try
            {
                var bestResult = await _gameApiService.GetBestResultAsync();
                if (bestResult != null)
                {
                    var username = string.IsNullOrEmpty(bestResult.Username) ? "Anonyme" : bestResult.Username;
                    BestTimeText = $"{bestResult.CompletionTimeFormatted}\n" +
                                  $"Par: {username}\n" +
                                  $"Plateforme: {bestResult.Platform ?? "N/A"}\n" +
                                  $"Date: {bestResult.CreatedAt:dd/MM/yyyy HH:mm}";
                }
                else
                {
                    BestTimeText = "Aucun résultat enregistré";
                }
            }
            catch (Exception)
            {
                BestTimeText = "Aucun résultat enregistré";
            }

            // Charger le top 10
            try
            {
                var topResults = await _gameApiService.GetTopResultsAsync(10);
                if (topResults != null && topResults.Any())
                {
                    var topResultsText = string.Join("\n", topResults.Select((r, index) =>
                    {
                        var username = string.IsNullOrEmpty(r.Username) ? "Anonyme" : r.Username;
                        return $"{index + 1}. {r.CompletionTimeFormatted} - {username} ({r.Platform ?? "N/A"})";
                    }));
                    TopResultsText = topResultsText;
                }
                else
                {
                    TopResultsText = "Aucun résultat disponible";
                }
            }
            catch (Exception)
            {
                TopResultsText = "Aucun résultat disponible";
            }
        }
        catch (Exception ex)
        {
            await Application.Current!.MainPage!.DisplayAlert("Erreur",
                $"Impossible de charger les résultats: {ex.Message}\n\n" +
                "Vérifiez que l'API est démarrée et accessible.",
                "OK");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LaunchGodotAsync()
    {
        try
        {
            await _godotLauncherService.LaunchGodotAsync();
        }
        catch (Exception ex)
        {
            await Application.Current!.MainPage!.DisplayAlert("Erreur", $"Impossible de lancer le jeu: {ex.Message}", "OK");
        }
    }
}


