# PennyPincher Style Guide

## Visual Direction

Lain-inspired dark aesthetic. CRT monitor vibes — scanlines, vignette, phosphor glow. Terminal/monospace for data and accents. Clean and minimal in the main app, expressive on the login splash.

## Color System

### Themes

Five switchable accent themes. Stored in `localStorage('pp-theme')`. No saved preference = random each visit.

| Theme | Accent | RGB | Glitch 1 | Glitch 2 | Dim |
|-------|--------|-----|----------|----------|-----|
| Amber | `#e8b44c` | `232,180,76` | `#e6677a` | `#d4a040` | `#3a2e18` |
| Violet | `#a78bfa` | `167,139,250` | `#e6677a` | `#7c5cbf` | `#2a2240` |
| Rose | `#e07a8e` | `224,122,142` | `#c94060` | `#60b4f7` | `#3a1e28` |
| Silver | `#b0b0c0` | `176,176,192` | `#8888a0` | `#6868a0` | `#22222e` |
| Mint | `#4ade80` | `74,222,128` | `#e6677a` | `#38b060` | `#1a2e20` |

### Base Palette (theme-independent)

| Role | Color | Usage |
|------|-------|-------|
| Background | `#0c0c10` | Page background, input fills |
| Surface | `#111118` | Cards, elevated surfaces |
| Border | `#1a1a22` | Dividers, input borders (idle) |
| Grid | `#22222e` | Background graph grid lines |
| Text Primary | `#e8e8ed` | Headings, main content |
| Text Secondary | `#a0a0b4` | Subtitles, labels |
| Text Ghost | `#4a4a5e` | Placeholders, hints |
| Text Muted | `#3d3d50` | Disabled states |

### Semantic Colors (always these, regardless of theme)

| Role | Color | Usage |
|------|-------|-------|
| Income / Positive | `#47d17a` | Income amounts, positive trends |
| Expense / Negative | `#e66a7a` | Expense amounts, negative trends |
| Info | `#60b4f7` | Informational highlights |
| Warning | `#f5c842` | Warnings, pending states |

### CSS Custom Properties

All accent references use CSS variables set on `.login-page` (and eventually `:root` for the main app):

```css
--accent       /* Main accent color */
--accent-rgb   /* Comma-separated RGB for rgba() usage */
--glitch1      /* Chromatic aberration color 1 */
--glitch2      /* Chromatic aberration color 2 */
--accent-dim   /* Dark tint for subtle backgrounds */
```

Usage: `color: var(--accent)` or `rgba(var(--accent-rgb), .15)` for transparency.

## Typography

| Context | Font | Weight | Size |
|---------|------|--------|------|
| App title (glitch) | System (Segoe UI) | 800 | 3.6rem |
| Page headings | System | 700 | — |
| Body text | System | 400 | — |
| Subtitles / code | `Courier New`, monospace | 400 | 0.82–0.88rem |
| Input values | `Courier New`, monospace | 400 | 0.82rem |
| Labels / hints | `Courier New`, monospace | 400 | italic, ghost color |
| Button text | `Courier New`, monospace | 600 | 0.82rem |

## Login Splash

### Elements

- **Glitch title**: Chromatic aberration via `::before`/`::after` pseudo-elements with `clip-path` animation. Cycle: 3.5s. Displacement: 4–6px.
- **Subtitles**: Random from pool (3 one-liners + 7 haikus). One-liners type out with cursor. Haikus type line-by-line sequentially.
- **Background**: Randomly generated SVG graph (bull = accent color trending up, bear = glitch1 trending down). Scrolls left at 35s loop. Grid + area fill + 2 polylines.
- **Scanlines**: 2px repeating gradient, 4% opacity.
- **Vignette**: Radial gradient darkening edges.
- **Screen flash**: Brief white flash at 0.58s on load.
- **Corner brackets**: Decorative, fade in at 2s.
- **Theme dots**: 6 dots at bottom center (1 rainbow random + 5 themes). Click to switch live.

### Subtitle Pool

**One-liners** (typed with blinking cursor):
- stonks, but for real
- because ramen gets old
- adulting, but make it cute

**Haikus** (typed line-by-line):
- pennies become more / when you watch where they wander / wealth begins with sight
- numbers tell the truth / your wallet has a story / let the data speak
- small leaks sink big ships / but those who track every drop / sail to golden shores
- data over doubt / your finances laid bare here / clarity is power
- money comes and goes / but the ones who track it know / where the river flows
- swipe and forget not / every purchase tells a tale / read between the lines
- less chaos more cash / when you know where pennies go / dollars follow suit

## Form Elements (Login)

- **Inputs**: Transparent background, bottom-border only (`#1a1a22` idle → accent on focus/filled). Monospace, centered text. Text color = accent when filled. Chrome autofill overridden to match.
- **Button**: Black background (`#0c0c10`), accent border + text. Fills accent on hover with glow. Blinking `_` cursor in text.
- **Placeholders**: Italic, ghost color (`#4a4a5e`).
- **Error alerts**: Transparent with red border, red text with subtle glow.

## Animation Principles

- **Login splash**: Go wild. Glitch, type, scroll, flash, glow.
- **Main app**: Restrained. Purposeful transitions only — page loads, data appearing, filter changes. No decorative animation.
- **Timing**: Use `cubic-bezier(.4,0,.2,1)` for draws, `ease-in-out` for smooth transitions, `step-end` for discrete state changes.
- **Duration**: 0.2–0.3s for UI feedback, 1–2s for reveals, 3–5s for background loops.

## File Structure

| File | Role |
|------|------|
| `wwwroot/css/site.css` | All styles including login splash |
| `Pages/Login.cshtml` | Splash HTML + theme/subtitle/graph JS |
| `Pages/Shared/_Layout.cshtml` | App shell, sidebar, scripts |
