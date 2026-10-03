# Shotora design system

This document is the reference for Shotora's desktop UI (Avalonia 11.3). It records the audit of the UI as it was
before the design-system pass, the tokens and components that now exist, and the rules for building new UI.

- **Where things live**
  - `Shotora.App/Styles/Controls.axaml`: theme-independent tokens (spacing, radii, type scale, control metrics) and
    every shared control style. Reference tokens with `{StaticResource …}`.
  - `Shotora.App/Styles/Theme.axaml` (Dark), `LightTheme.axaml`, `SunsetTheme.axaml`: colour brushes only. Each theme
    defines **exactly the same keys**. Reference brushes with `{DynamicResource …}` so runtime theme switching works
    (`App.ApplyTheme` swaps the theme `Styles` instance).
  - `Shotora.App/Styles/Icons.axaml`: shared icon geometries (`IconUndo`, `IconCopy`, …), merged into the application
    resources by `Controls.axaml`.
  - `Shotora.App/Controls/DialogShell.cs`: window chrome for every custom-decorated window.
  - `Shotora.App/Controls/LabeledField.cs`: form row (label + field) for settings-style forms.
  - `Shotora.App/Controls/IconContentTemplate.cs`: renders `Geometry` content as an icon that follows the button's
    foreground.

---

## 1. Audit (state before this pass)

### 1.1 Tokens

| Area | Finding |
| --- | --- |
| Colour | Themes defined 38 brushes, but views also used brushes that no theme defined (`ThemeBorderBrush` in the text dialog, so the preview border never showed) and hard-coded colours (`#FF3478FF` check box, `#80000000` disabled check box text, which is invisible in Dark, `Lime` for "installed", `#999` picker labels). |
| Accent | No separate accent-button or link brushes. Links used `AccentPrimaryBrush`; on Dark that is **2.3:1** contrast against the panel, which fails WCAG AA. |
| Sunset | All buttons, tabs and the overlay toolbar used the same orange gradient, so primary and secondary actions looked identical. The panel gradient ended in dark terracotta, so `TextPrimaryBrush` fell to **2.4:1** and `TextSecondaryBrush` to **1.6:1** in the lower half of every window. `ButtonPressedBrush` had two malformed 6-digit stops. |
| Spacing | Ad-hoc margins everywhere: 4, 6, 8, 10, 12, 14, 16, 18 and 20 px. Window padding was 10, 14 or 16 px depending on the window. |
| Radii | 2, 3, 4, 6, 8, 10, 12 and 20 px were all in use. The `Button` template hard-coded `CornerRadius="4"`, so the `rounded-action` class's `CornerRadius=6` had no effect. |
| Typography | Window titles were 15 or 16 px in SemiBold or Bold. Section headers were also 16 px Medium, so they read at the same level as titles. |
| Control heights | Button `Height` was 25, 28 or 32 px, set per instance. Inputs were 28 or 34 px. Combo boxes were 28 px, with a 30 px fixed drop-down button. |
| Button padding | The template ignored `Padding`, so every button needed a fixed `Width` (70, 90, 100, 110 or 120 px) and long translations were clipped. |

### 1.2 Components and interaction

- **Button styles**: there was a plain `Button`, a `rounded-action` class that looked the same, a `tray-menu-item`
  class, and an `accent` class used in the OCR window. `accent` was never styled by Shotora, so it fell through to
  Fluent's blue (`#0078D7`). No dialog had a visual primary action.
- **States**: buttons had no focus visual. The global `Button TextBlock { Foreground }` rule broke foreground changes
  for hover, accent and disabled states. Check boxes had no focus visual and a non-themed checked colour. Tabs had no
  focus visual. Toolbar buttons had no hover state that differed from pressed, and were all solid accent circles, so
  the checked tool was hard to tell apart.
- **Tooltips**: none of the 17 icon-only overlay toolbar buttons had a tooltip. The colour-picker tooltip was
  hard-coded English (`"More colors"`).
- **Hard-coded English**: `"Preview"` and `"Sample"` in the text dialog.
- **Links (About)**: the links were `TextBlock`s with a `PointerReleased` handler. They were not focusable and had no
  keyboard activation or focus visual, and code-behind called `Process.Start` itself.
