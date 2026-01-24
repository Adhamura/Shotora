using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Shotora.App.Models.Utilities;

[ExcludeFromCodeCoverage]
public sealed class TesseractLanguageOption(string code, string englishName, string? localizationKey = null) : INotifyPropertyChanged
{
	public string  Code            { get; } = code;
	public string  EnglishName     { get; } = englishName;
	public string? LocalizationKey { get; } = localizationKey;

	public string DisplayName
	{
		get;
		set
		{
			if (field == value)
			{
				return;
			}
			field = value;
			OnPropertyChanged(nameof(DisplayName));
		}
	} = string.Empty;

	public string StatusText
	{
		get;
		set
		{
			if (field == value)
			{
				return;
			}
			field = value;
			OnPropertyChanged(nameof(StatusText));
		}
	} = string.Empty;

	public bool IsDownloading
	{
		get;
		set
		{
			if (field == value)
			{
				return;
			}
			field = value;
			OnPropertyChanged(nameof(IsDownloading));
		}
	}

	public bool IsInstalled
	{
		get;
		set
		{
			if (field == value)
			{
				return;
			}
			field = value;
			OnPropertyChanged(nameof(IsInstalled));
		}
	}

	public double Progress
	{
		get;
		set
		{
			if (Math.Abs(field - value) < 0.0001)
			{
				return;
			}
			field = value;
			OnPropertyChanged(nameof(Progress));
		}
	}

	public bool CanDownload
	{
		get;
		set
		{
			if (field == value)
			{
				return;
			}
			field = value;
			OnPropertyChanged(nameof(CanDownload));
		}
	}

	public event PropertyChangedEventHandler? PropertyChanged;

	private void OnPropertyChanged(string propertyName)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
