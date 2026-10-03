using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Controls;
using NativeSupport.Enums;

namespace Shotora.App.Models.ViewModels;

[ExcludeFromCodeCoverage]
public class TrayMenuWindowModel
{
	public Window Window { get; private set; } = new();

	public Button CaptureRegionButton { get; private set; } = new();
	public Button CaptureFullButton   { get; private set; } = new();
	public Button SettingsButton      { get; private set; } = new();
	public Button AboutButton         { get; private set; } = new();
	public Button UpdatesButton       { get; private set; } = new();
	public Button ExitButton          { get; private set; } = new();
	public Border Root                { get; private set; } = new();
	public Border Separator           { get; private set; } = new();

	public PixelPoint LastAnchor { get; set; }
	public RuntimeOs  CurrentOs  { get; set; } = RuntimeOs.Other;

	public void Init(Window window,
					 Button captureRegionButton,
					 Button captureFullButton,
					 Button settingsButton,
					 Button aboutButton,
					 Button updatesButton,
					 Button exitButton,
					 Border root,
					 Border separator)
	{
		Window              = window;
		CaptureRegionButton = captureRegionButton;
		CaptureFullButton   = captureFullButton;
		SettingsButton      = settingsButton;
		AboutButton         = aboutButton;
		UpdatesButton       = updatesButton;
		ExitButton          = exitButton;
		Root                = root;
		Separator           = separator;
	}
}