- **Keyboard**: Esc and Enter were not handled in any dialog. Esc in an open searchable combo box did not mark the key
  as handled. The text dialog had no keyboard shortcut to confirm.
- **Window chrome**: Main, Settings and About each re-implemented the same drag and resize code-behind (3 × 25
  lines). The text dialog was drag-only and the OCR window could not be moved. Title rows differed in size, weight
  and margin, there was no close affordance in the title bar, and footer buttons were in a different order in each
  window (primary first in the text dialog).
- **Duplication**: the 18 icon geometries were copied into both `SettingsWindow` and `OverlayWindow`. The
  `editor-circle` and `circle` styles were near-duplicates. `TrayMenuWindow.axaml` was dead code (the tray menu is
  built in `TrayMenuWindowService`). `LabeledField` was unused and broken: its `ContentPresenter` presented the
  control's own content, so it recursed.
- **Unused global styles**: there were full templates for `ComboBox`, `ComboBoxItem`, `TabControl` and `TabItem`,
  although none of these controls is used anywhere.

### 1.3 Responsiveness

- Settings rows used fixed `Width="140"` labels and `Width="240"` fields. German and Finnish labels such as
  "Kopieren + Schließen" were clipped or crowded, and the window had a horizontal scroll bar.
- The JPEG quality value, the EasyOCR status and the Tesseract status drifted out of alignment (an `Auto,20,*,Auto`
  grid pushed the status text to the far right).
- In About, the description ran under the scroll bar.
- The text dialog was a fixed 520×520 window with a large empty area.
- Multi-line text boxes could not scroll, because the global `TextBox` style disabled their scroll bars.
- The overlay toolbar was a single 836 px row with no wrapping, so it overflowed on screens narrower than about
  850 px.
- The Theme combo box was empty on first run, because `AppSettings.Theme` defaulted to `null`.

---

## 2. Tokens

### 2.1 Spacing (4-pt scale)

| Key | Value | Use |
| --- | --- | --- |
| `SpaceXs` | 4 | label ↔ field in stacked layouts, tight icon gaps |
| `SpaceSm` | 8 | between buttons, between form rows |
| `SpaceMd` | 12 | between groups, card padding |
| `SpaceLg` | 16 | dialog padding, section top margin, footer padding |
| `SpaceXl` | 24 | large separations |

Rule: use 4, 8, 12, 16 or 24 px for margins, padding and spacing. Avalonia `Spacing` and `ColumnSpacing` take a
`double`, so use the token or the literal value from the scale.

### 2.2 Corner radii

| Key | Value | Use |
| --- | --- | --- |
| `RadiusSm` | 4 | check boxes, swatches, list items in pop-ups |
| `RadiusMd` | 6 | buttons, text boxes, combo boxes, toggles, tooltips |
| `RadiusLg` | 8 | cards, pop-ups, context menus, settings icon options |
| `RadiusXl` | 12 | window shells (`DialogShell`), floating panels, overlay toolbar |

### 2.3 Typography

The font is `AppFontFamily` (bundled KazukiReiwa), and `Window` sets it for the whole tree.

| Key / class | Size | Weight | Use |
| --- | --- | --- | --- |
| `TextBlock.display` | 24 (`FontSizeDisplay`) | Normal | product name (About) |
| `TextBlock.title` | 18 (`FontSizeTitle`) | SemiBold | window title (set by `DialogShell`) |
| `TextBlock.section-header` | 15 (`FontSizeSubtitle`) | SemiBold | settings sections (`.first` removes the top margin) |
| body (default) | 14 (`FontSizeBody`) | Normal | everything else |
| `TextBlock.label` | 14 | SemiBold | inline labels (About details) |
| `TextBlock.secondary` | 14 | Normal, `TextSecondaryBrush`, wraps | supporting text |
| `TextBlock.caption` | 12 (`FontSizeCaption`) | Normal, `TextSecondaryBrush`, wraps | hints, copyright |

### 2.4 Control metrics

