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

### 1.4 Review round 2 (design critique) — what changed

| ID | Finding | Resolution |
| --- | --- | --- |
| M1 | Focus ring invisible on accent / checked fills (1.99:1) | Rings are drawn **outside** the control (`FocusRingMargin` = -3, radius + 3 via `FocusRingRadiusConverter`), leaving a 1 px panel-coloured gap; measured against the panel (8.0 / 11.1 / 9.4). Templates set `ClipToBounds=False`; scroll pages are padded so rings are not clipped. |
| M2 | Sunset primary text 3.97:1 at the gradient end; secondary text too close to primary | Accent gradients lightened (worst stop ≥ 5.5:1 for normal / hover / pressed); panel gradient end `#F0C0A0`; `TextPrimary #3A1C14`, `TextSecondary #74463A` (4.8:1 vs panel, 2.0:1 vs primary); `Success #1F5214`. Table below reports the **worst** stop. |
| M3 | Checked tool and primary Copy identical; Copy lost on Sunset toolbar | Checked tool = tinted (`AccentSubtleBrush` + accent border + `AccentTextBrush` icon) with its own hover; solid accent only for Copy; Save / Copy / Cancel grouped after a separator; Sunset `PanelAltBackgroundBrush` is now a solid cream. |
| M4 | Bold toggle (on by default) competes with "Place text" | `ToggleButton.toggle` uses the same tinted checked treatment. Text labels kept (they are localized; single-letter glyphs would not be). |
| M5 | Check-box focus ring clipped | Settings pages are pulled out 4 px and padded back in, so outside rings fit inside the scroll viewer. |
| m1 | Whole shell was a drag area (incl. pop-ups, blank scroll areas) | Drag only from `PART_TitleBar` and empty footer space; events whose source is in another `TopLevel` (pop-ups) are ignored. |
| m2 | New `Cursor` per pointer move | Cursors cached per edge, assigned only when the hovered edge changes, disposed on detach. |
| m3 | 32 vs 34 px control heights | `ButtonPadding` 16,4 / `InputPadding` 10,4 so text + padding + border fits in `ControlHeight` (32) everywhere. |
| m4 | Label column jumps between tabs; empty labels indent 16 px | Labels have `FormLabelMinWidth` (160); an empty `Label` collapses its cell (no margin). |
| m5 | Dead space at 680 px; uneven rhythm around slider rows | `FormFieldMaxWidth` 460; `Slider.quality` height = `ControlHeight`. |
| m6 | 15 solid accent circles in Settings › Editor | `CheckBox.icon-option`: standard check box + neutral icon + label; accent only in the check mark. |
| m7 | Swatches / combo boxes lacked accessible names | Swatches get hex tooltip + automation name; `LabeledField` exposes its label as the field's automation name and `SearchableComboBox` forwards it to its inner text box. |
| m8 | OCR: read-only `AcceptsReturn` swallowed Enter; unguarded async handler; no feedback | Read-only text uses `TextBox.multiline`; Copy is wrapped in try/catch and briefly shows the localized "Copied to clipboard". |
| m9 | Tray: dead brush/hover code, hard-coded metrics, invisible separator, 37 px rows | Tray entries are purely style-driven (`Button.menu-item`, 30 px rows); separator uses `Border.menu-separator` / `DividerBrush`; `HoverHandlers` removed; tests updated. |
| m13 | DIP / pixel mix in tray and OCR positioning | `TrayMenuWindowService.CalculateMenuPosition` (unit-tested at 100 % / 200 %) and `OcrResultWindow` convert DIPs with the screen / render scaling; the overlay passes the OCR anchor in pixels. |
| P1 | About: redundant heading, indented links, mixed headers | Header is logo + tagline (`TextBlock.subtitle`); links align with text (padding cancelled by margin); Updates / Links / Contact use `section-header`. |
| P2 | Overlay second row, "(0,0) 0x0" chip, no hover on checked tools | `WrapPanel ItemsAlignment=Center`; chip hidden until the first pointer update; checked-hover state added. |
| P4 | Unused tokens | Removed `AccentSecondaryBrush`, `AppBackgroundBrush`, `WindowBackgroundBrush`, `WindowOverlayBackgroundBrush`, `SliderValueBrush`, `ButtonDisabledBrush`, `ButtonDisabledForegroundBrush` (no XAML or C# lookups). |
| P5 | Doc mismatches / disabled-state inconsistency | 73 language files; non-resizable dialogs no longer declare `MinWidth`; every disabled control uses `Opacity = DisabledOpacity` (0.45). |

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

Rule: margins / paddings / spacings are 4, 8, 12, 16 or 24. Avalonia `Spacing` / `ColumnSpacing` take `double`, so
either use the token or the literal value from the scale.

### 2.2 Corner radii

| Key | Value | Use |
| --- | --- | --- |
| `RadiusSm` | 4 | check boxes, swatches, list items in pop-ups |
| `RadiusMd` | 6 | buttons, text boxes, combo boxes, toggles, tooltips, menu items |
| `RadiusLg` | 8 | cards, pop-ups, context menus |
| `RadiusXl` | 12 | window shells (`DialogShell`), floating panels, overlay toolbar, tray menu |

### 2.3 Typography

Font: `AppFontFamily` (bundled KazukiReiwa, the author's brand font), set for the whole tree by `Window`.

| Key / class | Size | Weight | Use |
| --- | --- | --- | --- |
| `TextBlock.display` | 24 (`FontSizeDisplay`) | Normal | reserved for hero numbers / product names |
| `TextBlock.title` | 18 (`FontSizeTitle`) | SemiBold | window title (set by `DialogShell`) |
| `TextBlock.subtitle` | 15 (`FontSizeSubtitle`) | SemiBold, wraps | prominent in-content heading (About tagline, update status) |
| `TextBlock.section-header` | 15 | SemiBold | section headings (`.first` removes the top margin) |
| body (default) | 14 (`FontSizeBody`) | Normal | everything else |
| `TextBlock.label` | 14 | SemiBold | inline labels (About details, update panel) |
| `TextBlock.secondary` | 14 | Normal, `TextSecondaryBrush`, wraps | supporting text |
| `TextBlock.caption` | 12 (`FontSizeCaption`) | Normal, `TextSecondaryBrush`, wraps | hints, copyright |

### 2.4 Control metrics

| Key | Value | Use |
| --- | --- | --- |
| `ControlHeight` | 32 | min height of buttons, text boxes, combo boxes, check boxes, sliders |
| `MenuItemHeight` | 30 | tray menu rows |
| `ButtonMinWidth` | 96 | min width of dialog footer buttons (`StackPanel.dialog-actions > Button`) |
| `IconButtonSize` | 32 | square icon-only buttons (`Button.icon`) |
| `ToolButtonSize` | 40 | overlay toolbar buttons (`.tool`) |
| `FormLabelMinWidth` / `FormLabelMaxWidth` | 160 / 220 | label column bounds in `LabeledField` (longer labels wrap) |
| `FormFieldMaxWidth` | 460 | field column cap in `LabeledField` |
| `ButtonPadding` / `InputPadding` | 16,4 / 10,4 | chosen so text + padding + border fits in `ControlHeight` |
| `DisabledOpacity` | 0.45 | the single disabled treatment for every interactive control |
| `FocusRingThickness` / `FocusRingMargin` | 2 / -3 | focus ring drawn 1 px outside the control |

### 2.5 Colour brushes (defined in every theme — 54 keys, identical sets)

| Group | Keys |
| --- | --- |
| Surfaces | `PanelBackgroundBrush` (window shell), `PanelAltBackgroundBrush` (floating overlay panels, solid), `PanelBorderBrush`, `CardBackgroundBrush`, `DividerBrush`, `OverlayMaskBrush` |
| Text | `TextPrimaryBrush`, `TextSecondaryBrush`, `TextDisabledBrush` (Fluent context-menu items), `IconBrush` |
| Inputs | `InputBackgroundBrush`, `InputBackgroundHoverBrush`, `InputBorderBrush`, `InputBorderHoverBrush`, `InputForegroundBrush` |
| Secondary buttons | `ButtonBackgroundBrush`, `ButtonBorderBrush`, `ButtonHoverBrush`, `ButtonPressedBrush`, `ButtonForegroundBrush` |
| Subtle | `SubtleHoverBrush`, `SubtlePressedBrush` (translucent overlays for ghost buttons, tabs, menu items, tools) |
| Accent | `AccentPrimaryBrush`, `AccentPrimaryDarkBrush`, `AccentForegroundBrush` (on solid accent fills: check marks, selection), `AccentSubtleBrush` / `AccentSubtleHoverBrush` (tinted checked state), `AccentTextBrush` (accent-coloured text/icons on panels), `AccentButtonBackgroundBrush`, `AccentButtonHoverBrush`, `AccentButtonPressedBrush`, `AccentButtonBorderBrush`, `AccentButtonForegroundBrush`, `SelectionBorderBrush`, `FocusRingBrush` |
| Status | `DangerBrush`, `DangerHoverBrush`, `DangerPressedBrush`, `DangerForegroundBrush`, `SuccessBrush`, `LinkBrush`, `LinkHoverBrush` |
| Chrome | `ScrollBarBackgroundBrush`, `ScrollBarThumbBrush`, `ScrollBarThumbHoverBrush`, `PopupBackgroundBrush`, `PopupBorderBrush`, `HighlightBrush` (selected list item; also read by `TrayMenuWindowService` fallbacks) |
| Sliders | `SliderTrackBrush`, `SliderFillBrush`, `SliderThumbBrush`, `SliderThumbBorderBrush`, `SliderDisabledThumbBrush` |

**Contrast checks (WCAG 2.1). Sunset rows use the worst (darkest / lightest as applicable) gradient stop:**

| Theme | Pair | Ratio |
| --- | --- | --- |
| Dark | text / panel | 14.2 |
| Dark | secondary / panel | 8.5 |
| Dark | link / panel | 6.7 |
| Dark | accent button text | 6.1 |
| Dark | focus ring / panel | 8.0 |
| Dark | checked tool icon / tint on toolbar | 7.5 |
| Light | text / panel | 15.1 |
| Light | secondary / panel | 6.8 |
| Light | link / panel | 6.9 |
| Light | accent button text | 6.1 |
| Light | focus ring / panel | 11.1 |
| Light | checked toggle text / tint | 6.0 |
| Sunset | text / panel (stop `#F0C0A0`) | 9.4 |
| Sunset | secondary / panel (stop `#F0C0A0`) | 4.8 |
| Sunset | secondary vs primary text (hierarchy) | 2.0 |
| Sunset | link / panel | 6.5 |
| Sunset | success / panel | 5.6 |
| Sunset | accent button text — normal (stop `#EE7A45`) | 5.5 |
| Sunset | accent button text — hover (stop `#F08B52`) | 6.3 |
| Sunset | accent button text — pressed (stop `#E9824B`) | 5.7 |
| Sunset | focus ring / panel | 9.4 |
| Sunset | toolbar icon / panel-alt | 13.6 |
| Sunset | checked tool icon / tint | 5.1 |

---

## 3. Components

### 3.1 Buttons

| Class | Look | Use |
| --- | --- | --- |
| *(none)* | neutral fill + border (secondary) | every non-primary action |
| `accent` | accent fill, `AccentButtonForegroundBrush`, SemiBold | **the one primary action of a dialog** |
| `danger` | red fill | destructive actions (Tesseract "Remove" when the language is installed: `Classes.danger="{Binding …IsInstalled}"`) |
| `subtle` | transparent until hover | low-emphasis actions, title-bar close, "Skip this version" |
| `icon` | 32×32 square, icon content | icon-only buttons; combine with `subtle`. **Always** set `ToolTip.Tip` + `AutomationProperties.Name` |
| `menu-item` | full-width, left-aligned, transparent, 30 px | tray menu entries (all visuals come from the style) |
| `swatch` | 22×22 colour chip | overlay quick colours (hex tooltip / automation name) |
| `tool` (Button / ToggleButton) | 40 px round, transparent, icon only; checked = tinted; `tool accent` = the solid primary (Copy) | overlay editor toolbar |

All buttons share one template (`PART_ButtonBorder` + `PART_FocusRing`) that honours `Padding` and `CornerRadius`,
uses `MinHeight = ControlHeight`, a hand cursor, and hover / pressed / `:focus-visible` states. Disabled = whole
control at `DisabledOpacity`. **Do not** name a presenter `PART_ContentPresenter` in a Button template: Fluent's
built-in `Button.accent` / state selectors target that name and would override Shotora's colours.

Focus ring: 2 px `FocusRingBrush`, drawn outside the control with a 1 px gap so it reads on any fill. Containers that
clip (scroll viewers) must leave ≥ 4 px around focusable children (see `ScrollViewer.settings-page`). Menu rows draw
their ring inside because the menu has no outside room.

Content may be a string or an icon `Geometry` (`Content="{StaticResource IconCopy}"`); `IconContentTemplate` renders
the geometry in the button's current foreground.

Links use `HyperlinkButton` with `NavigateUri` (no code-behind; keyboard and screen-reader accessible), styled with
`LinkBrush` / `LinkHoverBrush`, underline and a focus ring; a -2 px margin cancels the padding so link text lines up
with surrounding text.

### 3.2 Toggles and check boxes

Selection states never use the solid accent fill (that is reserved for the primary action):

- `ToggleButton.toggle` — two-state text toggle (Bold / Italic). Checked = `AccentSubtleBrush` fill, accent border,
  `AccentTextBrush` text; checked + hover = `AccentSubtleHoverBrush`.
- `ToggleButton.tool` — same tinted treatment in the overlay toolbar.
- `CheckBox` — themed box; the accent appears only as the checked box fill with the check mark.
- `CheckBox.icon-option` — check box + neutral 20 px icon (`Tag` = `Geometry`) + wrapping label (Settings › Editor
  icons).

### 3.3 Inputs

- `TextBox`: `ControlHeight`, `RadiusMd`, border states (hover `InputBorderHoverBrush`, focus `AccentPrimaryBrush`),
  accent selection. Multi-line editable input → `AcceptsReturn="True"`; multi-line **read-only** text →
  `Classes="multiline"` (keeps Enter for the dialog's default button). Both wrap and scroll vertically.
- `SearchableComboBox`: same metrics; hover / focus-within border states; chevron without divider; pop-up `RadiusLg`;
  Esc closes the pop-up first (handled) and only then reaches the dialog. Its automation name is forwarded to the
  inner text box.
- `Slider.quality`: `ControlHeight` tall, accent fill, round 16 px thumb, focus border on the thumb.
- `ProgressBar`: 6 px, accent fill.

### 3.4 Layout primitives

- **`DialogShell`** (window chrome) — use as the root of every custom-decorated window:

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

  12 px radius, 1 px border, title bar (18 px SemiBold title, optional `Subtitle`, subtle 32 px close button that is not a
  tab stop), 16 px footer padding. **Drag area:** the title bar and empty parts of the footer (presses handled by
  controls, presses in content, and events coming from pop-ups never move the window). Set
  `Shell.ResizeService = <IMouseInteractionService>` in code-behind to enable edge resizing (cursors cached per edge).
  Footer content stays in the logical tree, so localized `DynamicResource`s and `IsDefault` / `IsCancel` work.
- **`StackPanel.dialog-actions`** — right-aligned, 8 px spaced action row; buttons get `MinWidth = ButtonMinWidth`.
  Order: secondary first, primary (`accent`) last / right-most.
- **`LabeledField`** (forms):

  ```xml
  <StackPanel Grid.IsSharedSizeScope="True">
      <controls:LabeledField Label="{DynamicResource LocSettingsTheme}"> …field… </controls:LabeledField>
      <controls:LabeledField> <CheckBox …/> </controls:LabeledField>   <!-- no label -->
  </StackPanel>
  ```

  Labels share one auto-sized column (160–220 px, wrapping beyond that); fields stretch up to 460 px. An empty `Label`
  collapses its own cell (no indent); the row still aligns with the field column whenever other rows in the scope have
  labels. The label becomes the field's automation name unless the field sets one.
- **`Border.card`** — card background, 1 px border, `RadiusLg`, 12 px padding.
- **`Border.floating-panel`** — `PanelAltBackgroundBrush`, `RadiusXl`; overlay toolbar / palette / coordinates chip.
- **`Border.toolbar-separator`**, **`Border.menu-separator`** — 1 px `DividerBrush` dividers.
- **`CustomTabs`** — underline tabs on a divider; selected = 3 px accent indicator + primary text; unselected =
  secondary text, subtle hover; wraps for long labels; arrow-key navigation and an inside focus ring.

### 3.5 Keyboard & accessibility rules

1. **Esc** closes every dialog: give the dismiss button `IsCancel="True"`. (Overlay keeps its own Esc handling: leave
   the current tool first, close on the second press.)
2. **Enter** triggers the primary action where unambiguous (`IsDefault="True"`): About, Main, Update, Text dialog,
   OCR Copy. Settings has no default button (Enter is used inside combo boxes). In the multi-line text dialog,
   **Ctrl+Enter** places the text.
3. Every icon-only control has `ToolTip.Tip` and `AutomationProperties.Name` from existing `Loc*` keys (colour
   swatches use their hex value).
4. Focus is always visible: interactive templates show a 2 px `FocusRingBrush` ring outside the control on
   `:focus-visible`, and disable Fluent's square `FocusAdorner` to avoid double rings.
5. Never hard-code user-visible English in XAML; use existing `Loc*` keys. If a new key is unavoidable, add it to all 73
   `Strings.*.axaml` files.
6. Text that can be translated must wrap or trim (`TextWrapping="Wrap"` on body copy; labels go through `LabeledField`).
7. Disabled = `Opacity = DisabledOpacity` on the whole control; no per-control disabled brushes.
8. Screen positions are physical pixels and control sizes are DIPs; convert with the screen / render scaling
   (see `TrayMenuWindowService.CalculateMenuPosition`, `OcrResultWindow.PositionNearAnchor`).

### 3.6 Window inventory

| Window | Size / min | Chrome | Footer (secondary → primary) | Keys |
| --- | --- | --- | --- | --- |
| MainWindow | 420×260 / 320×220, resizable | DialogShell | Close (accent) | Enter / Esc close |
| SettingsWindow | 680×720 / 560×440, resizable | DialogShell + CustomTabs, per-tab scroll | Close (accent, saves) | Esc closes (and saves) |
| AboutWindow | 460×640 / 380×420, resizable | DialogShell, scrolling body; Version row + Updates card (`UpdatePanel`, secondary buttons only) | Close (accent) | Enter / Esc close |
| UpdateWindow | 520×460 / 400×300, resizable | DialogShell, status header (`subtitle`), release-notes card | Skip (subtle, left) · Later → one contextual accent action | Esc later, Enter primary |
| TextInputWindow | 520 wide, height sizes to content, fixed | DialogShell | Cancel → Place text (accent) | Esc cancel, Enter (single-line fields) / Ctrl+Enter place |
| OcrResultWindow | 480×260, fixed, positioned next to the selection | DialogShell (drag only) | Close → Copy (accent, shows "Copied to clipboard") | Esc close, Enter copy |
| OverlayWindow | full screen | floating toolbar (wraps, centred rows, output group after a divider), coordinates chip, draggable palette | — | existing editor hotkeys |
| Tray menu | sized to content, 30 px rows | built in `TrayMenuWindowService`, style-driven entries | — | — |

---

## 4. Known gaps / follow-ups

- **Corrupted translations** (pre-existing): `Strings.hr/hsb/hu/is/sl/sr.axaml` contain many U+FFFD characters; being
  re-translated separately.
- `TrayMenuWindowService.ApplyTheme` still themes the tray window chrome (root background/border, separator, text) in
  code because the menu is a code-built top-level window; entries are fully style-driven.
- `ColorPalettePicker` keeps functional hard-coded colours (hue/alpha gradients, checkerboard, thumb outlines) — these
  are content, not chrome.
- Windows render with transparent backgrounds and rounded shells; on platforms without per-pixel transparency the
  corners render on black (unchanged behaviour).
