using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Shotora.App.Interfaces.PeripheralServices;
using Shotora.App.Services.ViewModels;

namespace Shotora.App.Views;

[ExcludeFromCodeCoverage]
public partial class UpdateWindow : Window
{
	private readonly UpdateViewModel _viewModel;

	public UpdateWindow(UpdateViewModel viewModel, IMouseInteractionService mouseInteractionService)
	{
		_viewModel  = viewModel;
		DataContext = viewModel;
		InitializeComponent();
		Shell.ResizeService       =  mouseInteractionService;
		_viewModel.CloseRequested += OnCloseRequested;
	}

	public UpdateViewModel ViewModel => _viewModel;

	protected override void OnClosed(EventArgs e)
	{
		_viewModel.CloseRequested -= OnCloseRequested;
		_viewModel.Dispose();
		base.OnClosed(e);
	}

	private void OnCloseRequested(object? sender, EventArgs e)
	{
		Close();
	}
}