| Key | Value | Use |
| --- | --- | --- |
| `ControlHeight` | 32 | min height of buttons, text boxes, combo boxes, check boxes, list items |
| `ButtonMinWidth` | 96 | min width of dialog footer buttons (`StackPanel.dialog-actions > Button`) |
| `IconButtonSize` | 32 | square icon-only buttons (`Button.icon`) |
| `ToolButtonSize` | 40 | overlay toolbar buttons (`.tool`) and settings icon options |
| `FormLabelMaxWidth` | 220 | label column cap in `LabeledField` (longer labels wrap) |
| `FormFieldMaxWidth` | 380 | field column cap in `LabeledField` |
| `ButtonPadding` | 16,6 | default button padding |
| `InputPadding` | 10,6 | text box padding |
| `FocusRingThickness` | 2 | focus rings |

### 2.5 Colour brushes (defined in every theme)

| Group | Keys |
| --- | --- |
| Surfaces | `WindowBackgroundBrush`, `AppBackgroundBrush`, `PanelBackgroundBrush` (window shell), `PanelAltBackgroundBrush` (floating overlay panels), `PanelBorderBrush`, `CardBackgroundBrush`, `DividerBrush`, `WindowOverlayBackgroundBrush`, `OverlayMaskBrush` |
| Text | `TextPrimaryBrush`, `TextSecondaryBrush`, `TextDisabledBrush`, `IconBrush` |
| Inputs | `InputBackgroundBrush`, `InputBackgroundHoverBrush`, `InputBorderBrush`, `InputBorderHoverBrush`, `InputForegroundBrush` |
| Secondary buttons | `ButtonBackgroundBrush`, `ButtonBorderBrush`, `ButtonHoverBrush`, `ButtonPressedBrush`, `ButtonForegroundBrush`, `ButtonDisabledBrush`, `ButtonDisabledForegroundBrush` |
| Subtle | `SubtleHoverBrush`, `SubtlePressedBrush` (translucent overlays for ghost buttons, tabs, menu items, title-bar buttons) |
| Accent | `AccentPrimaryBrush`, `AccentPrimaryDarkBrush`, `AccentSecondaryBrush`, `AccentForegroundBrush` (on `AccentPrimaryBrush` fills), `AccentButtonBackgroundBrush`, `AccentButtonHoverBrush`, `AccentButtonPressedBrush`, `AccentButtonBorderBrush`, `AccentButtonForegroundBrush`, `SelectionBorderBrush`, `FocusRingBrush` |
| Status | `DangerBrush`, `DangerHoverBrush`, `DangerPressedBrush`, `DangerForegroundBrush`, `SuccessBrush`, `LinkBrush`, `LinkHoverBrush` |
| Chrome | `ScrollBarBackgroundBrush`, `ScrollBarThumbBrush`, `ScrollBarThumbHoverBrush`, `PopupBackgroundBrush`, `PopupBorderBrush`, `HighlightBrush` |
| Sliders | `SliderTrackBrush`, `SliderFillBrush`, `SliderThumbBrush`, `SliderThumbBorderBrush`, `SliderDisabledThumbBrush`, `SliderValueBrush` |

**Contrast checks (WCAG 2.1, against the window panel):**

| Theme | Pair | Ratio |
| --- | --- | --- |
| Dark | text | 14.2 |
| Dark | secondary text | 8.5 |
| Dark | link | 6.7 (was 2.3) |
| Dark | accent button text | 6.1 |
| Dark | focus ring | 8.0 |
| Light | text | 15.1 |
| Light | secondary text | 6.8 |
| Light | link | 6.9 |
| Light | focus ring | 11.1 |
| Sunset (darkest point of the gradient) | text | 6.4 (was 2.4) |
| Sunset (darkest point of the gradient) | secondary text | 5.0 (was 1.6) |
| Sunset (darkest point of the gradient) | link | 5.5 |
| Sunset (darkest point of the gradient) | accent button text | 5.7 |
| Sunset (darkest point of the gradient) | toolbar icons | 5.3 |

The Sunset gradients were lightened to reach these values. Sunset now uses a cream secondary button and keeps the
orange gradient for the primary (accent) button only.

---

## 3. Components

### 3.1 Buttons

