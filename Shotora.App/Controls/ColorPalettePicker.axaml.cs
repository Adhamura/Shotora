using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Shotora.App.Models.Utilities;

namespace Shotora.App.Controls;

[ExcludeFromCodeCoverage]
public partial class ColorPalettePicker : UserControl
{
	public ColorPalettePicker()
	{
		InitializeComponent();
	}

	#region Events

	public event EventHandler<Color>? ColorChanged;

	#endregion

	protected override void OnLoaded(RoutedEventArgs e)
	{
		base.OnLoaded(e);
		InitializeControls();
		InitializeFromColor(Color);
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);

		if (change.Property == ColorProperty && !_updatingFromColor)
		{
			InitializeFromColor((Color)change.NewValue!);
		}
		else if (change.Property == OriginalColorProperty)
		{
			UpdateOriginalColorPreview();
		}
	}

	#region Styled Properties

	public static readonly StyledProperty<Color> ColorProperty =
		AvaloniaProperty.Register<ColorPalettePicker, Color>(nameof(Color), Colors.Red,
			defaultBindingMode: BindingMode.TwoWay);

	public static readonly StyledProperty<bool> AlphaEnabledProperty =
		AvaloniaProperty.Register<ColorPalettePicker, bool>(nameof(AlphaEnabled));

	public static readonly StyledProperty<bool> ShowOriginalColorProperty =
		AvaloniaProperty.Register<ColorPalettePicker, bool>(nameof(ShowOriginalColor));

	public static readonly StyledProperty<Color> OriginalColorProperty =
		AvaloniaProperty.Register<ColorPalettePicker, Color>(nameof(OriginalColor), Colors.Red);

	public Color Color
	{
		get => GetValue(ColorProperty);
		set => SetValue(ColorProperty, value);
	}

	public bool AlphaEnabled
	{
		get => GetValue(AlphaEnabledProperty);
		set => SetValue(AlphaEnabledProperty, value);
	}

	public bool ShowOriginalColor
	{
		get => GetValue(ShowOriginalColorProperty);
		set => SetValue(ShowOriginalColorProperty, value);
	}

	public Color OriginalColor
	{
		get => GetValue(OriginalColorProperty);
		set => SetValue(OriginalColorProperty, value);
	}

	#endregion

	#region Private Fields

	private double _hue;
	private double _saturation = 1;
	private double _value      = 1;
	private double _alpha      = 1;

	private bool _isSvDragging;
	private bool _isHueDragging;
	private bool _isAlphaDragging;
	private bool _updatingFromColor;
	private bool _updatingInputs;

	private Grid?      _svPanel;
	private Ellipse?   _svThumb;
	private Ellipse?   _svThumbInner;
	private Rectangle? _hueBackground;
	private Grid?      _hueSliderContainer;
	private Border?    _hueThumb;
	private Grid?      _alphaSliderContainer;
	private Rectangle? _alphaSlider;
	private Border?    _alphaThumb;
	private TextBox?   _rInput;
	private TextBox?   _gInput;
	private TextBox?   _bInput;
	private TextBox?   _aInput;
	private TextBox?   _hexInput;
	private Rectangle? _currentColorPreview;
	private Rectangle? _originalColorPreview;

	#endregion

	#region Initialization

	private void InitializeControls()
	{
		_svPanel              = this.FindControl<Grid>("SvPanel");
		_svThumb              = this.FindControl<Ellipse>("SvThumb");
		_svThumbInner         = this.FindControl<Ellipse>("SvThumbInner");
		_hueBackground        = this.FindControl<Rectangle>("HueBackground");
		_hueSliderContainer   = this.FindControl<Grid>("HueSliderContainer");
		_hueThumb             = this.FindControl<Border>("HueThumb");
		_alphaSliderContainer = this.FindControl<Grid>("AlphaSliderContainer");
		_alphaSlider          = this.FindControl<Rectangle>("AlphaSlider");
		_alphaThumb           = this.FindControl<Border>("AlphaThumb");
		_rInput               = this.FindControl<TextBox>("RInput");
		_gInput               = this.FindControl<TextBox>("GInput");
		_bInput               = this.FindControl<TextBox>("BInput");
		_aInput               = this.FindControl<TextBox>("AInput");
		_hexInput             = this.FindControl<TextBox>("HexInput");
		_currentColorPreview  = this.FindControl<Rectangle>("CurrentColorPreview");
		_originalColorPreview = this.FindControl<Rectangle>("OriginalColorPreview");

		if (_svPanel != null)
		{
			_svPanel.PointerPressed  += SvPanel_PointerPressed;
			_svPanel.PointerMoved    += SvPanel_PointerMoved;
			_svPanel.PointerReleased += SvPanel_PointerReleased;
			_svPanel.SizeChanged     += SvPanel_SizeChanged;
		}

		if (_hueSliderContainer != null)
		{
			_hueSliderContainer.PointerPressed  += HueSlider_PointerPressed;
			_hueSliderContainer.PointerMoved    += HueSlider_PointerMoved;
			_hueSliderContainer.PointerReleased += HueSlider_PointerReleased;
			_hueSliderContainer.SizeChanged     += HueSlider_SizeChanged;
		}

		if (_alphaSliderContainer != null)
		{
			_alphaSliderContainer.PointerPressed  += AlphaSlider_PointerPressed;
			_alphaSliderContainer.PointerMoved    += AlphaSlider_PointerMoved;
			_alphaSliderContainer.PointerReleased += AlphaSlider_PointerReleased;
			_alphaSliderContainer.SizeChanged     += AlphaSlider_SizeChanged;
		}
	}

	private void InitializeFromColor(Color color)
	{
		_updatingFromColor = true;

		var hsv = ColorConversions.RgbToHsv(color);
		_hue        = hsv.H;
		_saturation = hsv.S;
		_value      = hsv.V;
		_alpha      = hsv.A;

		UpdateHueBackground();
		UpdateAlphaGradient();
		UpdatePreview();
		PositionThumbs();
		UpdateInputs();

		_updatingFromColor = false;
	}

	#endregion

	#region SV Panel

	private void SvPanel_PointerPressed(object? sender, PointerPressedEventArgs e)
	{
		if (_svPanel == null)
		{
			return;
		}

		var point = e.GetCurrentPoint(_svPanel);
		if (!point.Properties.IsLeftButtonPressed)
		{
			return;
		}

		_isSvDragging = true;
		e.Pointer.Capture(_svPanel);
		UpdateSvFromPointer(point.Position);
		e.Handled = true;
	}

	private void SvPanel_PointerMoved(object? sender, PointerEventArgs e)
	{
		if (_isSvDragging && _svPanel != null)
		{
			var point = e.GetCurrentPoint(_svPanel);
			if (!point.Properties.IsLeftButtonPressed)
			{
				StopSvDrag(e.Pointer);
				return;
			}
			UpdateSvFromPointer(point.Position);
			e.Handled = true;
		}
	}

	private void SvPanel_PointerReleased(object? sender, PointerReleasedEventArgs e)
	{
		if (!_isSvDragging)
		{
			return;
		}

		StopSvDrag(e.Pointer);
		e.Handled = true;
	}

	private void SvPanel_SizeChanged(object? sender, SizeChangedEventArgs e)
	{
		PositionSvThumb();
	}

	private void UpdateSvFromPointer(Point position)
	{
		if (_svPanel == null)
		{
			return;
		}

		var width  = _svPanel.Bounds.Width;
		var height = _svPanel.Bounds.Height;

		if (width <= 0 || height <= 0)
		{
			return;
		}

		_saturation = Math.Clamp(position.X / width, 0, 1);
		_value      = 1 - Math.Clamp(position.Y / height, 0, 1);

		ApplyColorChange();
		PositionSvThumb();
	}

	private void PositionSvThumb()
	{
		if (_svPanel == null || _svThumb == null || _svThumbInner == null)
		{
			return;
		}

		var width  = _svPanel.Bounds.Width;
		var height = _svPanel.Bounds.Height;

		var thumbWidth  = _svThumb.Width;
		var thumbHeight = _svThumb.Height;

		var x = _saturation  * width  - thumbWidth  / 2;
		var y = (1 - _value) * height - thumbHeight / 2;

		x = Math.Clamp(x, -thumbWidth  / 2, width  - thumbWidth  / 2);
		y = Math.Clamp(y, -thumbHeight / 2, height - thumbHeight / 2);

		Canvas.SetLeft(_svThumb, x);
		Canvas.SetTop(_svThumb, y);
		Canvas.SetLeft(_svThumbInner, x + 1);
		Canvas.SetTop(_svThumbInner, y  + 1);
	}

	#endregion

	#region Hue Slider

	private void HueSlider_PointerPressed(object? sender, PointerPressedEventArgs e)
	{
		if (_hueSliderContainer == null)
		{
			return;
		}

		if (!e.GetCurrentPoint(_hueSliderContainer).Properties.IsLeftButtonPressed)
		{
			return;
		}

		_isHueDragging = true;
		e.Pointer.Capture(_hueSliderContainer);
		UpdateHueFromPointer(e.GetPosition(_hueSliderContainer));
		e.Handled = true;
	}

	private void HueSlider_PointerMoved(object? sender, PointerEventArgs e)
	{
		if (_isHueDragging && _hueSliderContainer != null)
		{
			if (!e.GetCurrentPoint(_hueSliderContainer).Properties.IsLeftButtonPressed)
			{
				StopHueDrag(e.Pointer);
				return;
			}
			UpdateHueFromPointer(e.GetPosition(_hueSliderContainer));
			e.Handled = true;
		}
	}

	private void HueSlider_PointerReleased(object? sender, PointerReleasedEventArgs e)
	{
		if (!_isHueDragging)
		{
			return;
		}

		StopHueDrag(e.Pointer);
		e.Handled = true;
	}

	private void HueSlider_SizeChanged(object? sender, SizeChangedEventArgs e)
	{
		PositionHueThumb();
	}

	private void UpdateHueFromPointer(Point position)
	{
		if (_hueSliderContainer == null)
		{
			return;
		}

		var width = _hueSliderContainer.Bounds.Width;
		if (width <= 0)
		{
			return;
		}

		_hue = Math.Clamp(position.X / width, 0, 1);

		UpdateHueBackground();
		UpdateAlphaGradient();
		ApplyColorChange();
		PositionHueThumb();
	}

	private void PositionHueThumb()
	{
		if (_hueSliderContainer == null || _hueThumb == null)
		{
			return;
		}

		var width      = _hueSliderContainer.Bounds.Width;
		var thumbWidth = _hueThumb.Width;

		var x = _hue * width - thumbWidth / 2;
		x = Math.Clamp(x, 0, width - thumbWidth);

		Canvas.SetLeft(_hueThumb, x);
	}

	private void UpdateHueBackground()
	{
		_hueBackground?.Fill = new SolidColorBrush(ColorConversions.ColorFromHue(_hue));
	}

	#endregion

	#region Alpha Slider

	private void AlphaSlider_PointerPressed(object? sender, PointerPressedEventArgs e)
	{
		if (_alphaSliderContainer == null)
		{
			return;
		}

		if (!e.GetCurrentPoint(_alphaSliderContainer).Properties.IsLeftButtonPressed)
		{
			return;
		}

		_isAlphaDragging = true;
		e.Pointer.Capture(_alphaSliderContainer);
		UpdateAlphaFromPointer(e.GetPosition(_alphaSliderContainer));
		e.Handled = true;
	}

	private void AlphaSlider_PointerMoved(object? sender, PointerEventArgs e)
	{
		if (_isAlphaDragging && _alphaSliderContainer != null)
		{
			if (!e.GetCurrentPoint(_alphaSliderContainer).Properties.IsLeftButtonPressed)
			{
				StopAlphaDrag(e.Pointer);
				return;
			}
			UpdateAlphaFromPointer(e.GetPosition(_alphaSliderContainer));
			e.Handled = true;
		}
	}

	private void AlphaSlider_PointerReleased(object? sender, PointerReleasedEventArgs e)
	{
		if (!_isAlphaDragging)
		{
			return;
		}

		StopAlphaDrag(e.Pointer);
		e.Handled = true;
	}

	private void StopSvDrag(IPointer pointer)
	{
		_isSvDragging = false;
		pointer.Capture(null);
	}

	private void StopHueDrag(IPointer pointer)
	{
		_isHueDragging = false;
		pointer.Capture(null);
	}

	private void StopAlphaDrag(IPointer pointer)
	{
		_isAlphaDragging = false;
		pointer.Capture(null);
	}

	private void AlphaSlider_SizeChanged(object? sender, SizeChangedEventArgs e)
	{
		PositionAlphaThumb();
	}

	private void UpdateAlphaFromPointer(Point position)
	{
		if (_alphaSliderContainer == null)
		{
			return;
		}

		var width = _alphaSliderContainer.Bounds.Width;
		if (width <= 0)
		{
			return;
		}

		_alpha = Math.Clamp(position.X / width, 0, 1);

		ApplyColorChange();
		PositionAlphaThumb();
	}

	private void PositionAlphaThumb()
	{
		if (_alphaSliderContainer == null || _alphaThumb == null)
		{
			return;
		}

		var width      = _alphaSliderContainer.Bounds.Width;
		var thumbWidth = _alphaThumb.Width;

		var x = _alpha * width - thumbWidth / 2;
		x = Math.Clamp(x, 0, width - thumbWidth);

		Canvas.SetLeft(_alphaThumb, x);
	}

	private void UpdateAlphaGradient()
	{
		if (_alphaSlider == null)
		{
			return;
		}

		var baseColor = ColorConversions.HsvToRgb(_hue, _saturation, _value);

		_alphaSlider.Fill = new LinearGradientBrush
		{
			StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
			EndPoint   = new RelativePoint(1, 0, RelativeUnit.Relative),
			GradientStops =
			{
				new GradientStop(Color.FromArgb(0,   baseColor.R, baseColor.G, baseColor.B), 0),
				new GradientStop(Color.FromArgb(255, baseColor.R, baseColor.G, baseColor.B), 1)
			}
		};
	}

	#endregion

	#region Text Input Handlers

	private void RgbInput_LostFocus(object? sender, RoutedEventArgs e)
	{
		CommitRgbInput();
	}

	private void RgbInput_KeyDown(object? sender, KeyEventArgs e)
	{
		if (e.Key == Key.Enter)
		{
			CommitRgbInput();
			e.Handled = true;
		}
		else if (e.Key == Key.Escape)
		{
			UpdateInputs();
			e.Handled = true;
		}
	}

	private void CommitRgbInput()
	{
		if (_updatingInputs)
		{
			return;
		}

		var r = ColorConversions.TryParseByte(_rInput?.Text, out var rVal) ? rVal : Color.R;
		var g = ColorConversions.TryParseByte(_gInput?.Text, out var gVal) ? gVal : Color.G;
		var b = ColorConversions.TryParseByte(_bInput?.Text, out var bVal) ? bVal : Color.B;
		var a = AlphaEnabled && ColorConversions.TryParseByte(_aInput?.Text, out var aVal) ? aVal : Color.A;

		var newColor = Color.FromArgb(a, r, g, b);
		var hsv      = ColorConversions.RgbToHsv(newColor);

		_hue        = hsv.H;
		_saturation = hsv.S;
		_value      = hsv.V;
		_alpha      = hsv.A;

		UpdateHueBackground();
		UpdateAlphaGradient();
		ApplyColorChange();
		PositionThumbs();
	}

	private void HexInput_LostFocus(object? sender, RoutedEventArgs e)
	{
		CommitHexInput();
	}

	private void HexInput_KeyDown(object? sender, KeyEventArgs e)
	{
		if (e.Key == Key.Enter)
		{
			CommitHexInput();
			e.Handled = true;
		}
		else if (e.Key == Key.Escape)
		{
			UpdateInputs();
			e.Handled = true;
		}
	}

	private void CommitHexInput()
	{
		if (_updatingInputs || _hexInput == null)
		{
			return;
		}

		if (ColorConversions.TryParseHex(_hexInput.Text, out var color))
		{
			if (!AlphaEnabled)
			{
				color = Color.FromArgb(255, color.R, color.G, color.B);
			}

			var hsv = ColorConversions.RgbToHsv(color);
			_hue        = hsv.H;
			_saturation = hsv.S;
			_value      = hsv.V;
			_alpha      = hsv.A;

			UpdateHueBackground();
			UpdateAlphaGradient();
			ApplyColorChange();
			PositionThumbs();
		}
		else
		{
			UpdateInputs();
		}
	}

	#endregion

	#region Color Update

	private void ApplyColorChange()
	{
		_updatingFromColor = true;

		var newColor = ColorConversions.HsvToRgb(_hue, _saturation, _value, AlphaEnabled ? _alpha : 1);
		Color = newColor;

		UpdatePreview();
		UpdateInputs();

		ColorChanged?.Invoke(this, newColor);

		_updatingFromColor = false;
	}

	private void UpdatePreview()
	{
		_currentColorPreview?.Fill = new SolidColorBrush(Color);
	}

	private void UpdateOriginalColorPreview()
	{
		_originalColorPreview?.Fill = new SolidColorBrush(OriginalColor);
	}

	private void UpdateInputs()
	{
		_updatingInputs = true;

		_rInput?.Text   = Color.R.ToString();
		_gInput?.Text   = Color.G.ToString();
		_bInput?.Text   = Color.B.ToString();
		_aInput?.Text   = Color.A.ToString();
		_hexInput?.Text = ColorConversions.ToHex(Color, AlphaEnabled)[1..];

		_updatingInputs = false;
	}

	private void PositionThumbs()
	{
		PositionSvThumb();
		PositionHueThumb();
		PositionAlphaThumb();
	}

	#endregion
}
