using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SleepHaven;

public sealed class SeasonSection : INotifyPropertyChanged
{
    private bool _isExpanded;

    public SeasonSection(string key, string title, string summary, IEnumerable<Product> products, bool isExpanded = false)
    {
        Key = key;
        Title = title;
        Summary = summary;
        Products = new ObservableCollection<Product>(products);
        _isExpanded = isExpanded;
    }

    public string Key { get; }
    public string Title { get; }
    public string Summary { get; }
    public ObservableCollection<Product> Products { get; }

    public bool IsExpanded
    {
        get => _isExpanded;
        set
        {
            if (_isExpanded == value)
            {
                return;
            }

            _isExpanded = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ToggleSymbol));
        }
    }

    public string ToggleSymbol => IsExpanded ? "−" : "+";

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

public sealed record CarouselItem(
    string Id,
    string ImageUrl,
    string Number,
    string Eyebrow,
    string Title,
    string Summary);