| Class | Look | Use |
| --- | --- | --- |
| *(none)* | neutral fill and border (secondary) | every non-primary action |
| `accent` | accent fill, `AccentButtonForegroundBrush`, SemiBold | **the one primary action of a dialog** |
| `danger` | red fill | destructive actions (Tesseract "Remove" when the language is installed: `Classes.danger="{Binding …IsInstalled}"`) |
| `subtle` | transparent until hover | low-emphasis actions, title-bar close |
| `icon` | 32×32 square, icon content | icon-only buttons; combine with `subtle`. **Always** set `ToolTip.Tip` and `AutomationProperties.Name` |
| `menu-item` | full-width, left-aligned, transparent | tray menu entries |
| `swatch` | 22×22 colour chip | overlay quick colours |
| `tool` (Button / ToggleButton) | 40 px round, transparent, icon only; checked = accent fill; `tool accent` = primary | overlay editor toolbar |

All buttons share one template (`PART_ButtonBorder` plus a `PART_FocusRing` overlay). It honours `Padding` and
`CornerRadius`, uses `MinHeight = ControlHeight`, shows a hand cursor and has hover, pressed, disabled and
`:focus-visible` states. Do not name a presenter `PART_ContentPresenter` in a button template: Fluent's built-in
`Button.accent` and state selectors target that name and would override Shotora's colours.

Content may be a string or an icon `Geometry` (`Content="{StaticResource IconCopy}"`). `IconContentTemplate` renders
the geometry in the button's current foreground.

`HyperlinkButton` is used for links. It sets `NavigateUri`, so there is no code-behind and it is keyboard and screen
reader accessible. It is styled with `LinkBrush` / `LinkHoverBrush`, an underline and a focus ring.

### 3.2 Toggles

- `ToggleButton.toggle`: a two-state text toggle (Bold / Italic). Checked uses the accent fill.
- `ToggleButton.icon-option`: a row with a round icon and a wrapping label (Settings › Editor icons). `Content` is the
  label and `Tag` is the icon `Geometry`. The whole row is clickable. Unchecked dims the icon.
- `CheckBox`: a themed box with an accent fill when checked, a focus ring and a wrapping label.

### 3.3 Inputs

- `TextBox`: 32 px minimum height, `RadiusMd`. Border states: hover uses `InputBorderHoverBrush`, focus uses
  `AccentPrimaryBrush`. Selection uses the accent colour. Multi-line text boxes (`AcceptsReturn=True`) align content
  to the top and scroll vertically.
- `SearchableComboBox`: matches `TextBox` metrics, with hover and focus-within border states and a chevron without a
  divider. The pop-up uses `RadiusLg` and list items use `RadiusSm` with subtle hover. Esc closes the pop-up first and
  marks the key handled; a second Esc reaches the dialog.
- `Slider.quality`: accent fill, round 16 px thumb, focus border on the thumb.
- `ProgressBar`: 6 px, accent fill.

### 3.4 Layout primitives

- **`DialogShell`** (window chrome). Use it as the root of every custom-decorated window:

  ```xml
  <controls:DialogShell x:Name="Shell" Title="{DynamicResource LocXxxTitle}">
      <controls:DialogShell.Footer>
          <StackPanel Classes="dialog-actions">
              <Button Content="{DynamicResource LocButtonCancel}" IsCancel="True" Click="…" />
              <Button Classes="accent" Content="{DynamicResource LocButtonOk}" IsDefault="True" Click="…" />
          </StackPanel>
      </controls:DialogShell.Footer>
      …content…
  </controls:DialogShell>
  ```

  - It provides a 12 px radius, a 1 px border, a title bar (18 px SemiBold title, optional `Subtitle`, and a subtle
    32 px close button that is not a tab stop) and a footer with 16 px padding.
  - The whole surface is a drag area. Set `Shell.ResizeService = <IMouseInteractionService>` in code-behind to enable
    edge resizing.
  - Footer content stays in the logical tree, so localized `DynamicResource`s and `IsDefault` / `IsCancel` work.
- **`StackPanel.dialog-actions`**: a right-aligned, 8 px spaced action row. Buttons in it get
  `MinWidth = ButtonMinWidth`. Put the secondary action first and the primary (`accent`) action last, on the right.
- **`LabeledField`** (forms):

  ```xml
  <StackPanel Grid.IsSharedSizeScope="True">
      <controls:LabeledField Label="{DynamicResource LocSettingsTheme}"> …field… </controls:LabeledField>
      <controls:LabeledField> <CheckBox …/> </controls:LabeledField>   <!-- no label: aligns with the field column -->
  </StackPanel>
  ```

  Labels share one auto-sized column (capped at 220 px, wrapping beyond that). Fields stretch up to 380 px.
- **`Border.card`**: card background, 1 px border, `RadiusLg`, 12 px padding.
- **`Border.floating-panel`**: `PanelAltBackgroundBrush`, `RadiusXl`, for overlay toolbars and palettes.
- **`CustomTabs`**: underline tabs on a divider. The selected tab has a 3 px accent indicator and primary text,
  unselected tabs have secondary text and a subtle hover. Tabs wrap onto a second line if the labels are long.
  Keyboard: arrow keys move between tabs, and tabs have a focus ring.

### 3.5 Keyboard and accessibility rules

1. **Esc** closes every dialog: put `IsCancel="True"` on the dismiss button. The overlay keeps its own Esc handling,
   which leaves the current tool first and closes on the second press.
2. **Enter** triggers the primary action where it is unambiguous (`IsDefault="True"`): About, Main, Text dialog and
   OCR Copy. Settings has no default button, because Enter is used inside its combo boxes. In the multi-line text
   dialog, **Ctrl+Enter** places the text.
3. Every icon-only control has `ToolTip.Tip` and `AutomationProperties.Name` from existing `Loc*` keys.
4. Focus is always visible. Interactive templates use a 2 px `FocusRingBrush` ring on `:focus-visible`, and the
   square Fluent `FocusAdorner` is disabled for those controls so only one ring shows.
5. Never hard-code user-visible English in XAML. Use existing `Loc*` keys. If a new key is unavoidable, add it to
   all 70 `Strings.*.axaml` files.
6. Text that can be translated must wrap or trim: `TextWrapping="Wrap"` on body copy, and labels go through
   `LabeledField`.

### 3.6 Window inventory

| Window | Size / min | Chrome | Footer (secondary → primary) | Keys |
| --- | --- | --- | --- | --- |
| MainWindow | 420×260 / 320×220, resizable | DialogShell | Close (accent) | Enter / Esc close |
| SettingsWindow | 680×720 / 560×440, resizable | DialogShell + CustomTabs, each tab scrolls | Close (accent, saves) | Esc closes (and saves) |
| AboutWindow | 460×640 / 380×420, resizable | DialogShell, scrolling body; `VersionSection` grid is the extension point for update checks | Close (accent) | Enter / Esc close |
| TextInputWindow | 520 wide, height sizes to content | DialogShell | Cancel → Place text (accent) | Esc cancel, Enter (single-line fields) / Ctrl+Enter place |
| OcrResultWindow | 480×260, positioned next to the selection | DialogShell (drag only) | Close → Copy (accent) | Esc close, Enter copy |
| OverlayWindow | full screen | floating toolbar (wraps), coordinates chip, draggable palette panel | n/a | existing editor hotkeys |
| Tray menu | sized to content | built in `TrayMenuWindowService` | n/a | n/a |

---

## 4. Known gaps and follow-ups

- **Tray menu theming**: `TrayMenuWindowService.ApplyTheme` still sets brushes in code, and unit tests cover that
  behaviour. The idle and hover visuals of entries now come from `Button.menu-item`. A later clean-up could move the
  root and separator brushes to XAML as well.
- **Colour picker**: `ColorPalettePicker` keeps functional hard-coded colours (hue and alpha gradients, checkerboard,
  thumb outlines). These are content, not chrome.
- **Toolbar grouping**: the overlay toolbar has no group separators, because `ApplyIconVisibility` can hide any button
  and that would leave orphaned dividers. Grouping would need visibility-aware separators in code-behind.
- **Windows without a compositor**: windows use transparent backgrounds with rounded shells. Where the platform cannot
  provide per-pixel transparency, the corners render on black, as they did before this pass.
